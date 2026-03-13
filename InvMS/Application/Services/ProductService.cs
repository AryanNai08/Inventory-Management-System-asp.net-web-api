using Application.DTOs.Product;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository,IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var existing = await _productRepository.GetBySkuAsync(dto.Sku);

            if (existing != null)
                throw new BadRequestException("SKU already exists");

            var newproduct = _mapper.Map<Product>(dto);
            newproduct.CreatedDate = DateTime.UtcNow;
            await _productRepository.AddAsync(newproduct);
                
            // Reload with navigation properties so CategoryName, SupplierName, WarehouseName are populated
            var createdProduct = await _productRepository.GetByIdAsync(newproduct.Id);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
           var products= await _productRepository.GetAllAsync();
            if (products.Count <= 0) 
            {
                throw new NotFoundException("No Products found!!");
            }

            return _mapper.Map<List<ProductDto>>(products);
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
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new BadRequestException("This record was modified by another user. Please refresh and try again.");
            }
            return true;
        }
    }
}
