using Application.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        public Task<List<CustomerDto>> GetAllAsync();
        public Task<CustomerDto> GetByIdAsync(int id);
        Task<List<CustomerDto>> SearchAsync(string name, string city);
        public Task<CustomerDto> CreateAsync(CreateCustomerDto dto);

        public Task<bool> UpdateAsync(int id, UpdateCustomerDto dto);

        public Task<bool> SoftDeleteAsync(int id);
    }
}
