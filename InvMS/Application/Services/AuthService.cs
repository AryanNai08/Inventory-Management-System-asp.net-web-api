using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using static Application.Interfaces.IRoleRepository;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
<<<<<<< Updated upstream
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IRoleRepository roleRepository,IRefreshTokenRepository refreshTokenRepository)
=======
        private readonly IMemoryCache _cache;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IRoleRepository roleRepository, IMemoryCache cache)
>>>>>>> Stashed changes
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _roleRepository = roleRepository;
<<<<<<< Updated upstream
            _refreshTokenRepository = refreshTokenRepository;
=======
            _cache = cache;
>>>>>>> Stashed changes
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);

            if (!isValid)
                throw new BadRequestException("Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ModifiedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null)
                throw new UnauthorizedException("Invalid username or password");

            if (user.IsDeleted)
                throw new UnauthorizedException("Account is deactivated");

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValid)
                throw new UnauthorizedException("Invalid username or password");


            // JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }


            // Create access token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

                
                
            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            // Save refresh token in DB
            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshEntity);


            // Response
            return new LoginResponseDto
            {
                Username = user.Username,
                Token = accessToken,
                RefreshToken = refreshToken,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var username = await _userRepository.GetByUsernameAsync(dto.Username);
            var useremail = await _userRepository.GetByUseremailAsync(dto.Email);

            if (username != null)
                throw new BadRequestException("Username already taken");

            if (useremail != null)
                throw new BadRequestException("Email already taken");

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CreatedDate = DateTime.UtcNow;
            user.IsDeleted = false;

            await _userRepository.AddAsync(user);

            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            user.Roles.Add(role);
            await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDto>(user);
        }

<<<<<<< Updated upstream

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);

            if (storedToken == null)
                throw new UnauthorizedException("Invalid refresh token");

            if (storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token expired");

            if (storedToken.IsRevoked == true)
                throw new UnauthorizedException("Refresh token revoked");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);

            if (user == null || user.IsDeleted)
                throw new UnauthorizedException("User not valid");


            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email)
    };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new LoginResponseDto
            {
                Username = user.Username,
                Token = accessToken,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
        }

        
=======
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var cacheKey = $"otp:{dto.Email}";

            if (!_cache.TryGetValue(cacheKey, out var cachedOtp))
                throw new BadRequestException("OTP expired or invalid");

            if (cachedOtp?.ToString() != dto.Otp)
                throw new BadRequestException("Invalid OTP");

            var user = await _userRepository.GetByUseremailAsync(dto.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ModifiedDate = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            _cache.Remove(cacheKey);

            return true;
        }

        public async Task<bool> SendOtpAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByUseremailAsync(dto.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            var otp = new Random().Next(100000, 999999).ToString();

            var cacheKey = $"otp:{dto.Email}";

            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));

            // For now print OTP (replace with email later)
            Console.WriteLine($"OTP for {dto.Email}: {otp}");

            return true;
        }
>>>>>>> Stashed changes
    }
}