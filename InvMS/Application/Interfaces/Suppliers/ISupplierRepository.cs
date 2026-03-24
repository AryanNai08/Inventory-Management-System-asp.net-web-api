using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISupplierRepository
    {
        public Task<PaginatedResult<Supplier>> GetAllAsync(PaginationParams @params);
        public Task<Supplier> GetByIdAsync(int id);
        public Task<Supplier> GetByNameAsync(string name);
        public Task<List<Product>> GetProductsBySupplierIdAsync(int supplierId);
        public Task AddAsync(Supplier supplier);
        public Task UpdateAsync(Supplier supplier);
        public Task SoftDeleteAsync(int id);
        public Task<List<PurchaseOrder>> GetPurchaseOrdersBySupplierIdAsync(int supplierId);
    }
}
