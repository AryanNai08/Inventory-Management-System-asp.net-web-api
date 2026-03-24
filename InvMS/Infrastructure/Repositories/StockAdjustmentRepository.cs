using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;

namespace Infrastructure.Repositories
{
    public class StockAdjustmentRepository : IStockAdjustmentRepository
    {
        private readonly InventoryDbContext _dbContext;
        public StockAdjustmentRepository(InventoryDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(StockAdjustment adjustment)
        {
            await _dbContext.StockAdjustments.AddAsync(adjustment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<AdjustmentType>> GetAdjustmentTypesAsync()
        {
            return await _dbContext.AdjustmentTypes.ToListAsync();
        }

        public async Task<PaginatedResult<StockAdjustment>> GetAllAsync(PaginationParams @params)
        {
            var query = _dbContext.StockAdjustments
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.AdjustmentType)
                .Include(s => s.User)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("AdjustmentDate", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(s => s.AdjustmentDate) : query.OrderBy(s => s.AdjustmentDate);
                else
                    query = query.OrderByDescending(s => s.AdjustmentDate);
            }
            else
            {
                query = query.OrderByDescending(s => s.AdjustmentDate);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();

            return new PaginatedResult<StockAdjustment>(items, count, @params.PageNumber, @params.PageSize);
        }

        public async Task<StockAdjustment> GetByIdAsync(int id)
        {
            return await _dbContext.StockAdjustments
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.AdjustmentType)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s=>s.Id==id);
        }

        public async Task<List<StockAdjustment>> GetByProductIdAsync(int productId)
        {
                 return await _dbContext.StockAdjustments
                .Where(s => s.ProductId == productId)
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Include(s => s.AdjustmentType)
                .Include(s => s.User)
                .OrderByDescending(s => s.AdjustmentDate)
                .ToListAsync();
        }
    }
}
