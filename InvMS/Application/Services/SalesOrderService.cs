using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SalesOrder;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public SalesOrderService(
            ISalesOrderRepository salesOrderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<SalesOrderDto>> GetAllAsync()
        {
            var orders = await _salesOrderRepository.GetAllAsync();
            if (orders.Count == 0)
                throw new NotFoundException("No Sales Orders found!");

            return _mapper.Map<List<SalesOrderDto>>(orders);
        }

        public async Task<SalesOrderDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Sales Order Id must be greater than 0");

            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Sales Order with ID {id} not found");

            return _mapper.Map<SalesOrderDto>(order);
        }

        public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto dto)
        {
            // 1. Validate Customer
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new NotFoundException($"Customer with ID {dto.CustomerId} not found");

            // 2. Validate Products
            var productIds = new HashSet<int>();
            foreach (var item in dto.Items)
            {
                // Check for duplicate products
                if (!productIds.Add(item.ProductId))
                    throw new BadRequestException($"Duplicate Product ID {item.ProductId} found in the order items.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {item.ProductId} not found");

                // Validate Unit Price matches catalog
                if (item.UnitPrice != product.UnitPrice)
                    throw new BadRequestException($"Incorrect unit price for product '{product.Name}'. Expected {product.UnitPrice}, but received {item.UnitPrice}.");
            }

            // 3. Map to Entity
            var salesOrder = _mapper.Map<SalesOrder>(dto);

            // 4. Generate details
            salesOrder.OrderNumber = await _salesOrderRepository.GenerateOrderNumberAsync();
            salesOrder.OrderDate = DateTime.UtcNow;
            salesOrder.CreatedDate = DateTime.UtcNow;
            salesOrder.StatusId = 1; // 1 = Pending
            salesOrder.TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice);

            // 5. Save
            await _salesOrderRepository.AddAsync(salesOrder);

            // 6. Return created order
            return await GetByIdAsync(salesOrder.Id);
        }

        public async Task<SalesOrderDto> UpdateAsync(int id, UpdateSalesOrderDto dto)
        {
            // 1. Validate Order exists and is in Pending status
            var existingOrder = await _salesOrderRepository.GetByIdAsync(id);
            if (existingOrder == null)
                throw new NotFoundException($"Sales Order with ID {id} not found");

            if (existingOrder.StatusId != 1) // Only Pending can be modified
                throw new BadRequestException("Only Pending orders can be updated");

            // 2. Validate Customer
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
                throw new NotFoundException($"Customer with ID {dto.CustomerId} not found");

            // 3. Validate Products
            var productIds = new HashSet<int>();
            foreach (var item in dto.Items)
            {
                if (!productIds.Add(item.ProductId))
                    throw new BadRequestException($"Duplicate Product ID {item.ProductId} found in the order items.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {item.ProductId} not found");

                if (item.UnitPrice != product.UnitPrice)
                    throw new BadRequestException($"Incorrect unit price for product '{product.Name}'. Expected {product.UnitPrice}, but received {item.UnitPrice}.");
            }

            // 4. Update core order details
            existingOrder.CustomerId = dto.CustomerId;
            existingOrder.Notes = dto.Notes;
            existingOrder.ModifiedDate = DateTime.UtcNow;

            // 5. Update items
            existingOrder.SalesOrderItems.Clear();

            foreach (var itemDto in dto.Items)
            {
                existingOrder.SalesOrderItems.Add(new SalesOrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                });
            }

            // 6. Recalculate Total
            existingOrder.TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice);

            // 7. Save
            await _salesOrderRepository.UpdateAsync(existingOrder);

            return await GetByIdAsync(id);
        }

        public async Task<bool> ConfirmAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId != 1) 
                throw new BadRequestException("Only Pending orders can be confirmed");

            await _salesOrderRepository.UpdateStatusAsync(id, 2);
            return true;
        }

        public async Task<bool> ShipAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId != 2) 
                throw new BadRequestException("Only Confirmed orders can be shipped");

            await _salesOrderRepository.UpdateStatusAsync(id, 3); 
            return true;
        }

        public async Task<bool> DeliverAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId != 3) 
                throw new BadRequestException("Only Shipped orders can be delivered");

            await _salesOrderRepository.UpdateStatusAsync(id, 4); 
            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId == 4) 
                throw new BadRequestException("Cannot cancel an order that has already been delivered");


            await _salesOrderRepository.UpdateStatusAsync(id, 5); 
            return true;
        }
    }
}

