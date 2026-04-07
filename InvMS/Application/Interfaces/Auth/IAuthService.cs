using Application.DTOs.Auth;


namespace Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<UserDto>  RegisterAsync(RegisterDto dto);

        public Task<LoginResponseDto> LoginAsync(LoginDto dto);

        public Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);

        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto);

        public Task<bool> SendOtpAsync(ForgotPasswordDto dto);

        public Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task LogoutAsync(string? refreshToken);
    }
}
