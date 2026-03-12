using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetBySkuAsync(string sku);
    }
}
