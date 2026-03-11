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

        private readonly APIResponse _apiResponse;
        public AuthController(IAuthService authService, APIResponse apiResponse)
        {
            _authService = authService;
            _apiResponse = apiResponse;
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
        public async Task<ActionResult> RegisterUserAsync([FromBody] RegisterDto dto)
        {
            _apiResponse.Data=await _authService.RegisterAsync(dto);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            _apiResponse.Status = true;

            var result = (UserDto)_apiResponse.Data;
            return CreatedAtRoute("GetUserById", new { id = result.Id }, _apiResponse);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> LoginUser([FromBody] LoginDto dto)
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

            _apiResponse.Data = response;
            _apiResponse.StatusCode=HttpStatusCode.OK;
            _apiResponse.Status=true;

            return Ok(_apiResponse);
        }


        [HttpPost]
        [Authorize]
        [Route("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            _apiResponse.Data = await _authService.ChangePasswordAsync(userId, dto);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> RefreshToken([FromBody] RefreshTokenDto dto)
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

            _apiResponse.Data = response;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }






        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            _apiResponse.Data = await _authService.SendOtpAsync(dto);
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<APIResponse>> ResetPassword([FromBody]ResetPasswordDto dto)
        {
            _apiResponse.Data = await _authService.ResetPasswordAsync(dto);
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }


    }
}
