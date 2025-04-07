using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.System.DTO;
using Application.System.Interface.IUnitOperation;
using Application.System.Utility;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace Application.System.Tests.Interface
{
    public class IAllUnitOperationTests
    {
        private readonly Mock<IAllUnitOperation> _mockUnitService;
        private readonly UnitDTO _sampleUnitDto;
        private readonly UnitWithBranchNameDTO _sampleUnitWithBranchDto;

        public IAllUnitOperationTests()
        {
            _mockUnitService = new Mock<IAllUnitOperation>();

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

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<UnitDTO>.Success(_sampleUnitDto, "Created");
            _mockUnitService.Setup(x => x.CreateAsync(It.IsAny<UnitDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.CreateAsync(_sampleUnitDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Created", result.Message);
            Assert.Equal(1, result.Data.Id_Unit);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenInvalid()
        {
            // Arrange
            var invalidDto = new UnitDTO { Name = "" };
            var expectedResponse = Response<UnitDTO>.Failure("Validation failed", "400");
            _mockUnitService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Validation failed", result.Message);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenUnitExists()
        {
            // Arrange
            var expectedResponse = Response.Success("Deleted");
            _mockUnitService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Deleted", result.Message);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUnits()
        {
            // Arrange
            var units = new List<UnitDTO> { _sampleUnitDto };
            var expectedResponse = Response<List<UnitDTO>>.Success(units, "Success");
            _mockUnitService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.GetAllAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
        }

        #endregion

        #region GetAllIncludeToBranchAsync Tests

        [Fact]
        public async Task GetAllIncludeToBranchAsync_ShouldReturnUnitsWithBranchInfo()
        {
            // Arrange
            var units = new List<UnitWithBranchNameDTO> { _sampleUnitWithBranchDto };
            var expectedResponse = Response<List<UnitWithBranchNameDTO>>.Success(units, "Success");
            _mockUnitService.Setup(x => x.GetAllIncludeToBranchAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.GetAllIncludeToBranchAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Branch", result.Data[0].BranchName);
        }

        #endregion

        #region GetAllUnitsByBranch Tests

        [Fact]
        public async Task GetAllUnitsByBranch_ShouldReturnBranchSpecificUnits()
        {
            // Arrange
            var units = new List<UnitWithBranchNameDTO> { _sampleUnitWithBranchDto };
            var expectedResponse = Response<List<UnitWithBranchNameDTO>>.Success(units, "Success");
            _mockUnitService.Setup(x => x.GetAllUnitsByBranch(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.GetAllUnitsByBranch(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data[0].Branch_Id);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUnit_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<UnitDTO>.Success(_sampleUnitDto, "Found");
            _mockUnitService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.GetByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Unit", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var expectedResponse = Response<UnitDTO>.Failure("Not found", "404");
            _mockUnitService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.GetByIdAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Not found", result.Message);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedUnit()
        {
            // Arrange
            var updatedDto = new UnitDTO
            {
                Id_Unit = 1,
                Name = "Updated Name",
                Branch_Id = 1
            };

            var expectedResponse = Response<UnitDTO>.Success(updatedDto, "Updated");
            _mockUnitService.Setup(x => x.UpdateAsync(updatedDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockUnitService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Name", result.Data.Name);
        }

        #endregion
    }
}
