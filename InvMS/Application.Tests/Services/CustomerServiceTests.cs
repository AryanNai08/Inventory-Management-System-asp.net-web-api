using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Customer;
using Application.DTOs.SalesOrder;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Common;

namespace Application.Tests.Services
{
    public class CustomerServiceTests : ServiceTestFixture
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();

            _customerService = new CustomerService(
                _mockCustomerRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_NameAlreadyExists()
        {
            // Arrange
            var existingCustomer = TestDataBuilder.CreateTestCustomer(name: "John Doe");
            var createDto = new CreateCustomerDto { Name = "John Doe", City = "City", Phone = "1234567890" };

            _mockCustomerRepository.Setup(x => x.GetByNameAsync("John Doe")).ReturnsAsync(existingCustomer);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _customerService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_Should_CreateCustomer_When_NameIsUnique()
        {
            // Arrange
            var createDto = new CreateCustomerDto { Name = "John Doe", City = "City", Phone = "1234567890" };
            var newCustomer = TestDataBuilder.CreateTestCustomer(name: "John Doe");
            var customerDto = new CustomerDto { Id = 1, Name = "John Doe" };

            _mockCustomerRepository.Setup(x => x.GetByNameAsync("John Doe")).ReturnsAsync((Customer)null);
            _mockCustomerRepository.Setup(x => x.AddAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Customer>(createDto)).Returns(newCustomer);
            MockMapper.Setup(x => x.Map<CustomerDto>(newCustomer)).Returns(customerDto);

            // Act
            var result = await _customerService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            _mockCustomerRepository.Verify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoCustomersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var emptyResult = new PaginatedResult<Customer>(new List<Customer>(), 0, 1, 10);

            _mockCustomerRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(emptyResult);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _customerService.GetAllAsync(paginationParams));
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnPaginatedCustomers_When_CustomersExist()
        {
            // Arrange
            var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 10 };
            var customers = new List<Customer> 
            { 
                TestDataBuilder.CreateTestCustomer(1, "Customer 1"), 
                TestDataBuilder.CreateTestCustomer(2, "Customer 2") 
            };
            var paginatedCustomers = new PaginatedResult<Customer>(customers, 2, 1, 10);
            var customerDtos = new List<CustomerDto> { new CustomerDto { Id = 1 }, new CustomerDto { Id = 2 } };

            _mockCustomerRepository.Setup(x => x.GetAllAsync(paginationParams)).ReturnsAsync(paginatedCustomers);
            MockMapper.Setup(x => x.Map<List<CustomerDto>>(customers)).Returns(customerDtos);

            // Act
            var result = await _customerService.GetAllAsync(paginationParams);

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
            await Assert.ThrowsAsync<BadRequestException>(() => _customerService.GetByIdAsync(0));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_CustomerNotFound()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _customerService.GetByIdAsync(1));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnCustomer_When_CustomerExists()
        {
            // Arrange
            var customer = TestDataBuilder.CreateTestCustomer(1);
            var customerDto = new CustomerDto { Id = 1 };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            MockMapper.Setup(x => x.Map<CustomerDto>(customer)).Returns(customerDto);

            // Act
            var result = await _customerService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        #endregion

        #region GetSalesOrdersByCustomerIdAsync Tests

        [Fact]
        public async Task GetSalesOrdersByCustomerIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _customerService.GetSalesOrdersByCustomerIdAsync(0));
        }

        [Fact]
        public async Task GetSalesOrdersByCustomerIdAsync_Should_Throw_NotFoundException_When_CustomerNotFound()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _customerService.GetSalesOrdersByCustomerIdAsync(1));
        }

        #endregion

        #region SearchAsync Tests

        [Fact]
        public async Task SearchAsync_Should_ReturnMatchingCustomers()
        {
            // Arrange
            var customers = new List<Customer> { TestDataBuilder.CreateTestCustomer() };
            var customerDtos = new List<CustomerDto> { new CustomerDto { Id = 1 } };

            _mockCustomerRepository.Setup(x => x.SearchAsync("Test", null)).ReturnsAsync(customers);
            MockMapper.Setup(x => x.Map<List<CustomerDto>>(customers)).Returns(customerDtos);

            // Act
            var result = await _customerService.SearchAsync("Test", null);

            // Assert
            result.Should().NotBeEmpty();
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _customerService.SoftDeleteAsync(0));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_NotFoundException_When_CustomerNotFound()
        {
            // Arrange
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _customerService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_DeleteCustomer_When_CustomerExists()
        {
            // Arrange
            var customer = TestDataBuilder.CreateTestCustomer(1);
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockCustomerRepository.Setup(x => x.SoftDeleteAsync(1)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _customerService.SoftDeleteAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateCustomerDto();

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _customerService.UpdateAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_CustomerNotFound()
        {
            // Arrange
            var updateDto = new UpdateCustomerDto { Name = "Updated" };
            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Customer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _customerService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateCustomer_When_CustomerExists()
        {
            // Arrange
            var customer = TestDataBuilder.CreateTestCustomer(1);
            var updateDto = new UpdateCustomerDto { Name = "Updated" };

            _mockCustomerRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockCustomerRepository.Setup(x => x.UpdateAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, customer));

            // Act
            var result = await _customerService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
        }

        #endregion
    }
}
