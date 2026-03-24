using Application.DTOs.Auth;
using Application.DTOs.Product;
using Application.DTOs.PurchaseOrder;
using Application.DTOs.Supplier;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;

namespace Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public SupplierService(ISupplierRepository supplierRepository, IProductRepository productRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
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

        public async Task<PaginatedResult<SupplierDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedSuppliers = await _supplierRepository.GetAllAsync(@params);
            
            if (paginatedSuppliers.TotalCount <= 0)
            {
                throw new NotFoundException("No Suppliers found!!");
            }

            var dtos = _mapper.Map<List<SupplierDto>>(paginatedSuppliers.Items);
            
            return new PaginatedResult<SupplierDto>(dtos, paginatedSuppliers.TotalCount, @params.PageNumber, @params.PageSize);
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

        public async Task<List<ProductDto>> GetProductsBySupplierIdAsync(int supplierId)
        {
            if (supplierId <= 0)
                throw new BadRequestException("Id must be greater than 0");

            var supplier = await _supplierRepository.GetByIdAsync(supplierId);
            if (supplier == null)
                throw new NotFoundException($"Supplier with id:{supplierId} not found");

            var products = await _supplierRepository.GetProductsBySupplierIdAsync(supplierId);
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<List<PurchaseOrderDto>> GetPurchaseOrdersBySupplierId(int supplierId)
        {
           if(supplierId == 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var supplier = await _supplierRepository.GetByIdAsync(supplierId);
            if (supplier == null)
                throw new NotFoundException($"Supplier with id:{supplierId} not found");

            var purchaseorder = await _supplierRepository.GetPurchaseOrdersBySupplierIdAsync(supplierId);
            if(purchaseorder == null)
            {
                throw new NotFoundException($"No purchaseorder found for supplier id:{supplierId}");
            }
            return _mapper.Map<List<PurchaseOrderDto>>(purchaseorder);
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

            // Check if any products are linked to this supplier
            if (await _productRepository.ExistsBySupplierIdAsync(id))
            {
                throw new BadRequestException("Cannot delete — products are linked to this supplier");
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
                throw new NotFoundException($"Supplier with id:{id} not found");
            }

            if (!string.Equals(supplier.Name, dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var existingSupplier = await _supplierRepository.GetByNameAsync(dto.Name);
                if (existingSupplier != null)
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
