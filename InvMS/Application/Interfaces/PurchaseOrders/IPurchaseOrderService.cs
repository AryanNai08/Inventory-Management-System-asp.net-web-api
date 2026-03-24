using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.PurchaseOrder;
using Application.Common;

namespace Application.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PaginatedResult<PurchaseOrderDto>> GetAllAsync(PaginationParams @params);
        Task<PurchaseOrderDto> GetByIdAsync(int id);
        Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto dto);
        Task<PurchaseOrderDto> UpdateAsync(int id, UpdatePurchaseOrderDto dto);
        Task<bool> ApproveAsync(int id);
        Task<bool> ReceiveAsync(int id);
        Task<bool> CancelAsync(int id);
    }
}
