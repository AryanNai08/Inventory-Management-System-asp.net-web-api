using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public async Task<ActionResult<APIResponse>> GetAllUSer() 
        { 
          _apiResponse.Data = await _userService.GetAllAsync();
            _apiResponse.StatusCode=HttpStatusCode.OK;
            _apiResponse.Status = true;

            return Ok(_apiResponse);
        }
    }
}
