using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Application.DTOs.RolesAndPrivileges;
using Application.Tests.Fixtures;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Tests.Services
{
    public class RolePrivilegeServiceTests : ServiceTestFixture
    {
        private readonly Mock<IRolePrivilegeRepository> _mockRolePrivilegeRepository;
        private readonly RolePrivilegeService _rolePrivilegeService;

        public RolePrivilegeServiceTests()
        {
            _mockRolePrivilegeRepository = new Mock<IRolePrivilegeRepository>();

            _rolePrivilegeService = new RolePrivilegeService(
                _mockRolePrivilegeRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region AssignPrivilegeToRoleAsync Tests

        [Fact]
        public async Task AssignPrivilegeToRoleAsync_Should_Throw_NotFoundException_When_RoleNotFound()
        {
            // Arrange
            var rolePrivilegeDto = new RolePrivilegeDto { RoleId = 1, PrivilegeId = 1 };
            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync((Role)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _rolePrivilegeService.AssignPrivilegeToRoleAsync(rolePrivilegeDto));
        }

        [Fact]
        public async Task AssignPrivilegeToRoleAsync_Should_Throw_NotFoundException_When_PrivilegeNotFound()
        {
            // Arrange
            var role = TestDataBuilder.CreateTestRole(1);
            var rolePrivilegeDto = new RolePrivilegeDto { RoleId = 1, PrivilegeId = 1 };

            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);
            _mockRolePrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync((Privilege)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _rolePrivilegeService.AssignPrivilegeToRoleAsync(rolePrivilegeDto));
        }

        [Fact]
        public async Task AssignPrivilegeToRoleAsync_Should_Throw_BadRequestException_When_PrivilegeAlreadyAssigned()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var role = TestDataBuilder.CreateTestRole(1);
            role.Privileges.Add(privilege);

            var rolePrivilegeDto = new RolePrivilegeDto { RoleId = 1, PrivilegeId = 1 };

            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);
            _mockRolePrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => 
                _rolePrivilegeService.AssignPrivilegeToRoleAsync(rolePrivilegeDto));
        }

        [Fact]
        public async Task AssignPrivilegeToRoleAsync_Should_AssignPrivilege_When_Valid()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var role = TestDataBuilder.CreateTestRole(1);
            var rolePrivilegeDto = new RolePrivilegeDto { RoleId = 1, PrivilegeId = 1 };

            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);
            _mockRolePrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            _mockRolePrivilegeRepository.Setup(x => x.UpdateRoleAsync(It.IsAny<Role>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _rolePrivilegeService.AssignPrivilegeToRoleAsync(rolePrivilegeDto);

            // Assert
            role.Privileges.Should().Contain(privilege);
            _mockRolePrivilegeRepository.Verify(x => x.UpdateRoleAsync(role), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region RemovePrivilegeFromRoleAsync Tests

        [Fact]
        public async Task RemovePrivilegeFromRoleAsync_Should_Throw_NotFoundException_When_RoleNotFound()
        {
            // Arrange
            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync((Role)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _rolePrivilegeService.RemovePrivilegeFromRoleAsync(1, 1));
        }

        [Fact]
        public async Task RemovePrivilegeFromRoleAsync_Should_Throw_NotFoundException_When_PrivilegeNotAssigned()
        {
            // Arrange
            var role = TestDataBuilder.CreateTestRole(1);
            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _rolePrivilegeService.RemovePrivilegeFromRoleAsync(1, 1));
        }

        [Fact]
        public async Task RemovePrivilegeFromRoleAsync_Should_RemovePrivilege_When_Valid()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var role = TestDataBuilder.CreateTestRole(1);
            role.Privileges.Add(privilege);

            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);
            _mockRolePrivilegeRepository.Setup(x => x.UpdateRoleAsync(It.IsAny<Role>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _rolePrivilegeService.RemovePrivilegeFromRoleAsync(1, 1);

            // Assert
            role.Privileges.Should().NotContain(privilege);
            _mockRolePrivilegeRepository.Verify(x => x.UpdateRoleAsync(role), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetPrivilegesByRoleIdAsync Tests

        [Fact]
        public async Task GetPrivilegesByRoleIdAsync_Should_Throw_NotFoundException_When_RoleNotFound()
        {
            // Arrange
            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync((Role)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _rolePrivilegeService.GetPrivilegesByRoleIdAsync(1));
        }

        [Fact]
        public async Task GetPrivilegesByRoleIdAsync_Should_ReturnPrivilegesForRole_When_RoleExists()
        {
            // Arrange
            var privilege1 = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var privilege2 = TestDataBuilder.CreateTestPrivilege(2, "Delete");
            var role = TestDataBuilder.CreateTestRole(1);
            role.Privileges.Add(privilege1);
            role.Privileges.Add(privilege2);

            var privilegeDtos = new List<ReadPrivilegeDto>
            {
                new ReadPrivilegeDto { Id = 1, Name = "Create" },
                new ReadPrivilegeDto { Id = 2, Name = "Delete" }
            };

            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);
            MockMapper.Setup(x => x.Map<List<ReadPrivilegeDto>>(role.Privileges)).Returns(privilegeDtos);

            // Act
            var result = await _rolePrivilegeService.GetPrivilegesByRoleIdAsync(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetPrivilegesByRoleIdAsync_Should_ReturnEmptyList_When_RoleHasNoPrivileges()
        {
            // Arrange
            var role = TestDataBuilder.CreateTestRole(1);
            var privilegeDtos = new List<ReadPrivilegeDto>();

            _mockRolePrivilegeRepository.Setup(x => x.GetRoleWithPrivilegesAsync(1)).ReturnsAsync(role);
            MockMapper.Setup(x => x.Map<List<ReadPrivilegeDto>>(role.Privileges)).Returns(privilegeDtos);

            // Act
            var result = await _rolePrivilegeService.GetPrivilegesByRoleIdAsync(1);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion
    }
}
