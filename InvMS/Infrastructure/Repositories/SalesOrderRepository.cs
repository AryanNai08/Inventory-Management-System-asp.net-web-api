using Domain.Common;
using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PaginatedResult<SalesOrder>> GetAllAsync(PaginationParams @params)
        {
            var query = _dbContext.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .Include(s => s.Warehouse)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("OrderNumber", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(s => s.OrderNumber) : query.OrderBy(s => s.OrderNumber);
                else if (@params.SortColumn.Equals("OrderDate", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(s => s.OrderDate) : query.OrderBy(s => s.OrderDate);
                else if (@params.SortColumn.Equals("TotalAmount", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount);
                else
                    query = query.OrderByDescending(s => s.CreatedDate);
            }
            else
            {
                query = query.OrderByDescending(s => s.CreatedDate);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();

            return new PaginatedResult<SalesOrder>(items, count, @params.PageNumber, @params.PageSize);
        }

        public async Task<SalesOrder> GetByIdAsync(int id)
        {
            return await _dbContext.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .Include(s => s.Warehouse)
                .Include(s => s.SalesOrderItems)
                .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(SalesOrder salesOrder)
        {
            _dbContext.Update(salesOrder);
        }

        public async Task UpdateStatusAsync(int id, int newStatusId)
        {
            var order = await _dbContext.SalesOrders.FindAsync(id);
            if (order != null)
            {
                order.StatusId = newStatusId;
                // ModifiedDate and UpdatedBy are handled by DbContext.SaveChangesAsync automatically
            }
        }

        public async Task<List<SalesOrder>> SearchAsync(int? statusId, int? customerId, DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .Include(s => s.Warehouse)
                .AsQueryable();

            if (statusId.HasValue)
                query = query.Where(s => s.StatusId == statusId);

            if (customerId.HasValue)
                query = query.Where(s => s.CustomerId == customerId);

            if (startDate.HasValue)
                query = query.Where(s => s.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.OrderDate <= endDate.Value);

            return await query
                .OrderByDescending(s => s.OrderDate)
                .ToListAsync();
        }
    }
}
