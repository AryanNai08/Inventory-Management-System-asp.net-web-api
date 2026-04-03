using Application.Common;
using Application.DTOs.RolesAndPrivileges;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManagePrivileges")]
    public class PrivilegesController : ControllerBase
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse<IEnumerable<ReadPrivilegeDto>>>> GetAllPrivileges()
        {
            var response = await _privilegeService.GetAllPrivilegesAsync();
            return Ok(new APIResponse<IEnumerable<ReadPrivilegeDto>>(response, "Privileges fetched successfully"));
        }

        [HttpGet("id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse<ReadPrivilegeDto>>> GetPrivilegeById(int id)
        {
            var response = await _privilegeService.GetPrivilegeByIdAsync(id);
            return Ok(new APIResponse<ReadPrivilegeDto>(response, "Privilege fetched successfully"));
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse<string>>> CreatePrivilege(PrivilegeDto privilegeDto)
        {
            await _privilegeService.CreatePrivilegeAsync(privilegeDto);
            return Ok(new APIResponse<string>("Successful", "Privilege created successfully"));
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse<string>>> UpdatePrivilege(int id, PrivilegeDto privilegeDto)
        {
            await _privilegeService.UpdatePrivilegeAsync(id, privilegeDto);
            return Ok(new APIResponse<string>("Successful", "Privilege updated successfully"));
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse<string>>> DeletePrivilege(int id)
        {
            await _privilegeService.DeletePrivilegeAsync(id);
            return Ok(new APIResponse<string>("Successful", "Privilege deleted successfully"));
        }
    }
}
