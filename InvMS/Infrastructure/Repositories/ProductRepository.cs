using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _dbContext;
        public ProductRepository(InventoryDbContext dbContext)
        {
            _dbContext= dbContext;
        }
       
        public async Task<List<Product>> GetAllAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

      
        public async Task<Product> GetBySkuAsync(string sku)
        {
            return await _dbContext.Products
                .Where(p => p.Sku == sku && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }



    }
}
