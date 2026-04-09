using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.SalesOrder;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Common;
using Domain.Enums;

namespace Application.Tests.Services
{
    public class SalesOrderServiceTests : ServiceTestFixture
    {
        private readonly Mock<ISalesOrderRepository> _mockSalesOrderRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IWarehouseRepository> _mockWarehouseRepository;
        private readonly Mock<IProductWarehouseStockRepository> _mockStockRepository;
        private readonly SalesOrderService _salesOrderService;

        public SalesOrderServiceTests()
        {
            _mockSalesOrderRepository = new Mock<ISalesOrderRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockWarehouseRepository = new Mock<IWarehouseRepository>();
            _mockStockRepository = new Mock<IProductWarehouseStockRepository>();

            _salesOrderService = new SalesOrderService(
                _mockSalesOrderRepository.Object,
                _mockCustomerRepository.Object,
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
            var emptyResult = new PaginatedResult<SalesOrder>(new List<SalesOrder>(), 0, 1, 10);

            _mockSalesOrderRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(emptyResult);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _salesOrderService.GetAllAsync(paginationParams));
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnPaginatedOrders_When_OrdersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var orders = new List<SalesOrder> 
            { 
                TestDataBuilder.CreateTestSalesOrder(1), 
                TestDataBuilder.CreateTestSalesOrder(2) 
            };
            var paginatedOrders = new PaginatedResult<SalesOrder>(orders, 2, 1, 10);
            var orderDtos = new List<SalesOrderDto> { new SalesOrderDto { Id = 1 }, new SalesOrderDto { Id = 2 } };

            _mockSalesOrderRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(paginatedOrders);
            MockMapper.Setup(x => x.Map<List<SalesOrderDto>>(orders)).Returns(orderDtos);

            // Act
            var result = await _salesOrderService.GetAllAsync(paginationParams);

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
            await Assert.ThrowsAsync<BadRequestException>(() => _salesOrderService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_OrderNotFound()
        {
            // Arrange
            _mockSalesOrderRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((SalesOrder)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _salesOrderService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnOrder_When_OrderExists()
        {
            // Arrange
            var order = TestDataBuilder.CreateTestSalesOrder(1);
            var orderDto = new SalesOrderDto { Id = 1 };

            _mockSalesOrderRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
            MockMapper.Setup(x => x.Map<SalesOrderDto>(order)).Returns(orderDto);

            // Act
            var result = await _salesOrderService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_When_CustomerNotFound()
        {
            // Arrange
            var createDto = new CreateSalesOrderDto { CustomerId = 1, WarehouseId = 1, Items = new List<CreateSalesOrderItemDto>() };
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _salesOrderService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_NotFoundException_When_WarehouseNotFound()
        {
            // Arrange
            var customer = TestDataBuilder.CreateTestCustomer(1);
            var createDto = new CreateSalesOrderDto { CustomerId = 1, WarehouseId = 1, Items = new List<CreateSalesOrderItemDto>() };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _salesOrderService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_DuplicateProductInItems()
        {
            // Arrange
            var customer = TestDataBuilder.CreateTestCustomer(1);
            var warehouse = TestDataBuilder.CreateTestWarehouse(1);
            var product = TestDataBuilder.CreateTestProduct(1);

            var createDto = new CreateSalesOrderDto
            {
                CustomerId = 1,
                WarehouseId = 1,
                Items = new List<CreateSalesOrderItemDto>
        {
            new CreateSalesOrderItemDto { ProductId = 1, Quantity = 5, UnitPrice = 100 },
            new CreateSalesOrderItemDto { ProductId = 1, Quantity = 3, UnitPrice = 100 }
        }
            };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockWarehouseRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(warehouse);

            // 🔥 FIX: mock product
            _mockProductRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _salesOrderService.CreateAsync(createDto));
        }

        #endregion
    }
}
