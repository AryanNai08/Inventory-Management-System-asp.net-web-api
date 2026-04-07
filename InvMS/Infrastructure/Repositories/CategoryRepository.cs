using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly InventoryDbContext _dbContext;

        public CategoryRepository(InventoryDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Category category)
        {
            await _dbContext.AddAsync(category);
        }

        public async Task<List<Category>> GetAllAsync()
        {
          return  await _dbContext.Categories.Where(c=>!c.IsDeleted).ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _dbContext.Categories.Where(c =>c.Id==id && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Category> GetByNameAsync(string name)
        {
            return await _dbContext.Categories.Where(c => c.Name == name && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var category= await _dbContext.Categories.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
            category.IsDeleted=true;
            category.ModifiedDate= DateTime.UtcNow;
        }

        public async Task UpdateAsync(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
