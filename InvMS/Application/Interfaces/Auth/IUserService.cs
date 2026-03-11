using Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id,UpdateUserDto dto);

        Task<bool> SoftDeleteAsync(int id);

    }
}
