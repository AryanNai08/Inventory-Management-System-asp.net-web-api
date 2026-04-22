using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SalesOrder;
using Domain.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Application.Interfaces.SalesOrder;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SalesOrderService> _logger;

        public SalesOrderService(
            ISalesOrderRepository salesOrderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository productStockRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SalesOrderService> logger)
        {
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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

            // 2. Validate Warehouse
            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                throw new NotFoundException($"Warehouse with ID {dto.WarehouseId} not found");

            // 3. Validate Products and Stock
            var productIds = new HashSet<int>();
            foreach (var item in dto.Items)
            {
                if (!productIds.Add(item.ProductId))
                    throw new BadRequestException($"Duplicate Product ID {item.ProductId} found in the order items.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {item.ProductId} not found");

                if (item.UnitPrice <= 0)
                    throw new BadRequestException($"Unit price for product '{product.Name}' must be greater than zero.");
                
                if (product.PurchasePrice.HasValue && item.UnitPrice < product.PurchasePrice.Value)
                {
                    _logger.LogWarning("CRITICAL: Below-cost sale detected: ProductId={ProductId}, Cost={Cost}, Price={Price}", 
                        product.Id, product.PurchasePrice, item.UnitPrice);
                }
                else if (product.SalePrice.HasValue && item.UnitPrice < product.SalePrice.Value)
                {
                    _logger.LogInformation("INFO: Discount applied: ProductId={ProductId}, Catalog={Catalog}, Price={Price}", 
                        product.Id, product.SalePrice, item.UnitPrice);
                }
                
                var stock = await _productStockRepository.GetByProductAndWarehouseAsync(item.ProductId, dto.WarehouseId);
                int availableStock = stock?.Quantity ?? 0;

                if (availableStock < item.Quantity)
                {
                    throw new BadRequestException($"Insufficient stock for '{product.Name}' in warehouse '{warehouse.Name}'. Available: {availableStock}, Requested: {item.Quantity}");
                }
            }

            // 4. Generate Order Number and Save with Retry Logic
            int retries = 3;
            while (retries-- > 0)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // [FIX] Move entity creation inside the loop to avoid EF tracking issues on retry
                    var salesOrder = _mapper.Map<SalesOrder>(dto);
                    salesOrder.OrderDate = DateTime.UtcNow;
                    salesOrder.CreatedDate = DateTime.UtcNow;
                    salesOrder.StatusId = 1; // 1 = Pending
                    salesOrder.TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitPrice);

                    salesOrder.OrderNumber = await _salesOrderRepository.GenerateOrderNumberAsync();
                    await _salesOrderRepository.AddAsync(salesOrder);
                    
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    
                    // Success! Return directly with warnings
                    return await GetByIdWithWarningsAsync(salesOrder.Id);
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    await _unitOfWork.RollbackAsync();
                    
                    if (retries == 0)
                    {
                        throw new BadRequestException("Failed to generate a unique order number after multiple attempts. Please try again.");
                    }
                    
                    // Small delay before retrying
                    await Task.Delay(100); 
                    continue;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }

            throw new BadRequestException("An unexpected error occurred during order creation.");
        }

        private bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                // 2627 = Unique constraint violation, 2601 = Duplicate key index
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;
            }
            return false;
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

            // 3. Validate Warehouse
            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                throw new NotFoundException($"Warehouse with ID {dto.WarehouseId} not found");

            // 4. Validate Products and Stock
            var productIds = new HashSet<int>();
            foreach (var item in dto.Items)
            {
                if (!productIds.Add(item.ProductId))
                    throw new BadRequestException($"Duplicate Product ID {item.ProductId} found in the order items.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {item.ProductId} not found");

                if (item.UnitPrice <= 0)
                    throw new BadRequestException($"Unit price for product '{product.Name}' must be greater than zero.");

                if (product.PurchasePrice.HasValue && item.UnitPrice < product.PurchasePrice.Value)
                {
                    _logger.LogWarning("CRITICAL: Below-cost sale detected during update: ProductId={ProductId}, Cost={Cost}, Price={Price}",
                        product.Id, product.PurchasePrice, item.UnitPrice);
                }
                else if (product.SalePrice.HasValue && item.UnitPrice < product.SalePrice.Value)
                {
                    _logger.LogInformation("INFO: Discount applied during update: ProductId={ProductId}, Catalog={Catalog}, Price={Price}",
                        product.Id, product.SalePrice, item.UnitPrice);
                }
                
                var stock = await _productStockRepository.GetByProductAndWarehouseAsync(item.ProductId, dto.WarehouseId);
                int availableStock = stock?.Quantity ?? 0;

                if (availableStock < item.Quantity)
                {
                    throw new BadRequestException($"Insufficient stock for '{product.Name}' in warehouse '{warehouse.Name}'. Available: {availableStock}, Requested: {item.Quantity}");
                }
            }

            // 5. Update core order details
            existingOrder.CustomerId = dto.CustomerId;
            existingOrder.WarehouseId = dto.WarehouseId;
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

            // 7. Save with Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _salesOrderRepository.UpdateAsync(existingOrder);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return await GetByIdWithWarningsAsync(id);
        }

        public async Task<SalesOrderDto> GetByIdWithWarningsAsync(int id)
        {
            var result = await GetByIdAsync(id);
            if (result == null) return null;

            foreach (var item in result.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null) continue;

                // 1. Check for Below Cost (Critical)
                if (product.PurchasePrice.HasValue && item.UnitPrice < product.PurchasePrice.Value)
                {
                    result.Warnings.Add($"CRITICAL: '{product.Name}' is being sold BELOW COST (Cost: {product.PurchasePrice:C}, Price: {item.UnitPrice:C})");
                }
                // 2. Check for Discount (Informational)
                else if (product.SalePrice.HasValue && item.UnitPrice < product.SalePrice.Value)
                {
                    decimal discount = product.SalePrice.Value - item.UnitPrice;
                    result.Warnings.Add($"INFO: '{product.Name}' is discounted by {discount:C} (Catalog: {product.SalePrice:C}, Price: {item.UnitPrice:C})");
                }
            }

            return result;
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
                // Deduct stock for each item from the specific warehouse
                foreach (var item in order.SalesOrderItems)
                {
                    // Concurrency Handling: Re-fetch stock inside transaction
                    var stock = await _productStockRepository.GetByProductAndWarehouseAsync(item.ProductId, order.WarehouseId);
                    
                    if (stock == null || stock.Quantity < item.Quantity)
                    {
                        var product = await _productRepository.GetByIdAsync(item.ProductId);
                        throw new BadRequestException($"Insufficient stock for product '{product?.Name ?? "Unknown"}'. Available: {stock?.Quantity ?? 0}, Needed: {item.Quantity}. Stock may have changed, please retry.");
                    }

                    stock.Quantity -= item.Quantity;
                    await _productStockRepository.UpdateAsync(stock);
                }

                await _salesOrderRepository.UpdateStatusAsync(id, 2); // 2 = Confirmed

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackAsync();
                throw new BadRequestException("A concurrency error occurred while updating stock. Please try again.");
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
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeliverAsync(int id)
        {
            var order = await _salesOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Sales Order {id} not found");

            if (order.StatusId != 3) 
                throw new BadRequestException("Only Shipped orders can be delivered");

            await _salesOrderRepository.UpdateStatusAsync(id, 4); // 4 = Delivered
            await _unitOfWork.SaveChangesAsync();
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
                // If the order was already confirmed or shipped, we must RESTORE the stock
                if (order.StatusId == 2 || order.StatusId == 3) // 2 = Confirmed, 3 = Shipped
                {
                    foreach (var item in order.SalesOrderItems)
                    {
                        var stock = await _productStockRepository.GetByProductAndWarehouseAsync(item.ProductId, order.WarehouseId);
                        if (stock != null)
                        {
                            stock.Quantity += item.Quantity;
                            await _productStockRepository.UpdateAsync(stock);
                        }
                    }
                }

                await _salesOrderRepository.UpdateStatusAsync(id, 5); // 5 = Cancelled
                
                await _unitOfWork.SaveChangesAsync();
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

