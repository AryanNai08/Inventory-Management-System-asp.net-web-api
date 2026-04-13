using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface IRolePrivilegeRepository
    {
        Task<Role> GetRoleWithPrivilegesAsync(int roleId);
        Task<Privilege> GetPrivilegeByIdAsync(int privilegeId);
        Task UpdateRoleAsync(Role role);
        Task<bool> AnyPrivilegeInRoleAsync(int roleId);
    }
}
