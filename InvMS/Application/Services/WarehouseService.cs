using Application.DTOs.Warehouse;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;
        public WarehouseService(IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _warehouseRepository= warehouseRepository;
            _mapper = mapper;
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
            return _mapper.Map<WarehouseDto>(newwarehouse);
        }

        public async Task<List<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();

            if (warehouses.Count <= 0)
            {
                throw new NotFoundException("No warehouse found!!");
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
                throw new NotFoundException($"warehouse with id:{id} not found");
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
                throw new NotFoundException($"warehouse with id:{id} not found");
            }
            await _warehouseRepository.SoftDeleteAsync(id);
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
                throw new NotFoundException($"warehouse with id:{id} not found");
            }

            if (!string.Equals(warehouse.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingwarehouse = await _warehouseRepository.GetByNameAsync(dto.Name);
                if (existingwarehouse != null)
                {
                    throw new BadRequestException("warehouse name already exists");
                }
            }

            _mapper.Map(dto, warehouse);
            warehouse.ModifiedDate = DateTime.UtcNow;

            await _warehouseRepository.UpdateAsync(warehouse);
            return true;
        }
    }
}
