using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly InventoryDbContext _dbContext;

        public CustomerRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Customer customer)
        {
            await _dbContext.AddAsync(customer);
        }

        public async Task<PaginatedResult<Customer>> GetAllAsync(PaginationParams @params)
        {
            var query = _dbContext.Customers
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                else if (@params.SortColumn.Equals("City", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(c => c.City) : query.OrderBy(c => c.City);
                else
                    query = query.OrderBy(c => c.Id);
            }
            else
            {
                query = query.OrderBy(c => c.Id);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();

            return new PaginatedResult<Customer>(items, count, @params.PageNumber, @params.PageSize);
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Customer> GetByNameAsync(string name)
        {
            return await _dbContext.Customers.Where(c => c.Name == name && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<List<SalesOrder>> GetSalesOrdersByCustomerIdAsync(int customerId)
        {
                return await _dbContext.SalesOrders
                 .Include(po => po.Status) 
                 .Where(po => po.CustomerId == customerId)
                         .ToListAsync();
        }

        public async Task<List<Customer>> SearchAsync(string name, string city)
        {
            var query = _dbContext.Customers.Where(c => !c.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(c => c.City.Contains(city));
            }

            return await query.ToListAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var customer = await _dbContext.Customers.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
            customer.IsDeleted = true;
            // ModifiedDate and DeletedBy are now set automatically in DbContext.SaveChangesAsync
        }

        public async Task UpdateAsync(Customer customer)
        {
            _dbContext.Customers.Update(customer);
        }
    }
}
