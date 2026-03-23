using Application.DTOs.Product;
using Microsoft.AspNetCore.JsonPatch;
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
        public Task<List<ProductDto>> GetLowStockProducts();
        public Task<List<ProductDto>> GetOutOfStockProducts();
        public Task<bool> PatchAsync(int id, JsonPatchDocument<UpdateProductDto> patchDoc);
    }
}
