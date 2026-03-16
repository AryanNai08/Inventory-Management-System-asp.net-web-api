using Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IProductService
    {
        public Task<List<ProductDto>> GetAllAsync();
        public Task<ProductDto> GetByIdAsync(int id);
        public Task<ProductDto> GetBySkuAsync(string sku);
        public Task<List<ProductDto>> SearchAsync(string name, int? categoryId, int? supplierId);
        public Task<ProductDto> CreateAsync(CreateProductDto dto);

        public Task<bool> UpdateAsync(int id, UpdateProductDto dto);

        public Task<bool> SoftDeleteAsync(int id);
    }
}
