using Application.DTOs.Product;
using Application.DTOs.Warehouse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Products
{
    public interface IProductService
    {
        public Task<List<ProductDto>> GetAllAsync();
        public Task<ProductDto> GetByIdAsync(int id);

        public Task<ProductDto> CreateAsync(CreateProductDto dto);

        public Task<bool> UpdateAsync(int id, UpdateProductDto dto);

        public Task<bool> SoftDeleteAsync(int id);
    }
}
