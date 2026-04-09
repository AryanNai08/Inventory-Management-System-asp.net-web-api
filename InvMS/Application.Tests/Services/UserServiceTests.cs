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

namespace Application.Tests.Services
{
    public class UserServiceTests : ServiceTestFixture
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();

            _userService = new UserService(
                MockMapper.Object,
                _mockUserRepository.Object,
                MockUnitOfWork.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoUsersExist()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<User>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetAllAsync());
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnAllUsers_When_UsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                TestDataBuilder.CreateTestUser(1, "user1"),
                TestDataBuilder.CreateTestUser(2, "user2")
            };
            var userDtos = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "user1" },
                new UserDto { Id = 2, Username = "user2" }
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);
            MockMapper.Setup(x => x.Map<List<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsNegative()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.GetByIdAsync(-1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_UserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnUser_When_UserExists()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser(1, "testuser");
            var userDto = new UserDto { Id = 1, Username = "testuser" };

            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
            MockMapper.Setup(x => x.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.SoftDeleteAsync(0));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_UserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_DeleteUser_When_UserExists()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser(1);
            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.SoftDeleteAsync(1)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.SoftDeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            _mockUserRepository.Verify(x => x.SoftDeleteAsync(1), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateUserDto { Email = "updated@test.com" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _userService.UpdateAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_UserNotFound()
        {
            // Arrange
            var updateDto = new UpdateUserDto { Email = "updated@test.com" };
            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateUser_When_UserExists()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser(1);
            var updateDto = new UpdateUserDto { Email = "updated@test.com" };

            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, user));

            // Act
            var result = await _userService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_SetModifiedDate()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser(1);
            var updateDto = new UpdateUserDto { Email = "updated@test.com" };
            User capturedUser = null;

            _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, user));

            // Act
            await _userService.UpdateAsync(1, updateDto);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser.ModifiedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        #endregion
    }
}
