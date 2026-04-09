using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.StockAdjustment;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Tests.Services
{
    public class StockAdjustmentServiceTests : ServiceTestFixture
    {
        private readonly Mock<IStockAdjustmentRepository> _mockStockAdjustmentRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IWarehouseRepository> _mockWarehouseRepository;
        private readonly Mock<IProductWarehouseStockRepository> _mockStockRepository;
        private readonly StockAdjustmentService _stockAdjustmentService;

        public StockAdjustmentServiceTests()
        {
            _mockStockAdjustmentRepository = new Mock<IStockAdjustmentRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockWarehouseRepository = new Mock<IWarehouseRepository>();
            _mockStockRepository = new Mock<IProductWarehouseStockRepository>();

            _stockAdjustmentService = new StockAdjustmentService(
                _mockStockAdjustmentRepository.Object,
                _mockProductRepository.Object,
                _mockWarehouseRepository.Object,
                _mockStockRepository.Object,
                MockUnitOfWork.Object,
                MockMapper.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_ProductIdIsInvalid()
        {
            // Arrange
            var createDto = new CreateStockAdjustmentDto { ProductId = 0, WarehouseId = 1, QuantityChange = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _stockAdjustmentService.CreateAsync(createDto, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_WarehouseIdIsInvalid()
        {
            // Arrange
            var createDto = new CreateStockAdjustmentDto { ProductId = 1, WarehouseId = 0, QuantityChange = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _stockAdjustmentService.CreateAsync(createDto, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_QuantityChangeIsZero()
        {
            // Arrange
            var createDto = new CreateStockAdjustmentDto { ProductId = 1, WarehouseId = 1, QuantityChange = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _stockAdjustmentService.CreateAsync(createDto, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_When_ProductNotFound()
        {
            // Arrange
            var createDto = new CreateStockAdjustmentDto { ProductId = 1, WarehouseId = 1, QuantityChange = 10 };
            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _stockAdjustmentService.CreateAsync(createDto, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_When_WarehouseNotFound()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(1);
            var createDto = new CreateStockAdjustmentDto { ProductId = 1, WarehouseId = 1, QuantityChange = 10 };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _stockAdjustmentService.CreateAsync(createDto, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_InsufficientStock()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(1);
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            var stock = new ProductWarehouseStock { ProductId = 1, WarehouseId = 1, Quantity = 5 };
            var createDto = new CreateStockAdjustmentDto { ProductId = 1, WarehouseId = 1, QuantityChange = -10 };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);
            _mockStockRepository.Setup(x => x.GetByProductAndWarehouseAsync(1, 1)).ReturnsAsync(stock);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _stockAdjustmentService.CreateAsync(createDto, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_CreateAdjustment_When_QuantityChangeIsValid()
        {
            // Arrange
            var product = TestDataBuilder.CreateTestProduct(1);
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            var stock = new ProductWarehouseStock { ProductId = 1, WarehouseId = 1, Quantity = 10 };
            var adjustment = TestDataBuilder.CreateTestStockAdjustment(quantityChange: 5);
            var adjustmentDto = new StockAdjustmentDto { Id = 1 };

            var createDto = new CreateStockAdjustmentDto { ProductId = 1, WarehouseId = 1, QuantityChange = 5 };

            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);
            _mockStockRepository.Setup(x => x.GetByProductAndWarehouseAsync(1, 1)).ReturnsAsync(stock);
            _mockStockAdjustmentRepository.Setup(x => x.AddAsync(It.IsAny<StockAdjustment>())).Returns(Task.CompletedTask);
            _mockStockRepository.Setup(x => x.AddAsync(It.IsAny<ProductWarehouseStock>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockUnitOfWork.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
            _mockStockAdjustmentRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(adjustment);
            MockMapper.Setup(x => x.Map<StockAdjustment>(createDto)).Returns(adjustment);
            MockMapper.Setup(x => x.Map<StockAdjustmentDto>(adjustment)).Returns(adjustmentDto);

            // Act
            var result = await _stockAdjustmentService.CreateAsync(createDto, 1);

            // Assert
            result.Should().NotBeNull();
            _mockStockAdjustmentRepository.Verify(x => x.AddAsync(It.IsAny<StockAdjustment>()), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _stockAdjustmentService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_AdjustmentNotFound()
        {
            // Arrange
            _mockStockAdjustmentRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((StockAdjustment)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _stockAdjustmentService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnAdjustment_When_AdjustmentExists()
        {
            // Arrange
            var adjustment = TestDataBuilder.CreateTestStockAdjustment(1);
            var adjustmentDto = new StockAdjustmentDto { Id = 1 };

            _mockStockAdjustmentRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(adjustment);
            MockMapper.Setup(x => x.Map<StockAdjustmentDto>(adjustment)).Returns(adjustmentDto);

            // Act
            var result = await _stockAdjustmentService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion
    }
}
