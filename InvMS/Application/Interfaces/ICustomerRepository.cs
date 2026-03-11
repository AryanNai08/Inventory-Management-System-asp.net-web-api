using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<List<Customer>> GetAllAsync();
        public Task<Customer> GetByIdAsync(int id);
        public Task<Customer> GetByNameAsync(string name);
        public Task AddAsync(Customer customer);
        public Task UpdateAsync(Customer customer);
        public Task SoftDeleteAsync(int id);
    }
}
