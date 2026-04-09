using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Supplier;
using Application.DTOs.Product;
using Application.DTOs.PurchaseOrder;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Common;

namespace Application.Tests.Services
{
    public class SupplierServiceTests : ServiceTestFixture
    {
        private readonly Mock<ISupplierRepository> _mockSupplierRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly SupplierService _supplierService;

        public SupplierServiceTests()
        {
            _mockSupplierRepository = new Mock<ISupplierRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _supplierService = new SupplierService(
                _mockSupplierRepository.Object,
                _mockProductRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_NameAlreadyExists()
        {
            // Arrange
            var existingSupplier = TestDataBuilder.CreateTestSupplier(name: "Supplier A");
            var createDto = new CreateSupplierDto { Name = "Supplier A", City = "City", Phone = "1234567890" };

            _mockSupplierRepository.Setup(x => x.GetByNameAsync("Supplier A")).ReturnsAsync(existingSupplier);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _supplierService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_CreateSupplier_When_NameIsUnique()
        {
            // Arrange
            var createDto = new CreateSupplierDto { Name = "Supplier A", City = "City", Phone = "1234567890" };
            var newSupplier = TestDataBuilder.CreateTestSupplier(name: "Supplier A");
            var supplierDto = new SupplierDto { Id = 1, Name = "Supplier A" };

            _mockSupplierRepository.Setup(x => x.GetByNameAsync("Supplier A")).ReturnsAsync((Supplier)null);
            _mockSupplierRepository.Setup(x => x.AddAsync(It.IsAny<Supplier>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Supplier>(createDto)).Returns(newSupplier);
            MockMapper.Setup(x => x.Map<SupplierDto>(newSupplier)).Returns(supplierDto);

            // Act
            var result = await _supplierService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoSuppliersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var emptyResult = new PaginatedResult<Supplier>(new List<Supplier>(), 0, 1, 10);

            _mockSupplierRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(emptyResult);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _supplierService.GetAllAsync(paginationParams));
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnPaginatedSuppliers_When_SuppliersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var suppliers = new List<Supplier> 
            { 
                TestDataBuilder.CreateTestSupplier(1, "Supplier 1"), 
                TestDataBuilder.CreateTestSupplier(2, "Supplier 2") 
            };
            var paginatedSuppliers = new PaginatedResult<Supplier>(suppliers, 2, 1, 10);
            var supplierDtos = new List<SupplierDto> { new SupplierDto { Id = 1 }, new SupplierDto { Id = 2 } };

            _mockSupplierRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(paginatedSuppliers);
            MockMapper.Setup(x => x.Map<List<SupplierDto>>(suppliers)).Returns(supplierDtos);

            // Act
            var result = await _supplierService.GetAllAsync(paginationParams);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _supplierService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_SupplierNotFound()
        {
            // Arrange
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _supplierService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnSupplier_When_SupplierExists()
        {
            // Arrange
            var supplier = TestDataBuilder.CreateTestSupplier(1);
            var supplierDto = new SupplierDto { Id = 1 };

            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(supplier);
            MockMapper.Setup(x => x.Map<SupplierDto>(supplier)).Returns(supplierDto);

            // Act
            var result = await _supplierService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region GetProductsBySupplierIdAsync Tests

        [Fact]
        public async Task GetProductsBySupplierIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _supplierService.GetProductsBySupplierIdAsync(0));
        }

        [Fact]
        public async Task GetProductsBySupplierIdAsync_Should_Throw_NotFoundException_When_SupplierNotFound()
        {
            // Arrange
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _supplierService.GetProductsBySupplierIdAsync(1));
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _supplierService.SoftDeleteAsync(0));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_NotFoundException_When_SupplierNotFound()
        {
            // Arrange
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _supplierService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_ProductsLinked()
        {
            // Arrange
            var supplier = TestDataBuilder.CreateTestSupplier(1);
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockProductRepository.Setup(x => x.ExistsBySupplierIdAsync(1)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _supplierService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_DeleteSupplier_When_NoProductsLinked()
        {
            // Arrange
            var supplier = TestDataBuilder.CreateTestSupplier(1);
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockProductRepository.Setup(x => x.ExistsBySupplierIdAsync(1)).ReturnsAsync(false);
            _mockSupplierRepository.Setup(x => x.SoftDeleteAsync(1)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _supplierService.SoftDeleteAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateSupplierDto { Name = "Updated" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _supplierService.UpdateAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_SupplierNotFound()
        {
            // Arrange
            var updateDto = new UpdateSupplierDto { Name = "Updated" };
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _supplierService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateSupplier_When_SupplierExists()
        {
            // Arrange
            var supplier = TestDataBuilder.CreateTestSupplier(1);
            var updateDto = new UpdateSupplierDto { Name = "Updated" };

            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockSupplierRepository.Setup(x => x.UpdateAsync(It.IsAny<Supplier>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, supplier));

            // Act
            var result = await _supplierService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
        }

        #endregion
    }
}
