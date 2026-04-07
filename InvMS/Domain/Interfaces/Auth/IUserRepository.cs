using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Auth
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);

        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByUseremailAsync(string email);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task SoftDeleteAsync(int id);

    }
}
  
