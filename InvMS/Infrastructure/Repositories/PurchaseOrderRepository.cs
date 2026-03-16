using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly InventoryDbContext _dbContext;

        public PurchaseOrderRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(PurchaseOrder purchaseOrder)
        {
            await _dbContext.PurchaseOrders.AddAsync(purchaseOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseOrder purchaseOrder)
        {
            _dbContext.PurchaseOrders.Update(purchaseOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            // Format: PO-YYYYMMDD-0001
            string datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
            string prefix = $"PO-{datePrefix}-";

            // Find the last order number for today to increment the sequence
            var lastOrder = await _dbContext.PurchaseOrders
                .Where(p => p.OrderNumber.StartsWith(prefix))
                .OrderByDescending(p => p.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
            {
                // First order of the day
                return $"{prefix}0001";
            }

            // Extract the last 4 digits and increment
            string lastSequenceStr = lastOrder.OrderNumber.Substring(12); // "PO-yyyyMMdd-" is 12 chars
            if (int.TryParse(lastSequenceStr, out int lastSequence))
            {
                int nextSequence = lastSequence + 1;
                return $"{prefix}{nextSequence:D4}"; // Pads with leading zeros
            }

            return $"{prefix}0001"; // Fallback
        }

        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            return await _dbContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Status)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> GetByIdAsync(int id)
        {
            return await _dbContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Status)
                .Include(p => p.PurchaseOrderItems)
                    .ThenInclude(poi => poi.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateStatusAsync(int id, int newStatusId)
        {
            var order = await _dbContext.PurchaseOrders.FindAsync(id);
            if (order != null)
            {
                order.StatusId = newStatusId;
                order.ModifiedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
