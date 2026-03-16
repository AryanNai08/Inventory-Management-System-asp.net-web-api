using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Supplier;
using Application.DTOs.Product;

namespace Application.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllAsync();
        Task<SupplierDto> GetByIdAsync(int id);
        Task<List<ProductDto>> GetProductsBySupplierIdAsync(int supplierId);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<bool> UpdateAsync(int id, UpdateSupplierDto dto);
        Task<bool> SoftDeleteAsync(int id);
    }
}
