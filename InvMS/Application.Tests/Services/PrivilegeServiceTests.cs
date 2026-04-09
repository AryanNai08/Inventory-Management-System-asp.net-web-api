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
    public class PrivilegeServiceTests : ServiceTestFixture
    {
        private readonly Mock<IPrivilegeRepository> _mockPrivilegeRepository;
        private readonly PrivilegeService _privilegeService;

        public PrivilegeServiceTests()
        {
            _mockPrivilegeRepository = new Mock<IPrivilegeRepository>();

            _privilegeService = new PrivilegeService(
                _mockPrivilegeRepository.Object,
                MockMapper.Object,
                MockUnitOfWork.Object);
        }

        #region GetAllPrivilegesAsync Tests

        [Fact]
        public async Task GetAllPrivilegesAsync_Should_ReturnAllPrivileges()
        {
            // Arrange
            var privileges = new List<Privilege>
            {
                TestDataBuilder.CreateTestPrivilege(1, "Create"),
                TestDataBuilder.CreateTestPrivilege(2, "Delete")
            };
            var privilegeDtos = new List<ReadPrivilegeDto>
            {
                new ReadPrivilegeDto { Id = 1, Name = "Create" },
                new ReadPrivilegeDto { Id = 2, Name = "Delete" }
            };

            _mockPrivilegeRepository.Setup(x => x.GetAllPrivilegesAsync()).ReturnsAsync(privileges);
            MockMapper.Setup(x => x.Map<List<ReadPrivilegeDto>>(privileges)).Returns(privilegeDtos);

            // Act
            var result = await _privilegeService.GetAllPrivilegesAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        #endregion

        #region GetPrivilegeByIdAsync Tests

        [Fact]
        public async Task GetPrivilegeByIdAsync_Should_Throw_NotFoundException_When_PrivilegeNotFound()
        {
            // Arrange
            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync((Privilege)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _privilegeService.GetPrivilegeByIdAsync(1));
        }

        [Fact]
        public async Task GetPrivilegeByIdAsync_Should_ReturnPrivilege_When_PrivilegeExists()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var privilegeDto = new ReadPrivilegeDto { Id = 1, Name = "Create" };

            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            MockMapper.Setup(x => x.Map<ReadPrivilegeDto>(privilege)).Returns(privilegeDto);

            // Act
            var result = await _privilegeService.GetPrivilegeByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Create");
        }

        #endregion

        #region CreatePrivilegeAsync Tests

        [Fact]
        public async Task CreatePrivilegeAsync_Should_Throw_BadRequestException_When_PrivilegeAlreadyExists()
        {
            // Arrange
            var privilegeDto = new PrivilegeDto { Name = "Create", Description = "Create permission" };
            _mockPrivilegeRepository.Setup(x => x.PrivilegeExistsAsync("Create")).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _privilegeService.CreatePrivilegeAsync(privilegeDto));
        }

        [Fact]
        public async Task CreatePrivilegeAsync_Should_CreatePrivilege_When_NameIsUnique()
        {
            // Arrange
            var privilegeDto = new PrivilegeDto { Name = "Create", Description = "Create permission" };
            var privilege = new Privilege { Id = 1, Name = "Create", Description = "Create permission" };

            _mockPrivilegeRepository.Setup(x => x.PrivilegeExistsAsync("Create")).ReturnsAsync(false);
            _mockPrivilegeRepository.Setup(x => x.CreatePrivilegeAsync(It.IsAny<Privilege>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _privilegeService.CreatePrivilegeAsync(privilegeDto);

            // Assert
            _mockPrivilegeRepository.Verify(x => x.CreatePrivilegeAsync(It.IsAny<Privilege>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UpdatePrivilegeAsync Tests

        [Fact]
        public async Task UpdatePrivilegeAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Arrange
            var updateDto = new PrivilegeDto { Name = "Update", Description = "Update privilege" };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _privilegeService.UpdatePrivilegeAsync(0, updateDto));
        }

        [Fact]
        public async Task UpdatePrivilegeAsync_Should_Throw_NotFoundException_When_PrivilegeNotFound()
        {
            // Arrange
            var updateDto = new PrivilegeDto { Name = "Update", Description = "Update privilege" };
            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync((Privilege)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _privilegeService.UpdatePrivilegeAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdatePrivilegeAsync_Should_Throw_BadRequestException_When_NewNameAlreadyExists()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var updateDto = new PrivilegeDto { Name = "Delete", Description = "Delete privilege" };

            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            _mockPrivilegeRepository.Setup(x => x.PrivilegeExistsAsync("Delete")).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _privilegeService.UpdatePrivilegeAsync(1, updateDto));
        }

        [Fact]
        public async Task UpdatePrivilegeAsync_Should_UpdatePrivilege_When_NameIsUnique()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1, "Create");
            var updateDto = new PrivilegeDto { Name = "Update", Description = "Update privilege" };

            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            _mockPrivilegeRepository.Setup(x => x.PrivilegeExistsAsync("Update")).ReturnsAsync(false);
            _mockPrivilegeRepository.Setup(x => x.UpdatePrivilegeAsync(It.IsAny<Privilege>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
            MockMapper.Setup(x => x.Map(updateDto, privilege));

            // Act
            await _privilegeService.UpdatePrivilegeAsync(1, updateDto);

            // Assert
            _mockPrivilegeRepository.Verify(x => x.UpdatePrivilegeAsync(It.IsAny<Privilege>()), Times.Once);
        }

        #endregion

        #region DeletePrivilegeAsync Tests

        [Fact]
        public async Task DeletePrivilegeAsync_Should_Throw_BadRequestException_When_IdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _privilegeService.DeletePrivilegeAsync(0));
        }

        [Fact]
        public async Task DeletePrivilegeAsync_Should_Throw_NotFoundException_When_PrivilegeNotFound()
        {
            // Arrange
            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync((Privilege)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _privilegeService.DeletePrivilegeAsync(1));
        }

        [Fact]
        public async Task DeletePrivilegeAsync_Should_DeletePrivilege_When_PrivilegeExists()
        {
            // Arrange
            var privilege = TestDataBuilder.CreateTestPrivilege(1);
            _mockPrivilegeRepository.Setup(x => x.GetPrivilegeByIdAsync(1)).ReturnsAsync(privilege);
            _mockPrivilegeRepository.Setup(x => x.DeletePrivilegeAsync(privilege)).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _privilegeService.DeletePrivilegeAsync(1);

            // Assert
            _mockPrivilegeRepository.Verify(x => x.DeletePrivilegeAsync(privilege), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
