using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPrivilegeRepository
    {
        Task<List<Privilege>> GetAllPrivilegesAsync();
        Task<Privilege> GetPrivilegeByIdAsync(int id);
        Task<bool> PrivilegeExistsAsync(string name);
        Task CreatePrivilegeAsync(Privilege privilege);
        Task UpdatePrivilegeAsync(Privilege privilege);
        Task DeletePrivilegeAsync(Privilege privilege);
    }
}
