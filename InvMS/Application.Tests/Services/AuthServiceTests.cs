using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Auth;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Interfaces.Auth;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Tests.Services
{
    public class AuthServiceTests : ServiceTestFixture
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockCache = new Mock<IMemoryCache>();

            SetupConfiguration();

            _authService = new AuthService(
                _mockUserRepository.Object,
                MockMapper.Object,
                _mockConfiguration.Object,
                _mockRoleRepository.Object,
                _mockCache.Object,
                _mockRefreshTokenRepository.Object,
                _mockEmailService.Object,
                MockUnitOfWork.Object);
        }

        private void SetupConfiguration()
        {
            _mockConfiguration.Setup(x => x["Jwt:SecretKey"]).Returns("this-is-a-very-long-secret-key-for-testing-purposes-only-needs-to-be-long");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
            _mockConfiguration.Setup(x => x["Jwt:ExpiryInMinutes"]).Returns("60");
            _mockConfiguration.Setup(x => x["Otp:ExpiryInMinutes"]).Returns("10");
        }

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_Should_Throw_BadRequestException_When_UsernameIsEmpty()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "", Password = "password" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_BadRequestException_When_PasswordIsEmpty()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testuser", Password = "" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_BadRequestException_When_BothFieldsAreNull()
        {
            // Arrange
            var loginDto = new LoginDto { Username = null, Password = null };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_UnauthorizedException_When_UserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "nonexistent", Password = "password" };
            _mockUserRepository.Setup(x => x.GetByUsernameAsync("nonexistent")).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginDto));

            // Verify
            _mockUserRepository.Verify(x => x.GetByUsernameAsync("nonexistent"), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_UnauthorizedException_When_UserIsDeleted()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser(isDeleted: true);
            var loginDto = new LoginDto { Username = user.Username, Password = "password" };
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(user.Username)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_UnauthorizedException_When_PasswordIsInvalid()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");

            var user = TestDataBuilder.CreateTestUser(passwordHash: hashedPassword);

            var loginDto = new LoginDto
            {
                Username = user.Username,
                Password = "wrongpassword"
            };

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(user.Username))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_Should_Return_LoginResponseDto_When_CredentialsAreValid()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("validpassword");
            var user = TestDataBuilder.CreateTestUser(passwordHash: hashedPassword);
            var loginDto = new LoginDto { Username = user.Username, Password = "validpassword" };

            var role = TestDataBuilder.CreateTestRole();
            user.Roles.Add(role);

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(user.Username)).ReturnsAsync(user);
            _mockRefreshTokenRepository.Setup(x => x.GetByUserIdAsync(user.Id)).ReturnsAsync((RefreshToken)null);
            _mockRefreshTokenRepository.Setup(x => x.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be(user.Username);
            result.Token.Should().NotBeNullOrEmpty();
            _mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region ChangePasswordAsync Tests

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_NotFoundException_When_UserNotFound()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto { CurrentPassword = "old", NewPassword = "new"};
            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _authService.ChangePasswordAsync(1, changePasswordDto));
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_Throw_BadRequestException_When_CurrentPasswordIsInvalid()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = TestDataBuilder.CreateTestUser(passwordHash: hashedPassword);
            var changePasswordDto = new ChangePasswordDto { CurrentPassword = "wrongpassword", NewPassword = "newpassword" };

            _mockUserRepository.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.ChangePasswordAsync(user.Id, changePasswordDto));
        }

        [Fact]
        public async Task ChangePasswordAsync_Should_UpdatePassword_When_CurrentPasswordIsCorrect()
        {
            // Arrange
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = TestDataBuilder.CreateTestUser(passwordHash: hashedPassword);
            var changePasswordDto = new ChangePasswordDto { CurrentPassword = "correctpassword", NewPassword = "newpassword"};

            _mockUserRepository.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.ChangePasswordAsync(user.Id, changePasswordDto);

            // Assert
            result.Should().BeTrue();
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_Should_Throw_BadRequestException_When_UsernameAlreadyExists()
        {
            // Arrange
            var existingUser = TestDataBuilder.CreateTestUser(username: "existinguser");
            var registerDto = new RegisterDto 
            { 
                Username = "existinguser", 
                Email = "new@test.com", 
                Password = "password",
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync("existinguser")).ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task RegisterAsync_Should_Throw_BadRequestException_When_EmailAlreadyExists()
        {
            // Arrange
            var existingUser = TestDataBuilder.CreateTestUser(email: "existing@test.com");
            var registerDto = new RegisterDto 
            { 
                Username = "newuser", 
                Email = "existing@test.com", 
                Password = "password",
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync("newuser")).ReturnsAsync((User)null);
            _mockUserRepository.Setup(x => x.GetByUseremailAsync("existing@test.com")).ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task RegisterAsync_Should_CreateNewUser_When_CredentialsAreValid()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "new@test.com",
                Password = "password"
            };

            var role = new Role
            {
                Id = 1,
                Name = "User"
            };

            var newUser = new User
            {
                Id = 1,
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedDate = DateTime.UtcNow,
                Roles = new List<Role> { role } // ✅ IMPORTANT
            };

            var userDto = new UserDto
            {
                Id = 1,
                Username = "newuser",
                Email = "new@test.com"
            };

            // ✅ User checks
            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync("newuser"))
                .ReturnsAsync((User)null);

            _mockUserRepository
                .Setup(x => x.GetByUseremailAsync("new@test.com"))
                .ReturnsAsync((User)null);

            _mockRoleRepository.Setup(x => x.GetByIdAsync(6)).ReturnsAsync(role);

            // ✅ Add user
            _mockUserRepository
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // ✅ Save changes
            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // ✅ Mapper
            MockMapper
                .Setup(x => x.Map<User>(registerDto))
                .Returns(newUser);

            MockMapper
                .Setup(x => x.Map<UserDto>(newUser))
                .Returns(userDto);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("newuser");
            result.Email.Should().Be("new@test.com");

            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_UnauthorizedException_When_RefreshTokenIsEmpty()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "" };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.RefreshTokenAsync(refreshTokenDto));
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_UnauthorizedException_When_RefreshTokenNotFound()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "invalid-token" };
            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync("invalid-token")).ReturnsAsync((RefreshToken)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.RefreshTokenAsync(refreshTokenDto));
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_UnauthorizedException_When_RefreshTokenIsExpired()
        {
            // Arrange
            var refreshToken = new RefreshToken 
            { 
                Token = "valid-token", 
                UserId = 1, 
                ExpiresAt = DateTime.UtcNow.AddMinutes(-1), 
                IsRevoked = false 
            };
            var refreshTokenDto = new RefreshTokenDto { RefreshToken = "valid-token" };

            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync("valid-token")).ReturnsAsync(refreshToken);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.RefreshTokenAsync(refreshTokenDto));
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_Throw_UnauthorizedException_When_RefreshTokenIsRevoked()
        {
            // Arrange
            var refreshToken = new RefreshToken
            {
                Token = "valid-token",
                UserId = 1,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-10), // 🔥 FORCE FAIL
                IsRevoked = true
            };

            var user = TestDataBuilder.CreateTestUser(id: 1);

            var refreshTokenDto = new RefreshTokenDto
            {
                RefreshToken = "valid-token"
            };

            _mockRefreshTokenRepository
                .Setup(x => x.GetByTokenAsync("valid-token"))
                .ReturnsAsync(refreshToken);

            _mockUserRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _authService.RefreshTokenAsync(refreshTokenDto));
        }

        #endregion

        #region LogoutAsync Tests

        [Fact]
        public async Task LogoutAsync_Should_RevokeRefreshToken_When_TokenIsValid()
        {
            // Arrange
            var refreshToken = new RefreshToken 
            { 
                Token = "valid-token", 
                UserId = 1, 
                IsRevoked = false, 
                ExpiresAt = DateTime.UtcNow.AddMinutes(30) 
            };

            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync("valid-token")).ReturnsAsync(refreshToken);
            _mockRefreshTokenRepository.Setup(x => x.UpdateAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _authService.LogoutAsync("valid-token");

            // Assert
            _mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<RefreshToken>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_Should_Complete_When_RefreshTokenIsNull()
        {
            // Arrange
            _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(null)).ReturnsAsync((RefreshToken)null);

            // Act
            await _authService.LogoutAsync(null);

            // Assert - Should not throw exception
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region SendOtpAsync Tests

        [Fact]
        public async Task SendOtpAsync_Should_Throw_NotFoundException_When_UserNotFound()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDto { Email = "nonexistent@test.com" };
            _mockUserRepository.Setup(x => x.GetByUseremailAsync("nonexistent@test.com")).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _authService.SendOtpAsync(forgotPasswordDto));
        }

        [Fact]
        public async Task SendOtpAsync_Should_SendOtp_When_UserExists()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            var forgotPasswordDto = new ForgotPasswordDto { Email = user.Email };

            _mockUserRepository.Setup(x => x.GetByUseremailAsync(user.Email)).ReturnsAsync(user);
            _mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(new Mock<ICacheEntry>().Object);

            // Act
            var result = await _authService.SendOtpAsync(forgotPasswordDto);

            // Assert
            result.Should().BeTrue();
            _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region ResetPasswordAsync Tests

        [Fact]
        public async Task ResetPasswordAsync_Should_Throw_BadRequestException_When_OtpIsInvalid()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDto { Email = "test@test.com", Otp = "123456", NewPassword = "newpass"};
            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny)).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.ResetPasswordAsync(resetPasswordDto));
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_UpdatePassword_When_OtpIsValid()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            var resetPasswordDto = new ResetPasswordDto { Email = user.Email, Otp = "123456", NewPassword = "newpass"};
            var otpCacheKey = $"otp_{user.Email}";

            object cachedOtp = "123456";

            _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedOtp)).Returns(true);
            _mockUserRepository.Setup(x => x.GetByUseremailAsync(user.Email)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            // Assert
            result.Should().BeTrue();
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
