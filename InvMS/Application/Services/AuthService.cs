using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Infrastructure.Repositories;
using System.Linq;
using Application.DTOs.Auth;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
        private readonly IMemoryCache _cache;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IRoleRepository roleRepository, IMemoryCache cache, IRefreshTokenRepository refreshTokenRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _roleRepository = roleRepository;
            _cache = cache;
            _refreshTokenRepository = refreshTokenRepository;
            _emailService = emailService;
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

            var privileges = user.Roles
                .SelectMany(r => r.Privileges)
                .Select(p => p.Name)
                .Distinct();

            foreach(var privilege in privileges)
            {
                claims.Add(new Claim("Permission", privilege));
            }

            var readkey = File.ReadAllText(_configuration["Jwt:SecretKey"]);
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(readkey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            var refreshTokenVal = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenVal,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            return new LoginResponseDto
            {
                Username = user.Username,
                Token = token,
                RefreshToken = refreshTokenVal,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);

            if (storedToken == null || storedToken.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token");

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
                throw new UnauthorizedException("User not found");

            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedToken);

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

            /* ADDED PRIVILEGES HERE */

            var privileges = user.Roles
                .SelectMany(r => r.Privileges)
                .Select(p => p.Name)
                .Distinct();

            foreach (var privilege in privileges)
            {
                claims.Add(new Claim("Permission", privilege));
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

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            var newRefreshTokenVal = GenerateRefreshToken();
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshTokenVal,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };
            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            return new LoginResponseDto
            {
                Username = user.Username,
                Token = token,
                RefreshToken = newRefreshTokenVal,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
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

        public async Task<bool> SendOtpAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByUseremailAsync(dto.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            var otp = new Random().Next(100000, 999999).ToString();

            var cacheKey = $"otp:{dto.Email}";

            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(2));

            // For now print OTP (replace with email later)
            //Console.WriteLine($"OTP for {dto.Email}: {otp}");


            await _emailService.SendEmailAsync(
                dto.Email,
                "Password Reset OTP",
                $"Your OTP for password reset is: {otp}. It expires in 2 minutes."
            );

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        
        {
            var cacheKey = $"otp:{dto.Email}";

            if (!_cache.TryGetValue(cacheKey, out var cachedOtp))
                throw new BadRequestException("OTP expired or invalid for this email");

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

    }
}