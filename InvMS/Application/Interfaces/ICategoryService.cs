using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<CategoryDto>> GetAllAsync();
        public Task<CategoryDto> GetByIdAsync(int id);

        public Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

        public Task<bool> UpdateAsync(int id,UpdateCategoryDto dto);

        public Task<bool> SoftDeleteAsync(int id);


    }
}
