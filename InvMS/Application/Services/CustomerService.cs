using Application.Common;
using Application.DTOs.Customer;
using Application.DTOs.SalesOrder;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var customer = await _customerRepository.GetByNameAsync(dto.Name);

            if (customer != null)
            {
                throw new BadRequestException("Customer name already exists");
            }

            var newcustomer = _mapper.Map<Customer>(dto);
            newcustomer.CreatedDate = DateTime.UtcNow;
            await _customerRepository.AddAsync(newcustomer);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CustomerDto>(newcustomer);
        }

        public async Task<PaginatedResult<CustomerDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedCustomers = await _customerRepository.GetAllAsync(@params);
 
            if (paginatedCustomers.TotalCount <= 0)
            {
                throw new NotFoundException("No Customers found!!");
            }
 
            var dtos = _mapper.Map<List<CustomerDto>>(paginatedCustomers.Items);
            
            return new PaginatedResult<CustomerDto>(dtos, paginatedCustomers.TotalCount, @params.PageNumber, @params.PageSize);
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with id:{id} not found");
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<List<SalesOrderDto>> GetSalesOrdersByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with id:{customerId} not found");
            }

            var salesorder=await _customerRepository.GetSalesOrdersByCustomerIdAsync(customerId);
            if(salesorder == null)
            {
                throw new NotFoundException($"salesorder with customer id:{customerId} not found");
            }
            return _mapper.Map<List<SalesOrderDto>>(salesorder);
        }

        public async Task<List<CustomerDto>> SearchAsync(string name, string city)
        {
            var customers = await _customerRepository.SearchAsync(name, city);

            if (customers.Count == 0)
                throw new NotFoundException("No customers matched the search criteria");

            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with id:{id} not found");
            }
            await _customerRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with id:{id} not found");
            }

            if (!string.Equals(customer.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingcustomer = await _customerRepository.GetByNameAsync(dto.Name);
                if (existingcustomer != null)
                {
                    throw new BadRequestException("Customer name already exists");
                }
            }

            _mapper.Map(dto, customer);
            customer.ModifiedDate = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
