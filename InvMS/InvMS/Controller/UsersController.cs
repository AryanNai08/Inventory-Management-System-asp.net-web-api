using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly APIResponse _apiResponse;

        public UsersController(IUserService userService,APIResponse apiResponse) 
        {
            _userService=userService;
            _apiResponse=apiResponse;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllUSer() 
        { 
          _apiResponse.Data = await _userService.GetAllAsync();
            _apiResponse.StatusCode=HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }


        [HttpGet]
        [Route("{id:int}", Name = "GetUserById")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetUserById(int id) 
        {
            _apiResponse.Data = await _userService.GetByIdAsync(id);
            _apiResponse.StatusCode=HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteUser")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> DeleteUserById(int id)
        {
            _apiResponse.Data = await _userService.SoftDeleteAsync(id);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }

        [HttpPut]
        [Route("{id:int}", Name = "Update")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateUserAsync(int id,[FromBody] UpdateUserDto dto)
        {

            var result = await _userService.UpdateAsync(id, dto);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Data = result;

            return Ok(_apiResponse);


        }



        [HttpGet]
        [Route("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            _apiResponse.Data = await _userService.GetByIdAsync(userId);
            _apiResponse.StatusCode = System.Net.HttpStatusCode.OK;
            _apiResponse.Status = true;
            return Ok(_apiResponse);
        }
    }
}
