using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<List<SalesOrder>> GetAllAsync();
        Task<SalesOrder> GetByIdAsync(int id);
        Task<string> GenerateOrderNumberAsync();
        Task AddAsync(SalesOrder salesOrder);
        Task UpdateAsync(SalesOrder salesOrder);
        Task UpdateStatusAsync(int id, int newStatusId);
    }
}
