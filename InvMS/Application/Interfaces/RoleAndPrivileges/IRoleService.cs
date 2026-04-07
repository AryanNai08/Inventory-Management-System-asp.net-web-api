using Application.DTOs.RolesAndPrivileges;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.RoleAndPrivileges
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task CreateRoleAsync(RoleDto roleDto);
        Task UpdateRoleAsync(int id, UpdateRoleDto roleDto);
        Task DeleteRoleAsync(int id);
    }
}
