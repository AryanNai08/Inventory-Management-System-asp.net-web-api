using Application.DTOs.RolesAndPrivileges;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.RoleAndPrivileges
{
    public interface IRolePrivilegeService
    {
        Task AssignPrivilegeToRoleAsync(RolePrivilegeDto dto);
        Task RemovePrivilegeFromRoleAsync(int roleId, int privilegeId);
        Task<List<ReadPrivilegeDto>> GetPrivilegesByRoleIdAsync(int roleId);
    }
}
