using Application.DTOs.Warehouse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Warehouse
{
    public interface IWarehouseService
    {
        public Task<List<WarehouseDto>> GetAllAsync();
        public Task<WarehouseDto> GetByIdAsync(int id);

        public Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);

        public Task<bool> UpdateAsync(int id, UpdateWarehouseDto dto);

        public Task<bool> SoftDeleteAsync(int id);
    }
}
