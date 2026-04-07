using Application.DTOs.Product;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;

namespace Application.Interfaces.Products
{
    public interface IProductService
    {
        public Task<PaginatedResult<ProductDto>> GetAllAsync(PaginationParams @params);
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
