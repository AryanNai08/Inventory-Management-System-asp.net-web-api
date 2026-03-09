using Application.Common;
using Application.DTOs.RolesAndPrivileges;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InvMS.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ManageRoleAndPrivilegeMapping")]
    public class RolePrivilegesController : ControllerBase
    {
        private readonly IRolePrivilegeService _rolePrivilegeService;
        private readonly APIResponse _apiResponse;

        public RolePrivilegesController(IRolePrivilegeService rolePrivilegeService, APIResponse apiResponse)
        {
            _rolePrivilegeService = rolePrivilegeService;
            _apiResponse = apiResponse;
        }

        [HttpPost("assign")]
        public async Task<ActionResult<APIResponse>> AssignPrivilege(RolePrivilegeDto dto)
        {
            await _rolePrivilegeService.AssignPrivilegeToRoleAsync(dto);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpGet("{roleId}/privileges")]
        public async Task<ActionResult<APIResponse>> GetPrivilegeByRole(int roleId)
        {
            _apiResponse.Data = await _rolePrivilegeService.GetPrivilegesByRoleIdAsync(roleId);
            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpDelete("{roleId}/{privilegeId}")]
        public async Task<ActionResult<APIResponse>> RemovePrivilege(int roleId, int privilegeId)
        {
            await _rolePrivilegeService.RemovePrivilegeFromRoleAsync(roleId, privilegeId);

            _apiResponse.Status = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
    }
}