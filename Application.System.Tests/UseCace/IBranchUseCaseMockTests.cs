using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.System.DTO;
using Application.System.UseCace.BranchUseCase.Interface;
using Application.System.Utility;
using Moq;
namespace Application.System.Tests.UseCace
{
    public class IBranchUseCaseMockTests
    {
        private readonly Mock<IBranchUseCase> _mockBranchUseCase;
        private readonly BranchDTO _sampleBranchDto;

        public IBranchUseCaseMockTests()
        {
            _mockBranchUseCase = new Mock<IBranchUseCase>();

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

        #region CreateBranchAsync Tests

        [Fact]
        public async Task CreateBranchAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<BranchDTO>.Success(_sampleBranchDto, "Branch created");
            _mockBranchUseCase.Setup(x => x.CreateBranchAsync(It.IsAny<BranchDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.CreateBranchAsync(_sampleBranchDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Main Branch", result.Data.Name);
            Assert.Equal("Branch created", result.Message);
        }

        [Fact]
        public async Task CreateBranchAsync_ShouldReturnFailure_WhenInvalid()
        {
            // Arrange
            var invalidDto = new BranchDTO { Name = "" }; // Missing required fields
            var expectedResponse = Response<BranchDTO>.Failure("Validation failed", "400");
            _mockBranchUseCase.Setup(x => x.CreateBranchAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.CreateBranchAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Validation failed", result.Message);
        }

        #endregion

        #region GetAllBranchesAsync Tests

        [Fact]
        public async Task GetAllBranchesAsync_ShouldReturnAllBranches()
        {
            // Arrange
            var branches = new List<BranchDTO> { _sampleBranchDto };
            var expectedResponse = Response<List<BranchDTO>>.Success(branches, "Branches retrieved");
            _mockBranchUseCase.Setup(x => x.GetAllBranchesAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.GetAllBranchesAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
            Assert.Equal("Main Branch", result.Data[0].Name);
        }

        #endregion

        #region DeleteBranchAsync Tests

        [Fact]
        public async Task DeleteBranchAsync_ShouldReturnSuccess_WhenExists()
        {
            // Arrange
            var expectedResponse = Response.Success("Branch deleted");
            _mockBranchUseCase.Setup(x => x.DeleteBranchAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.DeleteBranchAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Branch deleted", result.Message);
        }

        [Fact]
        public async Task DeleteBranchAsync_ShouldReturnFailure_WhenNotExists()
        {
            // Arrange
            var expectedResponse = Response.Failure("Branch not found", "404");
            _mockBranchUseCase.Setup(x => x.DeleteBranchAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.DeleteBranchAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Branch not found", result.Message);
        }

        #endregion

        #region GetBranchByIdAsync Tests

        [Fact]
        public async Task GetBranchByIdAsync_ShouldReturnBranch_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<BranchDTO>.Success(_sampleBranchDto, "Branch found");
            _mockBranchUseCase.Setup(x => x.GetBranchByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.GetBranchByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Main Branch", result.Data.Name);
        }

        [Fact]
        public async Task GetBranchByIdAsync_ShouldReturnFailure_WhenNotExists()
        {
            // Arrange
            var expectedResponse = Response<BranchDTO>.Failure("Branch not found", "404");
            _mockBranchUseCase.Setup(x => x.GetBranchByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.GetBranchByIdAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Branch not found", result.Message);
        }

        #endregion

        #region GetBranchNameByIdAsync Tests

        [Fact]
        public async Task GetBranchNameByIdAsync_ShouldReturnName_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<string>.Success("Main Branch", "Name retrieved");
            _mockBranchUseCase.Setup(x => x.GetBranchNameByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.GetBranchNameByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Main Branch", result.Data);
        }

        #endregion

        #region UpdateBranchAsync Tests

        [Fact]
        public async Task UpdateBranchAsync_ShouldReturnUpdatedBranch_WhenValid()
        {
            // Arrange
            var updatedDto = new BranchDTO
            {
                Id_Branch = 1,
                Name = "Updated Branch Name",
                Address = "123 Main St",
                City = "Metropolis",
                Phone = "555-1234",
                IsActive = true
            };

            var expectedResponse = Response<BranchDTO>.Success(updatedDto, "Branch updated");
            _mockBranchUseCase.Setup(x => x.UpdateBranchAsync(updatedDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.UpdateBranchAsync(updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Branch Name", result.Data.Name);
        }

        [Fact]
        public async Task UpdateBranchAsync_ShouldReturnFailure_WhenInvalid()
        {
            // Arrange
            var invalidDto = new BranchDTO { Name = "" }; // Missing required name
            var expectedResponse = Response<BranchDTO>.Failure("Validation failed", "400");
            _mockBranchUseCase.Setup(x => x.UpdateBranchAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockBranchUseCase.Object.UpdateBranchAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Validation failed", result.Message);
        }

        #endregion
    }
}
