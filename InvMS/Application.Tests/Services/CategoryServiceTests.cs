using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Category;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Application.DTOs.Auth;

namespace Application.Tests.Services
{
    public class CategoryServiceTests : ServiceTestFixture
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockProductRepository = new Mock<IProductRepository>();

            _categoryService = new CategoryService(
                _mockCategoryRepository.Object,
                _mockProductRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_Should_Throw_BadRequestException_When_CategoryNameAlreadyExists()
        {
            // Arrange
            var existingCategory = TestDataBuilder.CreateTestCategory(name: "Electronics");
            var createCategoryDto = new CreateCategoryDto { Name = "Electronics", Description = "Electronic products" };

            _mockCategoryRepository.Setup(x => x.GetByNameAsync("Electronics")).ReturnsAsync(existingCategory);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.CreateAsync(createCategoryDto));
            _mockCategoryRepository.Verify(x => x.GetByNameAsync("Electronics"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_CreateCategory_When_NameDoesNotExist()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto { Name = "New Category", Description = "Description" };
            var newCategory = new Category { Id = 1, Name = "New Category", Description = "Description", CreatedDate = DateTime.UtcNow };
            var categoryDto = new CategoryDto { Id = 1, Name = "New Category", Description = "Description" };

            _mockCategoryRepository.Setup(x => x.GetByNameAsync("New Category")).ReturnsAsync((Category)null);
            _mockCategoryRepository.Setup(x => x.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Category>(createCategoryDto)).Returns(newCategory);
            MockMapper.Setup(x => x.Map<CategoryDto>(newCategory)).Returns(categoryDto);

            // Act
            var result = await _categoryService.CreateAsync(createCategoryDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Category");
            _mockCategoryRepository.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_SetCreatedDate_When_Creating()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto { Name = "New Category", Description = "Description" };
            Category capturedCategory = null;

            var newCategory = new Category { Id = 1, Name = "New Category", Description = "Description" };

            _mockCategoryRepository.Setup(x => x.GetByNameAsync("New Category")).ReturnsAsync((Category)null);
            _mockCategoryRepository.Setup(x => x.AddAsync(It.IsAny<Category>())).Callback<Category>(cat => capturedCategory = cat).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Category>(createCategoryDto)).Returns(newCategory);
            MockMapper.Setup(x => x.Map<CategoryDto>(newCategory)).Returns(new CategoryDto { Id = 1, Name = "New Category" });

            // Act
            await _categoryService.CreateAsync(createCategoryDto);

            // Assert
            capturedCategory.Should().NotBeNull();
            capturedCategory.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_Should_Throw_NotFoundException_When_NoCategoriesExist()
        {
            // Arrange
            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.GetAllAsync());
            _mockCategoryRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnAllCategories_When_CategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                TestDataBuilder.CreateTestCategory(1, "Electronics"),
                TestDataBuilder.CreateTestCategory(2, "Furniture")
            };

            var categoryDtos = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Electronics" },
                new CategoryDto { Id = 2, Name = "Furniture" }
            };

            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            MockMapper.Setup(x => x.Map<List<CategoryDto>>(categories)).Returns(categoryDtos);

            // Act
            var result = await _categoryService.GetAllAsync();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Electronics");
            result[1].Name.Should().Be("Furniture");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsZero()
        {
            // Arrange
            var id = 0;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.GetByIdAsync(id));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_BadRequestException_When_IdIsNegative()
        {
            // Arrange
            var id = -1;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.GetByIdAsync(id));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_NotFoundException_When_CategoryNotFound()
        {
            // Arrange
            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.GetByIdAsync(1));
            _mockCategoryRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnCategory_When_CategoryExists()
        {
            // Arrange
            var category = TestDataBuilder.CreateTestCategory(1, "Electronics");
            var categoryDto = new CategoryDto { Id = 1, Name = "Electronics" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            MockMapper.Setup(x => x.Map<CategoryDto>(category)).Returns(categoryDto);

            // Act
            var result = await _categoryService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Electronics");
            _mockCategoryRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_IdIsZero()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { Name = "Updated", Description = "Updated Description" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.UpdateAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_NotFoundException_When_CategoryNotFound()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { Name = "Updated", Description = "Updated Description" };
            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_BadRequestException_When_NewNameAlreadyExists()
        {
            // Arrange
            var existingCategory = TestDataBuilder.CreateTestCategory(1, "Electronics");
            var conflictingCategory = TestDataBuilder.CreateTestCategory(2, "NewName");
            var updateDto = new UpdateCategoryDto { Name = "NewName", Description = "Updated Description" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingCategory);
            _mockCategoryRepository.Setup(x => x.GetByNameAsync("NewName")).ReturnsAsync(conflictingCategory);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.UpdateAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateCategory_When_NameIsUnique()
        {
            // Arrange
            var existingCategory = TestDataBuilder.CreateTestCategory(1, "OldName");
            var updateDto = new UpdateCategoryDto { Name = "NewName", Description = "Updated Description" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingCategory);
            _mockCategoryRepository.Setup(x => x.GetByNameAsync("NewName")).ReturnsAsync((Category)null);
            _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, existingCategory));

            // Act
            var result = await _categoryService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
            _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_AllowSameNameUpdate()
        {
            // Arrange
            var existingCategory = TestDataBuilder.CreateTestCategory(1, "Electronics");
            var updateDto = new UpdateCategoryDto { Name = "Electronics", Description = "Updated Description" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingCategory);
            _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, existingCategory));

            // Act
            var result = await _categoryService.UpdateAsync(1, updateDto);

            // Assert
            result.Should().BeTrue();
            _mockCategoryRepository.Verify(x => x.GetByNameAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region SoftDeleteAsync Tests

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_IdIsZero()
        {
            // Arrange
            var id = 0;

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.SoftDeleteAsync(id));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_NotFoundException_When_CategoryNotFound()
        {
            // Arrange
            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _categoryService.SoftDeleteAsync(1));
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_Throw_BadRequestException_When_ProductsLinked()
        {
            // Arrange
            var category = TestDataBuilder.CreateTestCategory(1);
            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            _mockProductRepository.Setup(x => x.ExistsByCategoryIdAsync(1)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _categoryService.SoftDeleteAsync(1));
            _mockCategoryRepository.Verify(x => x.SoftDeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SoftDeleteAsync_Should_DeleteCategory_When_NoProductsLinked()
        {
            // Arrange
            var category = TestDataBuilder.CreateTestCategory(1);
            _mockCategoryRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);
            _mockProductRepository.Setup(x => x.ExistsByCategoryIdAsync(1)).ReturnsAsync(false);
            _mockCategoryRepository.Setup(x => x.SoftDeleteAsync(1)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _categoryService.SoftDeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            _mockCategoryRepository.Verify(x => x.SoftDeleteAsync(1), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
