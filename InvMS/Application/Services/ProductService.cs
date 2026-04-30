using Domain.Common;
using Application.DTOs.Product;
using Domain.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Application.Interfaces.Products;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _stockRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ProductService(
            IProductRepository productRepository, 
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository stockRepository,
            IMapper mapper, 
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
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
                
            // Reload with optimized projection (Read Model)
            var readModel = await _productRepository.GetProjectedByIdAsync(newproduct.Id);
            var result = _mapper.Map<ProductDto>(readModel);
            
            // Apply Security (Allow Admin and Manager)
            if (!_currentUserService.IsInRole("Admin") && !_currentUserService.IsInRole("Manager")) 
                result.PurchasePrice = null;
            
            return result;
        }

        public async Task<PaginatedResult<ProductDto>> GetAllAsync(PaginationParams @params)
        {
            var paginatedReadModels = await _productRepository.GetProjectedAllAsync(@params);
            
            if (paginatedReadModels.TotalCount <= 0) 
            {
                throw new NotFoundException("No Products found!!");
            }
            
            bool isAdmin = _currentUserService.IsInRole("Admin");
            bool isManager = _currentUserService.IsInRole("Manager");

            var dtos = paginatedReadModels.Items.Select(rm => {
                var dto = _mapper.Map<ProductDto>(rm);
                if (!isAdmin && !isManager) dto.PurchasePrice = null;
                return dto;
            }).ToList();
            
            return new PaginatedResult<ProductDto>(dtos, paginatedReadModels.TotalCount, @params.PageNumber, @params.PageSize);
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new BadRequestException("Id must be greater than 0");
            }

            var readModel = await _productRepository.GetProjectedByIdAsync(id);
            
            if(readModel == null)
            {
                throw new NotFoundException($"Product with id:{id} not found");
            }

            var dto = _mapper.Map<ProductDto>(readModel);
            if (!_currentUserService.IsInRole("Admin") && !_currentUserService.IsInRole("Manager")) 
                dto.PurchasePrice = null;

            return dto;
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

        public async Task<ProductStockBreakdownResponseDto> GetStockBreakdownAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new NotFoundException($"Product {productId} not found");

            var allWarehouses = await _warehouseRepository.GetAllAsync();
            var stocks = await _stockRepository.GetByProductAsync(productId);

            var locations = allWarehouses.Select(w => new ProductStockBreakdownDto
            {
                WarehouseId = w.Id,
                WarehouseName = w.Name,
                Quantity = stocks.FirstOrDefault(s => s.WarehouseId == w.Id)?.Quantity ?? 0
            }).ToList();

            return new ProductStockBreakdownResponseDto
            {
                TotalStock = locations.Sum(l => l.Quantity),
                Locations = locations
            };
        }
    }
}
