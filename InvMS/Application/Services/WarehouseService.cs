using Application.DTOs.Warehouse;
using Domain.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces;
using Application.Interfaces.Warehouse;

namespace Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductWarehouseStockRepository _stockRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(
            IWarehouseRepository warehouseRepository, 
            IProductRepository productRepository, 
            IProductWarehouseStockRepository stockRepository,
            IMapper mapper, 
            IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _productRepository = productRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
        {
            var warehouse = await _warehouseRepository.GetByNameAsync(dto.Name);

            if (warehouse != null)
            {
                throw new BadRequestException("Warehouse name already exists");
            }

            var newwarehouse = _mapper.Map<Warehouse>(dto);
            newwarehouse.CreatedDate = DateTime.UtcNow;
            await _warehouseRepository.AddAsync(newwarehouse);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WarehouseDto>(newwarehouse);
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();

            if (warehouses.Count <= 0)
            {
                throw new NotFoundException("No Warehouses found!!");
            }

            return _mapper.Map<List<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(id);

            if (warehouse == null)
            {
                throw new NotFoundException($"Warehouse with id:{id} not found");
            }

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(id);

            if (warehouse == null)
            {
                throw new NotFoundException($"Warehouse with id:{id} not found");
            }

            // Check if any products are linked to this warehouse
            if (await _productRepository.ExistsByWarehouseIdAsync(id))
            {
                throw new BadRequestException("Cannot delete — products are linked to this warehouse");
            }

            await _warehouseRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }

        public async Task<bool> UpdateAsync(int id, UpdateWarehouseDto dto)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var warehouse = await _warehouseRepository.GetByIdAsync(id);

            if (warehouse == null)
            {
                throw new NotFoundException($"Warehouse with id:{id} not found");
            }

            if (!string.Equals(warehouse.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingwarehouse = await _warehouseRepository.GetByNameAsync(dto.Name);
                if (existingwarehouse != null)
                {
                    throw new BadRequestException("Warehouse name already exists");
                }
            }

            _mapper.Map(dto, warehouse);
            warehouse.ModifiedDate = DateTime.UtcNow;

            await _warehouseRepository.UpdateAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<WarehouseStockDto>> GetWarehouseStockAsync(int warehouseId)
        {
            var stocks = await _stockRepository.GetByWarehouseAsync(warehouseId);
            return stocks.Select(s => new WarehouseStockDto
            {
                ProductId = s.ProductId,
                ProductName = s.Product?.Name ?? "Unknown",
                ProductSku = s.Product?.Sku ?? "N/A",
                Quantity = s.Quantity
            }).ToList();
        }
    }
}
