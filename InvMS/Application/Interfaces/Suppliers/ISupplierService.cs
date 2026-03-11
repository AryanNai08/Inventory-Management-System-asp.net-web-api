using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.DTOs.Supplier;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ISupplierService
    {
        public Task<List<SupplierDto>> GetAllAsync();
        public Task<SupplierDto> GetByIdAsync(int id);

        public Task<SupplierDto> CreateAsync(CreateSupplierDto dto);

        public Task<bool> UpdateAsync(int id, UpdateSupplierDto dto);

        public Task<bool> SoftDeleteAsync(int id);
    }
}
