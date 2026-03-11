using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
        Task<RefreshToken?> GetByIdAsync(int id);
        Task<RefreshToken?> GetByUserIdAsync(int userId);
    }
}
