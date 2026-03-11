using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAllAsync();
        public Task<Category> GetByIdAsync(int id);
        public Task<Category> GetByNameAsync(string name);
        public Task AddAsync(Category category);
        public Task UpdateAsync(Category category);
        public Task SoftDeleteAsync(int id);
    }
}
