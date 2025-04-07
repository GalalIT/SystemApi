using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.System.DTO;
using Application.System.UseCace.UnitUseCase.Interface;
using Application.System.Utility;
using Moq;

namespace Application.System.Tests.UseCace
{
    
    public class IUnitUseCaseMockTests
    {
        private readonly Mock<IUnitUseCase> _mockUnitUseCase;
        private readonly UnitDTO _sampleUnitDto;
        private readonly UnitWithBranchNameDTO _sampleUnitWithBranchDto;

        public IUnitUseCaseMockTests()
        {
            _mockUnitUseCase = new Mock<IUnitUseCase>();

            _sampleUnitDto = new UnitDTO
            {
                Id_Unit = 1,
                Name = "Test Unit",
                Branch_Id = 1
            };

            _sampleUnitWithBranchDto = new UnitWithBranchNameDTO
            {
                Id_Unit = 1,
                Name = "Test Unit",
                Branch_Id = 1,
                BranchName = "Test Branch"
            };
        }

        #region CreateUnitAsync Tests

        [Fact]
        public async Task CreateUnitAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<UnitDTO>.Success(_sampleUnitDto, "Unit created");
            _mockUnitUseCase.Setup(x => x.CreateUnitAsync(It.IsAny<UnitDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.CreateUnitAsync(_sampleUnitDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Unit", result.Data.Name);
        }

        [Fact]
        public async Task CreateUnitAsync_ShouldReturnFailure_WhenNameMissing()
        {
            // Arrange
            var invalidDto = new UnitDTO { Name = "", Branch_Id = 1 };
            var expectedResponse = Response<UnitDTO>.Failure("Unit name is required", "400");
            _mockUnitUseCase.Setup(x => x.CreateUnitAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.CreateUnitAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Unit name is required", result.Message);
        }

        [Fact]
        public async Task CreateUnitAsync_ShouldReturnFailure_WhenInvalidBranch()
        {
            // Arrange
            var invalidDto = new UnitDTO { Name = "Test", Branch_Id = 0 };
            var expectedResponse = Response<UnitDTO>.Failure("Branch ID is invalid", "400");
            _mockUnitUseCase.Setup(x => x.CreateUnitAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.CreateUnitAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Branch ID is invalid", result.Message);
        }

        #endregion

        #region DeleteUnitAsync Tests

        [Fact]
        public async Task DeleteUnitAsync_ShouldReturnSuccess_WhenExists()
        {
            // Arrange
            var expectedResponse = Response.Success("Unit deleted");
            _mockUnitUseCase.Setup(x => x.DeleteUnitAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.DeleteUnitAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Unit deleted", result.Message);
        }

        [Fact]
        public async Task DeleteUnitAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var expectedResponse = Response.Failure("Unit not found", "404");
            _mockUnitUseCase.Setup(x => x.DeleteUnitAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.DeleteUnitAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Unit not found", result.Message);
        }

        #endregion

        #region GetAllUnitsAsync Tests

        [Fact]
        public async Task GetAllUnitsAsync_ShouldReturnAllUnits()
        {
            // Arrange
            var units = new List<UnitDTO> { _sampleUnitDto };
            var expectedResponse = Response<List<UnitDTO>>.Success(units, "Units retrieved");
            _mockUnitUseCase.Setup(x => x.GetAllUnitsAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.GetAllUnitsAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
        }

        #endregion

        #region GetAllUnitsIncludeToBranchAsync Tests

        [Fact]
        public async Task GetAllUnitsIncludeToBranchAsync_ShouldReturnUnitsWithBranchInfo()
        {
            // Arrange
            var units = new List<UnitWithBranchNameDTO> { _sampleUnitWithBranchDto };
            var expectedResponse = Response<List<UnitWithBranchNameDTO>>.Success(units, "Success");
            _mockUnitUseCase.Setup(x => x.GetAllUnitsIncludeToBranchAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.GetAllUnitsIncludeToBranchAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Branch", result.Data[0].BranchName);
        }

        #endregion

        #region GetAllUnitsByBranchAsync Tests

        [Fact]
        public async Task GetAllUnitsByBranchAsync_ShouldReturnUnits_WhenValidBranch()
        {
            // Arrange
            var units = new List<UnitWithBranchNameDTO> { _sampleUnitWithBranchDto };
            var expectedResponse = Response<List<UnitWithBranchNameDTO>>.Success(units, "Success");
            _mockUnitUseCase.Setup(x => x.GetAllUnitsByBranchAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.GetAllUnitsByBranchAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data[0].Branch_Id);
        }

        [Fact]
        public async Task GetAllUnitsByBranchAsync_ShouldReturnFailure_WhenInvalidBranch()
        {
            // Arrange
            var expectedResponse = Response<List<UnitWithBranchNameDTO>>.Failure("Invalid branch ID", "400");
            _mockUnitUseCase.Setup(x => x.GetAllUnitsByBranchAsync(0))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.GetAllUnitsByBranchAsync(0);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid branch ID", result.Message);
        }

        #endregion

        #region GetUnitByIdAsync Tests

        [Fact]
        public async Task GetUnitByIdAsync_ShouldReturnUnit_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<UnitDTO>.Success(_sampleUnitDto, "Unit found");
            _mockUnitUseCase.Setup(x => x.GetUnitByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.GetUnitByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Unit", result.Data.Name);
        }

        [Fact]
        public async Task GetUnitByIdAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var expectedResponse = Response<UnitDTO>.Failure("Unit not found", "404");
            _mockUnitUseCase.Setup(x => x.GetUnitByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.GetUnitByIdAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Unit not found", result.Message);
        }

        #endregion

        #region UpdateUnitAsync Tests

        [Fact]
        public async Task UpdateUnitAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var updatedDto = new UnitDTO
            {
                Id_Unit = 1,
                Name = "Updated Unit",
                Branch_Id = 1
            };

            var expectedResponse = Response<UnitDTO>.Success(updatedDto, "Unit updated");
            _mockUnitUseCase.Setup(x => x.UpdateUnitAsync(updatedDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.UpdateUnitAsync(updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Unit", result.Data.Name);
        }

        [Fact]
        public async Task UpdateUnitAsync_ShouldReturnFailure_WhenNameMissing()
        {
            // Arrange
            var invalidDto = new UnitDTO { Id_Unit = 1, Name = "", Branch_Id = 1 };
            var expectedResponse = Response<UnitDTO>.Failure("Unit name is required", "400");
            _mockUnitUseCase.Setup(x => x.UpdateUnitAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitUseCase.Object.UpdateUnitAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Unit name is required", result.Message);
        }

        #endregion
    }
}
