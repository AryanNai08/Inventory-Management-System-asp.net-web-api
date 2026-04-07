using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<PaginatedResult<Customer>> GetAllAsync(PaginationParams @params);
        public Task<Customer> GetByIdAsync(int id);
        public Task<Customer> GetByNameAsync(string name);
        public Task<List<Customer>> SearchAsync(string name, string city);
        public Task AddAsync(Customer customer);
        public Task UpdateAsync(Customer customer);
        public Task SoftDeleteAsync(int id);
        public Task<List<SalesOrder>> GetSalesOrdersByCustomerIdAsync(int customerId);
    }
}
