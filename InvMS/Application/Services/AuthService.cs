using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Application.Interfaces.IRoleRepository;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;

        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _roleRepository = roleRepository;
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

            return new LoginResponseDto
            {
                Username = user.Username,
                Token = token,
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
    }
}