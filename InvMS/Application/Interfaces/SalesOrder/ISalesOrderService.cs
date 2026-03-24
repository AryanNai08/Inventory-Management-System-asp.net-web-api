using Application.DTOs.SalesOrder;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;

namespace Application.Interfaces
{
    public interface ISalesOrderService
    {
        Task<PaginatedResult<SalesOrderDto>> GetAllAsync(PaginationParams @params);
        Task<SalesOrderDto> GetByIdAsync(int id);
        Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto dto);
        Task<SalesOrderDto> UpdateAsync(int id, UpdateSalesOrderDto dto);

        Task<bool> ConfirmAsync(int id);
        Task<bool> ShipAsync(int id);
        Task<bool> DeliverAsync(int id);
        Task<bool> CancelAsync(int id);
        Task<List<SalesOrderDto>> SearchAsync(int? statusId, int? customerId, DateTime? startDate, DateTime? endDate);
    }
}
