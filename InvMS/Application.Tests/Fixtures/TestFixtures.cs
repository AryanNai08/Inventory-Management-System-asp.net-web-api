using AutoMapper;
using Moq;
using Domain.Interfaces;
using Domain.Interfaces.Auth;

namespace Application.Tests.Fixtures
{
    /// <summary>
    /// Base fixture for service testing providing common mock setup
    /// </summary>
    public abstract class ServiceTestFixture
    {
        protected readonly Mock<IMapper> MockMapper;
        protected readonly Mock<IUnitOfWork> MockUnitOfWork;

        protected ServiceTestFixture()
        {
            MockMapper = new Mock<IMapper>();
            MockUnitOfWork = new Mock<IUnitOfWork>();
        }

        protected void ResetAllMocks()
        {
            MockMapper.Reset();
            MockUnitOfWork.Reset();
        }
    }

    /// <summary>
    /// Helper class for creating test data entities and DTOs
    /// </summary>
    public static class TestDataBuilder
    {
        public static Domain.Entities.User CreateTestUser(
            int id = 1,
            string username = "testuser",
            string email = "test@example.com",
            string passwordHash = "$2a$11$hash",
            bool isDeleted = false)
        {
            return new Domain.Entities.User
            {
                Id = id,
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                IsDeleted = isDeleted,
                CreatedDate = DateTime.UtcNow,
                Roles = new List<Domain.Entities.Role>()
            };
        }

        public static Domain.Entities.Category CreateTestCategory(
            int id = 1,
            string name = "Electronics",
            string description = "Electronic products")
        {
            return new Domain.Entities.Category
            {
                Id = id,
                Name = name,
                Description = description,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Domain.Entities.Product CreateTestProduct(
            int id = 1,
            string sku = "PROD-001",
            string name = "Test Product",
            decimal purchasePrice = 100m,
            decimal salePrice = 120m,
            int categoryId = 1,
            int supplierId = 1,
            int warehouseId = 1)
        {
            return new Domain.Entities.Product
            {
                Id = id,
                Sku = sku,
                Name = name,
                PurchasePrice = purchasePrice,
                SalePrice = salePrice,
                CategoryId = categoryId,
                SupplierId = supplierId,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Domain.Entities.Customer CreateTestCustomer(
            int id = 1,
            string name = "Test Customer",
            string city = "Test City",
            string phone = "1234567890")
        {
            return new Domain.Entities.Customer
            {
                Id = id,
                Name = name,
                City = city,
                Phone = phone,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Domain.Entities.Supplier CreateTestSupplier(
            int id = 1,
            string name = "Test Supplier",
            string city = "Test City",
            string phone = "1234567890")
        {
            return new Domain.Entities.Supplier
            {
                Id = id,
                Name = name,
                City = city,
                Phone = phone,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Domain.Entities.Warehouse CreateTestWarehouse(
            int id = 1,
            string name = "Test Warehouse",
            string location = "Test Location")
        {
            return new Domain.Entities.Warehouse
            {
                Id = id,
                Name = name,
                Location = location,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Domain.Entities.Role CreateTestRole(
            int id = 1,
            string name = "Admin",
            string description = "Administrator role")
        {
            return new Domain.Entities.Role
            {
                Id = id,
                Name = name,
                Description = description,
                Privileges = new List<Domain.Entities.Privilege>()
            };
        }

        public static Domain.Entities.Privilege CreateTestPrivilege(
            int id = 1,
            string name = "Create",
            string description = "Create permission")
        {
            return new Domain.Entities.Privilege
            {
                Id = id,
                Name = name,
                Description = description
            };
        }

        public static Domain.Entities.SalesOrder CreateTestSalesOrder(
    int id = 1,
    int customerId = 1)
        {
            return new Domain.Entities.SalesOrder
            {
                Id = id,
                OrderNumber = "SO-001",
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                StatusId = 1,
                TotalAmount = 500m,
                Notes = "Test Sales Order",
                CreatedDate = DateTime.UtcNow,

                // Correct collection name
                SalesOrderItems = new List<Domain.Entities.SalesOrderItem>(),

                Status = new Domain.Entities.SalesOrderStatus
                {
                    Id = 1,
                    Name = "Pending"
                },

                Customer = new Domain.Entities.Customer
                {
                    Id = customerId,
                    Name = "Test Customer"
                }
            };
        }

        public static Domain.Entities.PurchaseOrder CreateTestPurchaseOrder(
    int id = 1,
    int supplierId = 1)
        {
            return new Domain.Entities.PurchaseOrder
            {
                Id = id,
                OrderNumber = "PO-001",
                SupplierId = supplierId,
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(5),
                StatusId = 1,
                TotalAmount = 1000m,
                Notes = "Test Purchase Order",
                CreatedDate = DateTime.UtcNow,

                // Correct collection name
                PurchaseOrderItems = new List<Domain.Entities.PurchaseOrderItem>(),

                // Navigation (optional but safe)
                Status = new Domain.Entities.PurchaseOrderStatus
                {
                    Id = 1,
                    Name = "Pending"
                },

                Supplier = new Domain.Entities.Supplier
                {
                    Id = supplierId,
                    Name = "Test Supplier"
                }
            };
        }

        public static Domain.Entities.StockAdjustment CreateTestStockAdjustment(
            int id = 1,
            int productId = 1,
            int warehouseId = 1,
            int quantityChange = 10,
            int adjustedBy = 1)
        {
            return new Domain.Entities.StockAdjustment
            {
                Id = id,
                ProductId = productId,
                WarehouseId = warehouseId,
                QuantityChange = quantityChange,
                AdjustedBy = adjustedBy,
                AdjustmentDate = DateTime.UtcNow
            };
        }
    }
}
