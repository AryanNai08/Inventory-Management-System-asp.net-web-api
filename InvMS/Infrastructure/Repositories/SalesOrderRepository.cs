using Application.DTOs.SalesOrder;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace Infrastructure.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly InventoryDbContext _dbContext;

        public SalesOrderRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SalesOrder salesOrder)
        {
            await _dbContext.SalesOrders.AddAsync(salesOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            // Format: SO-YYYYMMDD-0001
            string datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
            string prefix = $"SO-{datePrefix}-";

            // Find the last order number for today to increment the sequence
            var lastOrder = await _dbContext.SalesOrders
                .Where(s => s.OrderNumber.StartsWith(prefix))
                .OrderByDescending(s => s.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
            {
                // First order of the day
                return $"{prefix}0001";
            }

            // Extract the last 4 digits and increment
            string lastSequenceStr = lastOrder.OrderNumber.Substring(12); // "SO-yyyyMMdd-" is 12 chars
            if (int.TryParse(lastSequenceStr, out int lastSequence))
            {
                int nextSequence = lastSequence + 1;
                return $"{prefix}{nextSequence:D4}"; // Pads with leading zeros
            }

            return $"{prefix}0001"; // Fallback
        }

        public async Task<List<SalesOrder>> GetAllAsync()
        {
            return await _dbContext.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }

        public async Task<SalesOrder> GetByIdAsync(int id)
        {
            return await _dbContext.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .Include(s => s.SalesOrderItems)
                .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(SalesOrder salesOrder)
        {
            _dbContext.Update(salesOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, int newStatusId)
        {
            var order = await _dbContext.SalesOrders.FindAsync(id);
            if (order != null)
            {
                order.StatusId = newStatusId;
                order.ModifiedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
