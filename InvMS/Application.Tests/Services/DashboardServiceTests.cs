using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Common.Models;

namespace Application.Tests.Services
{
    public class DashboardServiceTests
    {
        private readonly Mock<IDashboardRepository> _mockDashboardRepository;
        private readonly DashboardService _dashboardService;

        public DashboardServiceTests()
        {
            _mockDashboardRepository = new Mock<IDashboardRepository>();

            _dashboardService = new DashboardService(
                _mockDashboardRepository.Object
            );
        }

        #region GetSummaryAsync Tests

        [Fact]
        public async Task GetSummaryAsync_Should_ReturnDashboardSummary()
        {
            // Arrange
            var summaryData = new DashboardSummary
            {
                TotalSales = 10000,
                TotalPurchases = 5000,
                TotalProducts = 50,
                TotalSuppliers = 10,
                TotalCustomers = 20,
                LowStockItemsCount = 5,
                TopSellingProducts = new List<TopProduct>()
            };

            _mockDashboardRepository
                .Setup(x => x.GetSummaryStatsAsync())
                .ReturnsAsync(summaryData);

            // Act
            var result = await _dashboardService.GetSummaryAsync();

            // Assert
            result.Should().NotBeNull();
            result.TotalSales.Should().Be(10000);
            result.TotalProducts.Should().Be(50);
        }

        #endregion

        #region GetLowStockReportAsync Tests

        [Fact]
        public async Task GetLowStockReportAsync_Should_ReturnLowStockReport()
        {
            // Arrange
            var lowStockData = new List<LowStock>
            {
                new LowStock
                {
                    ProductId = 1,
                    ProductName = "Product 1",
                    CurrentStock = 5,
                    ReorderLevel = 10,
                    CategoryName = "Electronics"
                }
            };

            _mockDashboardRepository
                .Setup(x => x.GetLowStockReportAsync())
                .ReturnsAsync(lowStockData);

            // Act
            var result = await _dashboardService.GetLowStockReportAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().ProductName.Should().Be("Product 1");
        }

        #endregion

        #region GetTopSellingProductsAsync Tests

        [Fact]
        public async Task GetTopSellingProductsAsync_Should_ReturnTopProducts()
        {
            // Arrange
            var topProducts = new List<TopProduct>
            {
                new TopProduct
                {
                    ProductId = 1,
                    ProductName = "Top Product",
                    TotalQuantitySold = 100,
                    TotalRevenue = 10000
                }
            };

            _mockDashboardRepository
                .Setup(x => x.GetTopSellingProductsAsync(5))
                .ReturnsAsync(topProducts);

            // Act
            var result = await _dashboardService.GetTopSellingProductsAsync(5);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().ProductName.Should().Be("Top Product");
        }

        #endregion

        #region GetSalesByProductReportAsync Tests

        [Fact]
        public async Task GetSalesByProductReportAsync_Should_Throw_BadRequestException_When_StartDateAfterEndDate()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = DateTime.UtcNow;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _dashboardService.GetSalesByProductReportAsync(startDate, endDate));
        }

        #endregion

        #region GetPurchasesBySupplierReportAsync Tests

        [Fact]
        public async Task GetPurchasesBySupplierReportAsync_Should_Throw_BadRequestException_When_DateRangeInvalid()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = DateTime.UtcNow;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _dashboardService.GetPurchasesBySupplierReportAsync(startDate, endDate));
        }

        #endregion
    }
}