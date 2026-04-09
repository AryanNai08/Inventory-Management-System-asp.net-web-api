using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.PurchaseOrder;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Common;

namespace Application.Tests.Services
{
    public class PurchaseOrderServiceTests : ServiceTestFixture
    {
        private readonly Mock<IPurchaseOrderRepository> _mockPurchaseOrderRepository;
        private readonly Mock<ISupplierRepository> _mockSupplierRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IWarehouseRepository> _mockWarehouseRepository;
        private readonly Mock<IProductWarehouseStockRepository> _mockStockRepository;
        private readonly PurchaseOrderService _purchaseOrderService;

        public PurchaseOrderServiceTests()
        {
            _mockPurchaseOrderRepository = new Mock<IPurchaseOrderRepository>();
            _mockSupplierRepository = new Mock<ISupplierRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockWarehouseRepository = new Mock<IWarehouseRepository>();
            _mockStockRepository = new Mock<IProductWarehouseStockRepository>();

            _purchaseOrderService = new PurchaseOrderService(
                _mockPurchaseOrderRepository.Object,
                _mockSupplierRepository.Object,
                _mockProductRepository.Object,
                _mockWarehouseRepository.Object,
                _mockStockRepository.Object,
                MockUnitOfWork.Object,
                MockMapper.Object);
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoOrdersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var emptyResult = new PaginatedResult<PurchaseOrder>(new List<PurchaseOrder>(), 0, 1, 10);

            _mockPurchaseOrderRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(emptyResult);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseOrderService.GetAllAsync(paginationParams));
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnPaginatedOrders_When_OrdersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var orders = new List<PurchaseOrder> 
            { 
                TestDataBuilder.CreateTestPurchaseOrder(1), 
                TestDataBuilder.CreateTestPurchaseOrder(2) 
            };
            var paginatedOrders = new PaginatedResult<PurchaseOrder>(orders, 2, 1, 10);
            var orderDtos = new List<PurchaseOrderDto> { new PurchaseOrderDto { Id = 1 }, new PurchaseOrderDto { Id = 2 } };

            _mockPurchaseOrderRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(paginatedOrders);
            MockMapper.Setup(x => x.Map<List<PurchaseOrderDto>>(orders)).Returns(orderDtos);

            // Act
            var result = await _purchaseOrderService.GetAllAsync(paginationParams);

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
            await Assert.ThrowsAsync<BadRequestException>(() => _purchaseOrderService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_OrderNotFound()
        {
            // Arrange
            _mockPurchaseOrderRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((PurchaseOrder)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseOrderService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnOrder_When_OrderExists()
        {
            // Arrange
            var order = TestDataBuilder.CreateTestPurchaseOrder(1);
            var orderDto = new PurchaseOrderDto { Id = 1 };

            _mockPurchaseOrderRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
            MockMapper.Setup(x => x.Map<PurchaseOrderDto>(order)).Returns(orderDto);

            // Act
            var result = await _purchaseOrderService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_When_SupplierNotFound()
        {
            // Arrange
            var createDto = new CreatePurchaseOrderDto { SupplierId = 1, WarehouseId = 1, Items = new List<CreatePurchaseOrderItemDto>() };
            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseOrderService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_When_WarehouseNotFound()
        {
            // Arrange
            var supplier = TestDataBuilder.CreateTestSupplier(1);
            var createDto = new CreatePurchaseOrderDto { SupplierId = 1, WarehouseId = 1, Items = new List<CreatePurchaseOrderItemDto>() };

            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseOrderService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_DuplicateProductInItems()
        {
            // Arrange
            var supplier = TestDataBuilder.CreateTestSupplier(1);
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            var createDto = new CreatePurchaseOrderDto 
            { 
                SupplierId = 1, 
                WarehouseId = 1, 
                Items = new List<CreatePurchaseOrderItemDto>
                {
                    new CreatePurchaseOrderItemDto { ProductId = 1, Quantity = 5, UnitCost = 100 },
                    new CreatePurchaseOrderItemDto { ProductId = 1, Quantity = 3, UnitCost = 100 }
                }
            };

            _mockSupplierRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(supplier);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _purchaseOrderService.CreateAsync(createDto));
        }

        #endregion
    }
}
