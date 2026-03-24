using Application.DTOs.Product;
using Application.DTOs.PurchaseOrder;
using Application.DTOs.Supplier;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common;

namespace Application.Interfaces
{
    public interface ISupplierService
    {
        Task<PaginatedResult<SupplierDto>> GetAllAsync(PaginationParams @params);
        Task<SupplierDto> GetByIdAsync(int id);
        Task<List<ProductDto>> GetProductsBySupplierIdAsync(int supplierId);
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<bool> UpdateAsync(int id, UpdateSupplierDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<List<PurchaseOrderDto>> GetPurchaseOrdersBySupplierId(int supplierId);
    }
}
