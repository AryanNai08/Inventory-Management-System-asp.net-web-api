using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Common;

namespace Application.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PaginatedResult<PurchaseOrder>> GetAllAsync(PaginationParams @params);
        Task<PurchaseOrder> GetByIdAsync(int id);
        Task<string> GenerateOrderNumberAsync();
        Task AddAsync(PurchaseOrder purchaseOrder);
        Task UpdateAsync(PurchaseOrder purchaseOrder);
        Task UpdateStatusAsync(int id, int newStatusId);
    }
}
