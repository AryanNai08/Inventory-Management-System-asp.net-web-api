using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SalesOrder;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Application.Common;

namespace Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalesOrderService(
            ISalesOrderRepository salesOrderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<SalesOrderDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedOrders = await _salesOrderRepository.GetAllAsync(@params);
            
            if (paginatedOrders.TotalCount <= 0)
                throw new NotFoundException("No Sales Orders found!");
 
            var dtos = _mapper.Map<List<SalesOrderDto>>(paginatedOrders.Items);
            
            return new PaginatedResult<SalesOrderDto>(dtos, paginatedOrders.TotalCount, @params.PageNumber, @params.PageSize);
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
                
                // Optional logic: Check if enough stock IS AVAILABLE at draft time? (Basic check)
                if (product.CurrentStock < item.Quantity)
                {
                    throw new BadRequestException($"Insufficient stock for '{product.Name}'. Available: {product.CurrentStock}, Requested: {item.Quantity}");
                }
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
                
                if (product.CurrentStock < item.Quantity)
                {
                    throw new BadRequestException($"Insufficient stock for '{product.Name}'. Available: {product.CurrentStock}, Requested: {item.Quantity}");
                }
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

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Deduct stock for each item
                foreach (var item in order.SalesOrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null) throw new NotFoundException($"Product {item.ProductId} not found");

                    if (product.CurrentStock < item.Quantity)
                    {
                        throw new BadRequestException($"Insufficient stock for product '{product.Name}'. Available: {product.CurrentStock}, Needed: {item.Quantity}");
                    }

                    product.CurrentStock -= item.Quantity;
                    product.ModifiedDate = DateTime.UtcNow;
                    await _productRepository.UpdateAsync(product);
                }

                await _salesOrderRepository.UpdateStatusAsync(id, 2); // 2 = Confirmed

                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackAsync();
                throw new BadRequestException("A concurrency error occurred while updating product stock. Please try again.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ShipAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId != 2) 
                throw new BadRequestException("Only Confirmed orders can be shipped");

            await _salesOrderRepository.UpdateStatusAsync(id, 3); // 3 = Shipped
            return true;
        }

        public async Task<bool> DeliverAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId != 3) 
                throw new BadRequestException("Only Shipped orders can be delivered");

            await _salesOrderRepository.UpdateStatusAsync(id, 4); // 4 = Delivered
            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId == 4) 
                throw new BadRequestException("Cannot cancel an order that has already been delivered");
            
            if (order.StatusId == 5)
                throw new BadRequestException("Order is already cancelled");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // If the order was already confirmed, we must RESTORE the stock
                if (order.StatusId >= 2 && order.StatusId <= 3) // Confirmed or Shipped
                {
                    foreach (var item in order.SalesOrderItems)
                    {
                        var product = await _productRepository.GetByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            product.CurrentStock += item.Quantity;
                            product.ModifiedDate = DateTime.UtcNow;
                            await _productRepository.UpdateAsync(product);
                        }
                    }
                }

                await _salesOrderRepository.UpdateStatusAsync(id, 5); // 5 = Cancelled
                
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SalesOrderDto>> SearchAsync(int? statusId, int? customerId, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            var orders = await _salesOrderRepository.SearchAsync(statusId, customerId, startDate, endDate);
            return _mapper.Map<List<SalesOrderDto>>(orders);
        }
    }
}

