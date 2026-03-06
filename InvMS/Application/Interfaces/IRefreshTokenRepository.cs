using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task UpdateAsync(RefreshToken token);
    }
}
