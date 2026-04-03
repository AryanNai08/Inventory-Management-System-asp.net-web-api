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

        public RolePrivilegesController(IRolePrivilegeService rolePrivilegeService)
        {
            _rolePrivilegeService = rolePrivilegeService;
        }

        [HttpPost("assign")]
        public async Task<ActionResult<APIResponse<bool>>> AssignPrivilege(RolePrivilegeDto dto)
        {
            await _rolePrivilegeService.AssignPrivilegeToRoleAsync(dto);
            return Ok(new APIResponse<bool>(true, "Privilege assigned successfully"));
        }

        [HttpGet("{roleId}/privileges")]
        public async Task<ActionResult<APIResponse<List<ReadPrivilegeDto>>>> GetPrivilegeByRole(int roleId)
        {
            var result = await _rolePrivilegeService.GetPrivilegesByRoleIdAsync(roleId);
            return Ok(new APIResponse<List<ReadPrivilegeDto>>(result, "Role privileges fetched successfully"));
        }

        [HttpDelete("{roleId}/{privilegeId}")]
        public async Task<ActionResult<APIResponse<bool>>> RemovePrivilege(int roleId, int privilegeId)
        {
            await _rolePrivilegeService.RemovePrivilegeFromRoleAsync(roleId, privilegeId);
            return Ok(new APIResponse<bool>(true, "Privilege removed successfully"));
        }
    }
}