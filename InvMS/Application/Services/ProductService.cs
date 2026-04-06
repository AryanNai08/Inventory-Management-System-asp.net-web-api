using Application.Common;
using Application.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var existing = await _productRepository.GetBySkuAsync(dto.Sku);

            if (existing != null)
                throw new BadRequestException("SKU already exists");

            var newproduct = _mapper.Map<Product>(dto);
            newproduct.CreatedDate = DateTime.UtcNow;
            await _productRepository.AddAsync(newproduct);
            await _unitOfWork.SaveChangesAsync();
                
            // Reload with navigation properties so CategoryName, SupplierName, WarehouseName are populated
            var createdProduct = await _productRepository.GetByIdAsync(newproduct.Id);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<PaginatedResult<ProductDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedProducts = await _productRepository.GetAllAsync(@params);
            
            if (paginatedProducts.TotalCount <= 0) 
            {
                throw new NotFoundException("No Products found!!");
            }

            var dtos = _mapper.Map<List<ProductDto>>(paginatedProducts.Items);
            
            return new PaginatedResult<ProductDto>(dtos, paginatedProducts.TotalCount, @params.PageNumber, @params.PageSize);
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }
            var product=await _productRepository.GetByIdAsync(id);
            if(product == null)
            {
                throw new NotFoundException($"Product with id:{id} not found");
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> GetBySkuAsync(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new BadRequestException("SKU cannot be empty");

            var product = await _productRepository.GetBySkuAsync(sku);
            if (product == null)
                throw new NotFoundException($"Product with SKU:{sku} not found");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductDto>> SearchAsync(string name, int? categoryId, int? supplierId)
        {
            var products = await _productRepository.SearchAsync(name, categoryId, supplierId);
            
            if (products.Count == 0)
                throw new NotFoundException("No products matched the search criteria");

            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException($"Product with id:{id} not found");
            }
            await _productRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            // ... validation ...

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new NotFoundException($"Product with id:{id} not found");
            // Check SKU uniqueness if changed
            if (!string.Equals(product.Sku, dto.Sku, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _productRepository.GetBySkuAsync(dto.Sku);
                if (existing != null) throw new BadRequestException("SKU already exists");
            }
            // Set the RowVersion from the client for concurrency check
            product.RowVersion = dto.RowVersion;
            _mapper.Map(dto, product);
            product.ModifiedDate = DateTime.UtcNow;
            try
            {
                await _productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new BadRequestException("This record was modified by another user. Please refresh and try again.");
            }
            return true;
        }

        public async Task<List<ProductDto>> GetLowStockProducts()
        {
            var products = await _productRepository.GetLowStockAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<List<ProductDto>> GetOutOfStockProducts()
        {
            var products = await _productRepository.GetOutOfStockAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<bool> PatchAsync(int id, JsonPatchDocument<UpdateProductDto> patchDoc)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new NotFoundException($"Product {id} not found");

            // 1. Map Entity to DTO
            var productToPatch = _mapper.Map<UpdateProductDto>(product);

            // 2. Apply Patch to DTO
            patchDoc.ApplyTo(productToPatch);

            // 3. Map Patched DTO back to Entity
            _mapper.Map(productToPatch, product);
            product.ModifiedDate = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
