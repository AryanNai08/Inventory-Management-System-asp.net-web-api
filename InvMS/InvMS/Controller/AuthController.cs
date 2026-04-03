using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        [Route("register")]
        //[Authorize(Roles ="Admin")]
        [Authorize(Policy ="ManageUserRegistration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<UserDto>>> RegisterUserAsync([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return CreatedAtRoute("GetUserById", new { id = result.Id }, new APIResponse<UserDto>(result, "User registered successfully"));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<LoginResponseDto>>> LoginUser([FromBody] LoginDto dto)
        {
            var response= await _authService.LoginAsync(dto);

            Response.Cookies.Append("accesstoken", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });
            Response.Cookies.Append("refreshtoken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new APIResponse<LoginResponseDto>(response, "Login successful"));
        }


        [HttpPost]
        [Authorize]
        [Route("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var result = await _authService.ChangePasswordAsync(userId, dto);
            return Ok(new APIResponse<object>(result, "Password changed successfully"));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);

            Response.Cookies.Append("accesstoken", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(1)
            });
            Response.Cookies.Append("refreshtoken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new APIResponse<LoginResponseDto>(response, "Token refreshed successfully"));
        }






        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse<object>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.SendOtpAsync(dto);
            return Ok(new APIResponse<object>(result, "OTP sent successfully"));
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse<object>>> ResetPassword([FromBody]ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Ok(new APIResponse<object>(result, "Password reset successfully"));
        }


    }
}
