using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Common;

namespace Domain.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<PaginatedResult<SalesOrder>> GetAllAsync(PaginationParams @params);
        Task<SalesOrder> GetByIdAsync(int id);
        Task<string> GenerateOrderNumberAsync();
        Task AddAsync(SalesOrder salesOrder);
        Task UpdateAsync(SalesOrder salesOrder);
        Task UpdateStatusAsync(int id, int newStatusId);
        Task<List<SalesOrder>> SearchAsync(int? statusId, int? customerId, DateTime? startDate, DateTime? endDate);
    }
}
