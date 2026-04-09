using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.Auth;
using Application.DTOs.RolesAndPrivileges;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Tests.Services
{
    public class RoleServiceTests : ServiceTestFixture
    {
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();

            _roleService = new RoleService(
                _mockRoleRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region CreateRoleAsync Tests

        [Fact]
        public async Task CreateRoleAsync_Should_Throw_BadRequestException_When_RoleNameIsEmpty()
        {
            // Arrange
            var roleDto = new RoleDto { Name = "", Description = "Description" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _roleService.CreateRoleAsync(roleDto));
        }

        [Fact]
        public async Task CreateRoleAsync_Should_Throw_BadRequestException_When_RoleNameAlreadyExists()
        {
            // Arrange
            var roleDto = new RoleDto { Name = "Admin", Description = "Description" };
            _mockRoleRepository.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _roleService.CreateRoleAsync(roleDto));
        }

        [Fact]
        public async Task CreateRoleAsync_Should_CreateRole_When_NameIsUnique()
        {
            // Arrange
            var roleDto = new RoleDto { Name = "Admin", Description = "Administrator" };
            var role = TestDataBuilder.CreateTestRole(name: "Admin");

            _mockRoleRepository.Setup(x => x.RoleExistsAsync("Admin")).ReturnsAsync(false);
            _mockRoleRepository.Setup(x => x.CreateRoleAsync(It.IsAny<Role>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map<Role>(roleDto)).Returns(role);

            // Act
            await _roleService.CreateRoleAsync(roleDto);

            // Assert
            _mockRoleRepository.Verify(x => x.CreateRoleAsync(It.IsAny<Role>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetAllRolesAsync Tests

        [Fact]
        public async Task GetAllRolesAsync_Should_ReturnAllRoles()
        {
            // Arrange
            var roles = new List<Role> 
            { 
                TestDataBuilder.CreateTestRole(1, "Admin"), 
                TestDataBuilder.CreateTestRole(2, "User") 
            };
            var roleDtos = new List<RoleDto> 
            { 
                new RoleDto { Name = "Admin" }, 
                new RoleDto { Name = "User" } 
            };

            _mockRoleRepository.Setup(x => x.GetAllRolesAsync()).ReturnsAsync(roles);
            MockMapper.Setup(x => x.Map<List<RoleDto>>(roles)).Returns(roleDtos);

            // Act
            var result = await _roleService.GetAllRolesAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        #endregion

        #region GetRoleByIdAsync Tests

        [Fact]
        public async Task GetRoleByIdAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _roleService.GetRoleByIdAsync(0));
        }

        [Fact]
        public async Task GetRoleByIdAsync_Should_Throw_NotFoundException_When_RoleNotFound()
        {
            // Arrange
            _mockRoleRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Role)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _roleService.GetRoleByIdAsync(1));
        }

        [Fact]
        public async Task GetRoleByIdAsync_Should_ReturnRole_When_RoleExists()
        {
            // Arrange
            var role = TestDataBuilder.CreateTestRole(1, "Admin");
            var roleDto = new RoleDto { Name = "Admin" };

            _mockRoleRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(role);
            MockMapper.Setup(x => x.Map<RoleDto>(role)).Returns(roleDto);

            // Act
            var result = await _roleService.GetRoleByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region UpdateRoleAsync Tests

        [Fact]
        public async Task UpdateRoleAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new UpdateRoleDto { Name = "Updated" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _roleService.UpdateRoleAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdateRoleAsync_Should_Throw_BadRequestException_When_NameIsEmpty()
        {
            // Arrange
            var updateDto = new UpdateRoleDto { Name = "" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _roleService.UpdateRoleAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateRoleAsync_Should_Throw_NotFoundException_When_RoleNotFound()
        {
            // Arrange
            var updateDto = new UpdateRoleDto { Name = "Updated" };
            _mockRoleRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Role)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _roleService.UpdateRoleAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdateRoleAsync_Should_UpdateRole_When_RoleExists()
        {
            // Arrange
            var role = TestDataBuilder.CreateTestRole(1, "Admin");
            var updateDto = new UpdateRoleDto { Name = "SuperAdmin" };

            _mockRoleRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(role);
            _mockRoleRepository.Setup(x => x.UpdateRoleAsync(It.IsAny<Role>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, role));

            // Act
            await _roleService.UpdateRoleAsync(1, updateDto);

            // Assert
            _mockRoleRepository.Verify(x => x.UpdateRoleAsync(It.IsAny<Role>()), Times.Once);
        }

        #endregion

        #region DeleteRoleAsync Tests

        [Fact]
        public async Task DeleteRoleAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _roleService.DeleteRoleAsync(0));
        }

        [Fact]
        public async Task DeleteRoleAsync_Should_Throw_NotFoundException_When_RoleNotFound()
        {
            // Arrange
            _mockRoleRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Role)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _roleService.DeleteRoleAsync(1));
        }

        [Fact]
        public async Task DeleteRoleAsync_Should_DeleteRole_When_RoleExists()
        {
            // Arrange
            var role = TestDataBuilder.CreateTestRole(1);
            _mockRoleRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(role);
            _mockRoleRepository.Setup(x => x.DeleteRoleAsync(role)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _roleService.DeleteRoleAsync(1);

            // Assert
            _mockRoleRepository.Verify(x => x.DeleteRoleAsync(role), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
