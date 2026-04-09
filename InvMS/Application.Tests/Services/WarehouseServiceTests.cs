using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Warehouse;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Tests.Services
{
    public class WarehouseServiceTests : ServiceTestFixture
    {
        private readonly Mock<IWarehouseRepository> _mockWarehouseRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly WarehouseService _warehouseService;

        public WarehouseServiceTests()
        {
            _mockWarehouseRepository = new Mock<IWarehouseRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _warehouseService = new WarehouseService(
                _mockWarehouseRepository.Object,
                _mockProductRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_NameAlreadyExists()
        {
            // Arrange
            var existingWarehouse = TestDataBuilder.CreateTestWarehouse(name: "Warehouse A");
            var createDto = new CreateWarehouseDto { Name = "Warehouse A", Location = "Location" };

            _mockWarehouseRepository.Setup(x => x.GetByNameAsync("Warehouse A")).ReturnsAsync(existingWarehouse);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _warehouseService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_CreateWarehouse_When_NameIsUnique()
        {
            // Arrange
            var createDto = new CreateWarehouseDto { Name = "Warehouse A", Location = "Location" };
            var newWarehouse = TestDataBuilder.CreateTestWarehouse(name: "Warehouse A");
            var warehouseDto = new WarehouseDto { Id = 1, Name = "Warehouse A" };

            _mockWarehouseRepository.Setup(x => x.GetByNameAsync("Warehouse A")).ReturnsAsync((Warehouse)null);
            _mockWarehouseRepository.Setup(x => x.AddAsync(It.IsAny<Warehouse>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Warehouse>(createDto)).Returns(newWarehouse);
            MockMapper.Setup(x => x.Map<WarehouseDto>(newWarehouse)).Returns(warehouseDto);

            // Act
            var result = await _warehouseService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            _mockWarehouseRepository.Verify(x => x.AddAsync(It.IsAny<Warehouse>()), Times.Once);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoWarehousesExist()
        {
            // Arrange
            _mockWarehouseRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Warehouse>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _warehouseService.GetAllAsync());
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnAllWarehouses_When_WarehousesExist()
        {
            // Arrange
            var warehouses = new List<Warehouse> 
            { 
                TestDataBuilder.CreateTestWarehouse(1, "Warehouse A"), 
                TestDataBuilder.CreateTestWarehouse(2, "Warehouse B") 
            };
            var warehouseDtos = new List<WarehouseDto> 
            { 
                new WarehouseDto { Id = 1, Name = "Warehouse A" }, 
                new WarehouseDto { Id = 2, Name = "Warehouse B" } 
            };

            _mockWarehouseRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(warehouses);
            MockMapper.Setup(x => x.Map<List<WarehouseDto>>(warehouses)).Returns(warehouseDtos);

            // Act
            var result = await _warehouseService.GetAllAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _warehouseService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_WarehouseNotFound()
        {
            // Arrange
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _warehouseService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnWarehouse_When_WarehouseExists()
        {
            // Arrange
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            var warehouseDto = new WarehouseDto { Id = 1 };

            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);
            MockMapper.Setup(x => x.Map<WarehouseDto>(warehouse)).Returns(warehouseDto);

            // Act
            var result = await _warehouseService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateWarehouseDto { Name = "Updated" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _warehouseService.UpdateAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_WarehouseNotFound()
        {
            // Arrange
            var updateDto = new UpdateWarehouseDto { Name = "Updated" };
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _warehouseService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateWarehouse_When_WarehouseExists()
        {
            // Arrange
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            var updateDto = new UpdateWarehouseDto { Name = "Updated" };

            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);
            _mockWarehouseRepository.Setup(x => x.UpdateAsync(It.IsAny<Warehouse>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, warehouse));

            // Act
            var result = await _warehouseService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _warehouseService.SoftDeleteAsync(0));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_NotFoundException_When_WarehouseNotFound()
        {
            // Arrange
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _warehouseService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_ProductsLinked()
        {
            // Arrange
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);
            _mockProductRepository.Setup(x => x.ExistsByWarehouseIdAsync(1)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _warehouseService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_DeleteWarehouse_When_NoProductsLinked()
        {
            // Arrange
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);
            _mockProductRepository.Setup(x => x.ExistsByWarehouseIdAsync(1)).ReturnsAsync(false);
            _mockWarehouseRepository.Setup(x => x.SoftDeleteAsync(1)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _warehouseService.SoftDeleteAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        #endregion
    }
}
