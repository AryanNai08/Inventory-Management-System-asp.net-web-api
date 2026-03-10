using Application.DTOs.Auth;
using Application.DTOs.Supplier;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;
        public SupplierService(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }
        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            var supplier = await _supplierRepository.GetByNameAsync(dto.Name);

            if (supplier != null)
            {
                throw new BadRequestException("Supplier name already exists");
            }

            var newsupplier = _mapper.Map<Supplier>(dto);
            newsupplier.CreatedDate = DateTime.UtcNow;
            await _supplierRepository.AddAsync(newsupplier);
            return _mapper.Map<SupplierDto>(newsupplier);
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();

            if (suppliers.Count <= 0)
            {
                throw new NotFoundException("No Suppliers found!!");
            }

            return _mapper.Map<List<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                throw new NotFoundException($"Supplier with id:{id} not found");
            }

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                throw new NotFoundException($"Supplier with id:{id} not found");
            }
            await _supplierRepository.SoftDeleteAsync(id);
            return true;

        }

        public async Task<bool> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                throw new NotFoundException($"supplier with id:{id} not found");
            }

            if (!string.Equals(supplier.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingCategory = await _supplierRepository.GetByNameAsync(dto.Name);
                if (existingCategory != null)
                {
                    throw new BadRequestException("Supplier name already exists");
                }
            }

            _mapper.Map(dto, supplier);
            supplier.ModifiedDate = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);
            return true;
        }
    }
}
