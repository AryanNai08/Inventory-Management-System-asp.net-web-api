using Application.DTOs.Customer;
using Application.DTOs.SalesOrder;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        public Task<PaginatedResult<CustomerDto>> GetAllAsync(PaginationParams @params);
        public Task<CustomerDto> GetByIdAsync(int id);
        Task<List<CustomerDto>> SearchAsync(string name, string city);
        public Task<CustomerDto> CreateAsync(CreateCustomerDto dto);

        public Task<bool> UpdateAsync(int id, UpdateCustomerDto dto);

        public Task<bool> SoftDeleteAsync(int id);

        public Task<List<SalesOrderDto>>GetSalesOrdersByCustomerIdAsync(int customerId);
    }
}
