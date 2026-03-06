using Application.DTOs;


namespace Application.Interfaces
{
    public interface IAuthService
    {
        public Task<UserDto>  RegisterAsync(RegisterDto dto);

        public Task<LoginResponseDto> LoginAsync(LoginDto dto);

        public Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);

        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto);

        public Task<bool> SendOtpAsync(ForgotPasswordDto dto);

        public Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
