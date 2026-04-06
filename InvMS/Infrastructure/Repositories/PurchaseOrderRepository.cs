using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Common;

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
        }

        public async Task UpdateAsync(PurchaseOrder purchaseOrder)
        {
            _dbContext.PurchaseOrders.Update(purchaseOrder);
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

        public async Task<PaginatedResult<PurchaseOrder>> GetAllAsync(PaginationParams @params)
        {
            var query = _dbContext.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Status)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("OrderNumber", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(p => p.OrderNumber) : query.OrderBy(p => p.OrderNumber);
                else if (@params.SortColumn.Equals("OrderDate", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(p => p.OrderDate) : query.OrderBy(p => p.OrderDate);
                else if (@params.SortColumn.Equals("TotalAmount", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(p => p.TotalAmount) : query.OrderBy(p => p.TotalAmount);
                else
                    query = query.OrderByDescending(p => p.CreatedDate);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedDate);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();

            return new PaginatedResult<PurchaseOrder>(items, count, @params.PageNumber, @params.PageSize);
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
                // ModifiedDate and UpdatedBy are handled by DbContext.SaveChangesAsync automatically
            }
        }
    }
}
