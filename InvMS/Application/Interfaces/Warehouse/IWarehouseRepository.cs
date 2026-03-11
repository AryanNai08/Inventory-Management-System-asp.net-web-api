using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IWarehouseRepository
    {
        public Task<List<Warehouse>> GetAllAsync();
        public Task<Warehouse> GetByIdAsync(int id);
        public Task<Warehouse> GetByNameAsync(string name);
        public Task AddAsync(Warehouse warehouse);
        public Task UpdateAsync(Warehouse warehouse);
        public Task SoftDeleteAsync(int id);
    }
}
