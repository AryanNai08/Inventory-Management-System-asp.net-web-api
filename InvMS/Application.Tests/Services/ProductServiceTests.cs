using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Product;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Common;
using Domain.Models;

namespace Application.Tests.Services
{
    public class ProductServiceTests : ServiceTestFixture
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object,
                MockCurrentUserService.Object);
                
            // Default setup for current user (Admin by default)
            MockCurrentUserService.Setup(x => x.IsInRole("Admin")).Returns(true);
            MockCurrentUserService.Setup(x => x.IsInRole("Manager")).Returns(false);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_SkuAlreadyExists()
        {
            // Arrange
            var existingProduct = TestDataBuilder.CreateTestProduct(sku: "PROD-001");
            var createProductDto = new CreateProductDto { Sku = "PROD-001", Name = "Product", PurchasePrice = 100, SalePrice = 120 };

            _mockProductRepository.Setup(x => x.GetBySkuAsync("PROD-001")).ReturnsAsync(existingProduct);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productService.CreateAsync(createProductDto));
        }

        [Fact]
        public async Task CreateAsync_Should_CreateProduct_When_SkuIsUnique()
        {
            var createProductDto = new CreateProductDto { Sku = "PROD-001", Name = "Product", PurchasePrice = 100, SalePrice = 120 };
            var newProduct = TestDataBuilder.CreateTestProduct(sku: "PROD-001");
            var readModel = new ProductReadModel { Id = 1, Sku = "PROD-001", Name = "Product" };
            var productDto = new ProductDto { Id = 1, Sku = "PROD-001", Name = "Product", PurchasePrice = 100, SalePrice = 120 };

            _mockProductRepository.Setup(x => x.GetBySkuAsync("PROD-001")).ReturnsAsync((Product)null);
            _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _mockProductRepository.Setup(x => x.GetProjectedByIdAsync(It.IsAny<int>())).ReturnsAsync(readModel);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Product>(createProductDto)).Returns(newProduct);
            MockMapper.Setup(x => x.Map<ProductDto>(readModel)).Returns(productDto);

            // Act
            var result = await _productService.CreateAsync(createProductDto);

            // Assert
            result.Should().NotBeNull();
            result.Sku.Should().Be("PROD-001");
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoProductsExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var emptyResult = new PaginatedResult<ProductReadModel>(new List<ProductReadModel>(), 0, 1, 10);

            _mockProductRepository.Setup(x => x.GetProjectedAllAsync(paginationParams)).ReturnsAsync(emptyResult);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetAllAsync(paginationParams));
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnPaginatedProducts_When_ProductsExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var readModels = new List<ProductReadModel>
            {
                new ProductReadModel { Id = 1, Sku = "PROD-001" },
                new ProductReadModel { Id = 2, Sku = "PROD-002" }
            };
            var paginatedReadModels = new PaginatedResult<ProductReadModel>(readModels, 2, 1, 10);

            var productDto1 = new ProductDto { Id = 1, Sku = "PROD-001" };
            var productDto2 = new ProductDto { Id = 2, Sku = "PROD-002" };

            _mockProductRepository.Setup(x => x.GetProjectedAllAsync(paginationParams)).ReturnsAsync(paginatedReadModels);
            MockMapper.Setup(x => x.Map<ProductDto>(readModels[0])).Returns(productDto1);
            MockMapper.Setup(x => x.Map<ProductDto>(readModels[1])).Returns(productDto2);

            // Act
            var result = await _productService.GetAllAsync(paginationParams);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var id = 0;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productService.GetByIdAsync(id));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_ProductNotFound()
        {
            // Arrange
            _mockProductRepository.Setup(x => x.GetProjectedByIdAsync(1)).ReturnsAsync((ProductReadModel)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnProduct_When_ProductExists()
        {
            // Arrange
            var readModel = new ProductReadModel { Id = 1, Sku = "PROD-001" };
            var productDto = new ProductDto { Id = 1, Sku = "PROD-001" };

            _mockProductRepository.Setup(x => x.GetProjectedByIdAsync(1)).ReturnsAsync(readModel);
            MockMapper.Setup(x => x.Map<ProductDto>(readModel)).Returns(productDto);

            // Act
            var result = await _productService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        #endregion

        #region GetBySkuAsync Tests

        [Fact]
        public async Task GetBySkuAsync_Should_Throw_BadRequestException_When_SkuIsEmpty()
        {
            // Arrange
            var sku = "";

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productService.GetBySkuAsync(sku));
        }

        [Fact]
        public async Task GetBySkuAsync_Should_Throw_NotFoundException_When_ProductNotFound()
        {
            // Arrange
            _mockProductRepository.Setup(x => x.GetBySkuAsync("PROD-001")).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetBySkuAsync("PROD-001"));
        }

        [Fact]
        public async Task GetBySkuAsync_Should_ReturnProduct_When_ProductExists()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(sku: "PROD-001");
            var productDto = new ProductDto { Sku = "PROD-001" };

            _mockProductRepository.Setup(x => x.GetBySkuAsync("PROD-001")).ReturnsAsync(product);
            MockMapper.Setup(x => x.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _productService.GetBySkuAsync("PROD-001");

            // Assert
            result.Should().NotBeNull();
            result.Sku.Should().Be("PROD-001");
        }

        #endregion

        #region SearchAsync Tests

        [Fact]
        public async Task SearchAsync_Should_Throw_NotFoundException_When_NoMatchesFound()
        {
            // Arrange
            _mockProductRepository.Setup(x => x.SearchAsync("NonExistent", null, null)).ReturnsAsync(new List<Product>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.SearchAsync("NonExistent", null, null));
        }

        [Fact]
        public async Task SearchAsync_Should_ReturnMatchingProducts()
        {
            // Arrange
            var products = new List<Product> { TestDataBuilder.CreateTestProduct() };
            var productDtos = new List<ProductDto> { new ProductDto { Id = 1 } };

            _mockProductRepository.Setup(x => x.SearchAsync("Test", null, null)).ReturnsAsync(products);
            MockMapper.Setup(x => x.Map<List<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.SearchAsync("Test", null, null);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var id = 0;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productService.SoftDeleteAsync(id));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_NotFoundException_When_ProductNotFound()
        {
            // Arrange
            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_DeleteProduct_When_ProductExists()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(1);
            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _mockProductRepository.Setup(x => x.SoftDeleteAsync(1)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _productService.SoftDeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            _mockProductRepository.Verify(x => x.SoftDeleteAsync(1), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateProductDto();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.UpdateAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_ProductNotFound()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Sku = "PROD-001" };
            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_NewSkuAlreadyExists()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(1, "PROD-001");
            var conflictingProduct = TestDataBuilder.CreateTestProduct(2, "PROD-002");
            var updateDto = new UpdateProductDto { Sku = "PROD-002" };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _mockProductRepository.Setup(x => x.GetBySkuAsync("PROD-002")).ReturnsAsync(conflictingProduct);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _productService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateProduct_When_SkuIsUnique()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(1, "PROD-001");
            var updateDto = new UpdateProductDto { Sku = "PROD-002" };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _mockProductRepository.Setup(x => x.GetBySkuAsync("PROD-002")).ReturnsAsync((Product)null);
            _mockProductRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, product));

            // Act
            var result = await _productService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region GetLowStockProducts Tests

        [Fact]
        public async Task GetLowStockProducts_Should_ReturnLowStockProducts()
        {
            // Arrange
            var products = new List<Product> { TestDataBuilder.CreateTestProduct() };
            var productDtos = new List<ProductDto> { new ProductDto { Id = 1 } };

            _mockProductRepository.Setup(x => x.GetLowStockAsync()).ReturnsAsync(products);
            MockMapper.Setup(x => x.Map<List<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetLowStockProducts();

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region GetOutOfStockProducts Tests

        [Fact]
        public async Task GetOutOfStockProducts_Should_ReturnOutOfStockProducts()
        {
            // Arrange
            var products = new List<Product> { TestDataBuilder.CreateTestProduct() };
            var productDtos = new List<ProductDto> { new ProductDto { Id = 1 } };

            _mockProductRepository.Setup(x => x.GetOutOfStockAsync()).ReturnsAsync(products);
            MockMapper.Setup(x => x.Map<List<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetOutOfStockProducts();

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion
    }
}
