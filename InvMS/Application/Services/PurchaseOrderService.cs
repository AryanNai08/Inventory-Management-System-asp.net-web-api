using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.PurchaseOrder;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Application.Common;

namespace Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepository,
            ISupplierRepository supplierRepository,
            IProductRepository productRepository,
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository productStockRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<PurchaseOrderDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedOrders = await _purchaseOrderRepository.GetAllAsync(@params);
            
            if (paginatedOrders.TotalCount <= 0)
            {
                throw new NotFoundException("No Purchaseorders found!!");
            }

            var dtos = _mapper.Map<List<PurchaseOrderDto>>(paginatedOrders.Items);
            
            return new PaginatedResult<PurchaseOrderDto>(dtos, paginatedOrders.TotalCount, @params.PageNumber, @params.PageSize);
        }

        public async Task<PurchaseOrderDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Purchase Order Id must be greater than 0");

            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Purchase Order with ID {id} not found");

            return _mapper.Map<PurchaseOrderDto>(order);
        }

        public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto dto)
        {
            // 1. Validate Supplier
            var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new NotFoundException($"Supplier with ID {dto.SupplierId} not found");

            // 1.1 Validate Warehouse
            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
                throw new NotFoundException($"Warehouse with ID {dto.WarehouseId} not found");

            // 2. Validate Products
            var productIds = new HashSet<int>();
            foreach (var item in dto.Items)
            {
                // Check for duplicate products in the same order
                if (!productIds.Add(item.ProductId))
                    throw new BadRequestException($"Duplicate Product ID {item.ProductId} found in the order items.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {item.ProductId} not found");

                // Validate Supplier
                if (product.SupplierId != dto.SupplierId)
                    throw new BadRequestException($"Product '{product.Name}' (ID: {product.Id}) is supplied by Supplier ID {product.SupplierId}, not the selected Supplier ID {dto.SupplierId}.");

                // Validate Unit Cost (Optional: strictly match catalog price)
                if (item.UnitCost != product.UnitPrice)
                    throw new BadRequestException($"Incorrect unit cost for product '{product.Name}'. Expected {product.UnitPrice}, but received {item.UnitCost}.");
            }

            // 3. Map to Entity
            var purchaseOrder = _mapper.Map<PurchaseOrder>(dto);

            // 4. Generate details
            purchaseOrder.WarehouseId = dto.WarehouseId;
            purchaseOrder.OrderDate = DateTime.UtcNow;
            purchaseOrder.CreatedDate = DateTime.UtcNow;
            purchaseOrder.StatusId = 1; // 1 = Draft
            purchaseOrder.TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitCost); // Calculate total

            // 5. Generate Order Number and Save with Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                purchaseOrder.OrderNumber = await _purchaseOrderRepository.GenerateOrderNumberAsync();
                await _purchaseOrderRepository.AddAsync(purchaseOrder);
                
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            // 6. Return created order
            return await GetByIdAsync(purchaseOrder.Id);
        }

        public async Task<PurchaseOrderDto> UpdateAsync(int id, UpdatePurchaseOrderDto dto)
        {
            // 1. Validate Order exists and is in Draft status
            var existingOrder = await _purchaseOrderRepository.GetByIdAsync(id);
            if (existingOrder == null)
                throw new NotFoundException($"Purchase Order with ID {id} not found");

            if (existingOrder.StatusId != 1) // Only Draft can be modified
                throw new BadRequestException("Only Draft orders can be updated");

            // 2. Validate Supplier
            var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new NotFoundException($"Supplier with ID {dto.SupplierId} not found");

            // 3. Validate Products
            var productIds = new HashSet<int>();
            foreach (var item in dto.Items)
            {
                // Check for duplicate products in the same order
                if (!productIds.Add(item.ProductId))
                    throw new BadRequestException($"Duplicate Product ID {item.ProductId} found in the order items.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product with ID {item.ProductId} not found");

                // Validate Supplier
                if (product.SupplierId != dto.SupplierId)
                    throw new BadRequestException($"Product '{product.Name}' (ID: {product.Id}) is supplied by Supplier ID {product.SupplierId}, not the selected Supplier ID {dto.SupplierId}.");

                // Validate Unit Cost (Optional: strictly match catalog price)
                if (item.UnitCost != product.UnitPrice)
                    throw new BadRequestException($"Incorrect unit cost for product '{product.Name}'. Expected {product.UnitPrice}, but received {item.UnitCost}.");
            }

            // 4. Update core order details
            existingOrder.SupplierId = dto.SupplierId;
            existingOrder.WarehouseId = dto.WarehouseId;
            existingOrder.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
            existingOrder.Notes = dto.Notes;
            existingOrder.ModifiedDate = DateTime.UtcNow;

            // 5. Update items 
            // - Clear existing items and re-add to simplify update logic since EF Core will manage the tracking.
            // In a production app with huge POs, we'd do a careful merge (check what to update, add, delete).
            existingOrder.PurchaseOrderItems.Clear();
            
            foreach (var itemDto in dto.Items)
            {
                existingOrder.PurchaseOrderItems.Add(new PurchaseOrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitCost = itemDto.UnitCost
                });
            }

            // 6. Recalculate Total
            existingOrder.TotalAmount = dto.Items.Sum(i => i.Quantity * i.UnitCost);

            // 7. Save with Transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _purchaseOrderRepository.UpdateAsync(existingOrder);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return await GetByIdAsync(id);
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Purchase Order {id} not found");
            
            if (order.StatusId != 1) // Only Draft can be Approved
                throw new BadRequestException("Only Draft orders can be approved");

            await _purchaseOrderRepository.UpdateStatusAsync(id, 2); // 2 = Approved
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReceiveAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Purchase Order {id} not found");

            if (order.StatusId != 2) // Only Approved can be Received
                throw new BadRequestException("Only Approved orders can be received");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Update ProductWarehouseStock for each item
                foreach (var item in order.PurchaseOrderItems)
                {
                    var stock = await _productStockRepository.GetByProductAndWarehouseAsync(item.ProductId, order.WarehouseId);
                    bool isNewStock = false;

                    if (stock == null)
                    {
                        stock = new ProductWarehouseStock
                        {
                            ProductId = item.ProductId,
                            WarehouseId = order.WarehouseId,
                            Quantity = 0
                        };
                        isNewStock = true;
                    }

                    stock.Quantity += item.Quantity;

                    if (isNewStock)
                    {
                        await _productStockRepository.AddAsync(stock);
                    }
                    else
                    {
                        await _productStockRepository.UpdateAsync(stock);
                    }
                }

                // Update Order Status
                await _purchaseOrderRepository.UpdateStatusAsync(id, 3); // 3 = Received

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

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException($"Purchase Order {id} not found");

            if (order.StatusId == 3) // Cannot cancel if already received
                throw new BadRequestException("Cannot cancel an order that has already been received");

            await _purchaseOrderRepository.UpdateStatusAsync(id, 4); // 4 = Cancelled
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
