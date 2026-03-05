using Application.Common;
using Application.DTOs;
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
        [Authorize(Roles ="Admin")]
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

            return CreatedAtRoute("GetUserById", new { id = ((UserDto)_apiResponse.Data).Id }, _apiResponse);
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
            _apiResponse.Data =await _authService.LoginAsync(dto);
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


    }
}
