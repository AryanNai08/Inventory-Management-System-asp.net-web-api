using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRoleRepository
    {
        
            Task<Role> GetByIdAsync(int id);
        Task<bool> RoleExistsAsync(string roleName);
        Task CreateRoleAsync(Role role);
        Task<List<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(Role role);

    }
}
