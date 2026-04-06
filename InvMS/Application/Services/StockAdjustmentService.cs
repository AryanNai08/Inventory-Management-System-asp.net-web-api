using Application.DTOs.StockAdjustment;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common;

namespace Application.Services
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IStockAdjustmentRepository _stockAdjustmentRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _productStockRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockAdjustmentService(
            IStockAdjustmentRepository stockAdjustmentRepository,
            IProductRepository productRepository,
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository productStockRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _stockAdjustmentRepository = stockAdjustmentRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _productStockRepository = productStockRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StockAdjustmentDto> CreateAsync(CreateStockAdjustmentDto dto, int userId)
        {
            if (dto.ProductId <= 0) throw new BadRequestException("Invalid Product Id");
            if (dto.WarehouseId <= 0) throw new BadRequestException("Invalid Warehouse Id");
            if (dto.QuantityChange == 0) throw new BadRequestException("Quantity change cannot be zero");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _productRepository.GetByIdAsync(dto.ProductId);
                if (product == null) throw new NotFoundException($"Product with Id {dto.ProductId} not found");

                var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
                if (warehouse == null) throw new NotFoundException($"Warehouse with Id {dto.WarehouseId} not found");

                // Get or Create ProductWarehouseStock
                var stock = await _productStockRepository.GetByProductAndWarehouseAsync(dto.ProductId, dto.WarehouseId);
                bool isNewStock = false;

                if (stock == null)
                {
                    stock = new ProductWarehouseStock
                    {
                        ProductId = dto.ProductId,
                        WarehouseId = dto.WarehouseId,
                        Quantity = 0
                    };
                    isNewStock = true;
                }

                // Business Rule: Check for negative stock
                int newStockQuantity = stock.Quantity + dto.QuantityChange;
                if (newStockQuantity < 0)
                {
                    throw new BadRequestException($"Insufficient stock in warehouse '{warehouse.Name}'. Current: {stock.Quantity}, Requested Adjustment: {dto.QuantityChange}");
                }

                // 1. Create adjustment record
                var adjustment = _mapper.Map<StockAdjustment>(dto);
                adjustment.AdjustedBy = userId;
                adjustment.AdjustmentDate = DateTime.UtcNow;
                await _stockAdjustmentRepository.AddAsync(adjustment);

                // 2. Update/Add product stock
                stock.Quantity = newStockQuantity;
                if (isNewStock)
                {
                    await _productStockRepository.AddAsync(stock);
                }
                else
                {
                    await _productStockRepository.UpdateAsync(stock);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                // Return the created adjustment with full details (includes names of product, warehouse etc.)
                return await GetByIdAsync(adjustment.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackAsync();
                throw new BadRequestException("The product stock was updated by another user. Please try again.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<List<AdjustmentTypeDto>> GetAdjustmentTypesAsync()
        {
            var adjustmettypes = await _stockAdjustmentRepository.GetAdjustmentTypesAsync();
            if(adjustmettypes.Count == 0)
            {
                    throw new NotFoundException("adjustmettypes not found");
            }
            return _mapper.Map<List<AdjustmentTypeDto>>(adjustmettypes);
        }

        public async Task<PaginatedResult<StockAdjustmentDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedAdjustments = await _stockAdjustmentRepository.GetAllAsync(@params);
            
            if (paginatedAdjustments.TotalCount == 0)
            {
                throw new NotFoundException("Stocks not found");
            }

            var dtos = _mapper.Map<List<StockAdjustmentDto>>(paginatedAdjustments.Items);
            
            return new PaginatedResult<StockAdjustmentDto>(dtos, paginatedAdjustments.TotalCount, @params.PageNumber, @params.PageSize);
        }

        public async Task<StockAdjustmentDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("stock Id must be greater than 0");
            var stock = await _stockAdjustmentRepository.GetByIdAsync(id);
            if (stock == null)
            {
                throw new NotFoundException($"No stocks found with id:{id}!");
            }

            return _mapper.Map<StockAdjustmentDto>(stock);
        }

        public async Task<List<StockAdjustmentDto>> GetByProductIdAsync(int productId)
        {
            if (productId <= 0)
                throw new BadRequestException("product Id must be greater than 0");
            var stock = await _stockAdjustmentRepository.GetByProductIdAsync(productId);
            if (stock==null)
            {
                throw new NotFoundException($"Stock with id:{productId} not found");
            }
            return _mapper.Map<List<StockAdjustmentDto>>(stock);
        }
    }
}
