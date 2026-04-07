using Application.DTOs.RolesAndPrivileges;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.RoleAndPrivileges
{
    public interface IPrivilegeService
    {
        Task<List<ReadPrivilegeDto>> GetAllPrivilegesAsync();
        Task<ReadPrivilegeDto> GetPrivilegeByIdAsync(int id);
        Task CreatePrivilegeAsync(PrivilegeDto dto);
        Task UpdatePrivilegeAsync(int id, PrivilegeDto dto);
        Task DeletePrivilegeAsync(int id);
    }
}
