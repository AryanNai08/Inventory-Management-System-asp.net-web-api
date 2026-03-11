using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Customer> GetByNameAsync(string name)
        {
            return await _dbContext.Customers.Where(c => c.Name == name && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var customer = await _dbContext.Customers.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
            if (customer == null)
                throw new NotFoundException($"Customer with id:{id} not found");
            customer.IsDeleted = true;
            customer.ModifiedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
