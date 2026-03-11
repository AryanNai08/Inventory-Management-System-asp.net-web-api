using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ISupplierRepository
    {
        public Task<List<Supplier>> GetAllAsync();
        public Task<Supplier> GetByIdAsync(int id);
        public Task<Supplier> GetByNameAsync(string name);
        public Task AddAsync(Supplier supplier);
        public Task UpdateAsync(Supplier supplier);
        public Task SoftDeleteAsync(int id);
    }
}
