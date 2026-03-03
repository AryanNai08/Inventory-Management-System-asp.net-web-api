using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found");

            //  — Verify old password

            byte[] storedSalt = Convert.FromBase64String(user.PasswordSalt);

            using var hmac = new HMACSHA512(storedSalt);

            byte[] computedHash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(dto.CurrentPassword));

            byte[] storedHash = Convert.FromBase64String(user.PasswordHash);

            if (!CryptographicOperations.FixedTimeEquals(computedHash, storedHash))
                throw new BadRequestException("Current password is incorrect");

            // Generate new hash & salt

            using var newHmac = new HMACSHA512();

            byte[] newPasswordBytes = Encoding.UTF8.GetBytes(dto.NewPassword);

            byte[] newHash = newHmac.ComputeHash(newPasswordBytes);

            string newSaltString = Convert.ToBase64String(newHmac.Key);
            string newHashString = Convert.ToBase64String(newHash);

            //  Update user

            user.PasswordSalt = newSaltString;
            user.PasswordHash = newHashString;
            user.ModifiedDate = DateTime.UtcNow;

            // Save changes

            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid username or password");
            }

            if (user.IsDeleted == true)
            {
                throw new UnauthorizedException("Account is deactivated");
            }

            // Verify Password

            byte[] saltBytes = Convert.FromBase64String(user.PasswordSalt);

            var hmac = new HMACSHA512(saltBytes);

            byte[] computedHash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(dto.Password));

            byte[] storedHash = Convert.FromBase64String(user.PasswordHash);

            if (!computedHash.SequenceEqual(storedHash))
            {
                throw new UnauthorizedException("Invalid username or password");
            }

            // Generate JWT Token

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            // Return Response

            return new LoginResponseDto
            {
                Username = user.Username,
                UserType = user.UserType,
                Token = token
            };
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var username = await _userRepository.GetByUsernameAsync(dto.Username);
            var useremail = await _userRepository.GetByUseremailAsync(dto.Email);
            if (username != null)
            {
                throw new BadRequestException("Username already taken!!");
            }

            if (useremail != null)
            {
                throw new BadRequestException("Email already taken!!");
            }

            //  Generate Hash + Salt
            var hmac = new HMACSHA512();

            byte[] passwordBytes = Encoding.UTF8.GetBytes(dto.Password);

            byte[] hashBytes = hmac.ComputeHash(passwordBytes);

            string passwordSalt = Convert.ToBase64String(hmac.Key);
            string passwordHash = Convert.ToBase64String(hashBytes);

            // 4 Map DTO → User Entity
            var user = _mapper.Map<User>(dto);

            // 5️ Manually assign security fields
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.CreatedDate = DateTime.Now;
            user.IsDeleted = false;

            // 6 Save
            await _userRepository.AddAsync(user);

            //  Map back → UserDto
            return _mapper.Map<UserDto>(user);

        }
    }
}
