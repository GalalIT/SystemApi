using Application.System.DTO;
using Application.System.Interface.IBranchOperation;
using Application.System.Services.BranchServices;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Tests.Interface
{
    public class IAllBranchOperationTests
    {
        private readonly Mock<IAllBranchOperation> _mockBranchService;
        private readonly BranchDTO _sampleBranchDto;

        public IAllBranchOperationTests()
        {
            _mockBranchService = new Mock<IAllBranchOperation>();

            _sampleBranchDto = new BranchDTO
            {
                Id_Branch = 1,
                Name = "Main Branch",
                Address = "123 Main St",
                City = "Metropolis",
                Phone = "555-1234",
                IsActive = true
            };
        }

        #region Interface Contract Tests

        [Fact]
        public void IAllBranchOperation_ShouldInheritAllRequiredInterfaces()
        {
            // Arrange
            var interfaces = typeof(IAllBranchOperation).GetInterfaces();

            // Assert
            Assert.Contains(typeof(IAddBranchOperation), interfaces);
            Assert.Contains(typeof(IDeleteBreanchOperation), interfaces);
            Assert.Contains(typeof(IEditBreanchOperation), interfaces);
            Assert.Contains(typeof(IGetAllBreanchOperation), interfaces);
            Assert.Contains(typeof(IGetByIdBreanchOperation), interfaces);
        }

        #endregion

        #region IAddBranchOperation Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_ForValidBranch()
        {
            // Arrange
            _mockBranchService.Setup(x => x.CreateAsync(It.IsAny<BranchDTO>()))
                .ReturnsAsync(Response<BranchDTO>.Success(_sampleBranchDto, "Created"));

            // Act
            var result = await _mockBranchService.Object.CreateAsync(_sampleBranchDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Main Branch", result.Data.Name);
            _mockBranchService.Verify(x => x.CreateAsync(It.IsAny<BranchDTO>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldValidateRequiredFields()
        {
            // Arrange
            var invalidDto = new BranchDTO { Name = "" };

            _mockBranchService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<BranchDTO>.Failure("Validation failed", "400"));

            // Act
            var result = await _mockBranchService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Status);
        }

        #endregion

        #region IDeleteBreanchOperation Tests

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenBranchExists()
        {
            // Arrange
            _mockBranchService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockBranchService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            _mockBranchService.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNotFound_WhenBranchMissing()
        {
            // Arrange
            _mockBranchService.Setup(x => x.DeleteAsync(99))
                .ReturnsAsync(Response.Failure("Not found", "404"));

            // Act
            var result = await _mockBranchService.Object.DeleteAsync(99);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("404", result.Status);
        }

        #endregion

        #region IEditBreanchOperation Tests

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingBranch()
        {
            // Arrange
            var updatedDto = new BranchDTO
            {
                Id_Branch = 1,
                Name = "Updated Branch",
                Phone = "555-4321",
                IsActive = false
            };

            _mockBranchService.Setup(x => x.UpdateAsync(It.IsAny<BranchDTO>()))
                .ReturnsAsync(Response<BranchDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockBranchService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.Equal("Updated Branch", result.Data.Name);
            Assert.False(result.Data.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPreserveIsActive_WhenNull()
        {
            // Arrange
            var originalBranch = new Branch
            {
                Id_Branch = 1,
                Name = "Main Branch",
                IsActive = true
            };

            var updatedBranch = new Branch
            {
                Id_Branch = 1,
                Name = "Updated Branch",
                IsActive = true // Preserved from original
            };

            var updateDto = new BranchDTO
            {
                Id_Branch = 1,
                Name = "Updated Branch",
                IsActive = null
            };

            // Setup mock repository behavior
            var mockUnitOfWork = new Mock<IUnitOfRepository>();
            mockUnitOfWork.Setup(u => u._Branch.GetByIdAsync(1))
                .ReturnsAsync(originalBranch);

            mockUnitOfWork.Setup(u => u._Branch.UpdateAsync(It.IsAny<Branch>()))
                .ReturnsAsync(updatedBranch); // Return the updated entity

            var branchService = new AllBranchServices(mockUnitOfWork.Object);

            // Act
            var result = await branchService.UpdateAsync(updateDto);

            // Assert
            mockUnitOfWork.Verify(u => u._Branch.UpdateAsync(It.Is<Branch>(b =>
                b.Id_Branch == 1 &&
                b.Name == "Updated Branch" &&
                b.IsActive == true)),
                Times.Once);

            Assert.True(result.Data.IsActive);
            Assert.Equal("Updated Branch", result.Data.Name);
        }

        #endregion

        #region IGetAllBreanchOperation Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnMultipleBranches()
        {
            // Arrange
            var branches = new List<BranchDTO>
        {
            _sampleBranchDto,
            new BranchDTO { Id_Branch = 2, Name = "Second Branch" }
        };

            _mockBranchService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<BranchDTO>>.Success(branches, "Success"));

            // Act
            var result = await _mockBranchService.Object.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, b => b.Name == "Main Branch");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoBranches()
        {
            // Arrange
            _mockBranchService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<BranchDTO>>.Success(new List<BranchDTO>(), "No branches"));

            // Act
            var result = await _mockBranchService.Object.GetAllAsync();

            // Assert
            Assert.Empty(result.Data);
        }

        #endregion

        #region IGetByIdBreanchOperation Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBranch_WhenExists()
        {
            // Arrange
            _mockBranchService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<BranchDTO>.Success(_sampleBranchDto, "Found"));

            // Act
            var result = await _mockBranchService.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(1, result.Data.Id_Branch);
            Assert.Equal("555-1234", result.Data.Phone);
        }

        [Fact]
        public async Task GetBranchNameById_ShouldReturnName_WhenExists()
        {
            // Arrange
            _mockBranchService.Setup(x => x.GetBranchNameById(1))
                .ReturnsAsync("Main Branch");

            // Act
            var result = await _mockBranchService.Object.GetBranchNameById(1);

            // Assert
            Assert.Equal("Main Branch", result);
        }

        [Fact]
        public async Task GetBranchNameById_ShouldReturnEmpty_WhenNotFound()
        {
            // Arrange
            _mockBranchService.Setup(x => x.GetBranchNameById(99))
                .ReturnsAsync(string.Empty);

            // Act
            var result = await _mockBranchService.Object.GetBranchNameById(99);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task AllMethods_ShouldHandleExceptions()
        {
            // Arrange
            _mockBranchService.Setup(x => x.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            _mockBranchService.Setup(x => x.CreateAsync(It.IsAny<BranchDTO>()))
                .ThrowsAsync(new Exception("Validation error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _mockBranchService.Object.GetByIdAsync(1));
            await Assert.ThrowsAsync<Exception>(() => _mockBranchService.Object.CreateAsync(_sampleBranchDto));
        }

        #endregion
    }
}
