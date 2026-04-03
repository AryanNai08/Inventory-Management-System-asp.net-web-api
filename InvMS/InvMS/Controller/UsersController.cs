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

        public UsersController(IUserService userService) 
        {
            _userService=userService;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<UserDto>>>> GetAllUSer() 
        { 
            var result = await _userService.GetAllAsync();
            return Ok(new APIResponse<IEnumerable<UserDto>>(result, "Users fetched successfully"));
        }


        [HttpGet]
        [Route("{id:int}", Name = "GetUserById")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse<UserDto>>> GetUserById(int id) 
        {
            var result = await _userService.GetByIdAsync(id);
            return Ok(new APIResponse<UserDto>(result, "User fetched successfully"));
        }

        [HttpDelete]
        [Route("{id:int}", Name = "DeleteUser")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse<bool>>> DeleteUserById(int id)
        {
            var result = await _userService.SoftDeleteAsync(id);
            return Ok(new APIResponse<bool>(result, "User deleted successfully"));
        }

        [HttpPut]
        [Route("{id:int}", Name = "Update")]
        [Authorize(Policy = "ManageUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<bool>>> UpdateUserAsync(int id,[FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);
            return Ok(new APIResponse<bool>(result, "User updated successfully"));
        }



        [HttpGet]
        [Route("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse<UserDto>>> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var result = await _userService.GetByIdAsync(userId);
            return Ok(new APIResponse<UserDto>(result, "Profile fetched successfully"));
        }
    }
}
