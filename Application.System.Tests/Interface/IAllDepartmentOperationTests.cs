using Application.System.DTO;
using Application.System.Interface.IDepartmentOperation;
using Application.System.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Tests.Interface
{
    public class IAllDepartmentOperationTests
    {
        private readonly Mock<IAllDepartmentOperation> _mockDepartmentService;
        private readonly DepartmentDTO _sampleDepartmentDto;
        private readonly DepartmentWithBranchDTO _sampleDepartmentWithBranchDto;

        public IAllDepartmentOperationTests()
        {
            _mockDepartmentService = new Mock<IAllDepartmentOperation>();

            _sampleDepartmentDto = new DepartmentDTO
            {
                Id_Department = 1,
                Name = "IT Department",
                Description = "Information Technology",
                Branch_Id = 1
            };

            _sampleDepartmentWithBranchDto = new DepartmentWithBranchDTO
            {
                Id_Department = 1,
                Name = "IT Department",
                Branch_Id = 1,
                BranchName = "Main Branch",
                BranchAddress = "123 Main St"
            };
        }

        #region CRUD Operation Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_ForValidDepartment()
        {
            // Arrange
            _mockDepartmentService.Setup(x => x.CreateAsync(It.IsAny<DepartmentDTO>()))
                .ReturnsAsync(Response<DepartmentDTO>.Success(_sampleDepartmentDto, "Created"));

            // Act
            var result = await _mockDepartmentService.Object.CreateAsync(_sampleDepartmentDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("IT Department", result.Data.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedDepartment()
        {
            // Arrange
            var updatedDto = new DepartmentDTO { Id_Department = 1, Name = "Updated IT Dept" };
            _mockDepartmentService.Setup(x => x.UpdateAsync(It.IsAny<DepartmentDTO>()))
                .ReturnsAsync(Response<DepartmentDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockDepartmentService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.Equal("Updated IT Dept", result.Data.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenDepartmentExists()
        {
            // Arrange
            _mockDepartmentService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockDepartmentService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDepartment_WhenExists()
        {
            // Arrange
            _mockDepartmentService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<DepartmentDTO>.Success(_sampleDepartmentDto, "Found"));

            // Act
            var result = await _mockDepartmentService.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(1, result.Data.Id_Department);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDepartments()
        {
            // Arrange
            var departments = new List<DepartmentDTO> { _sampleDepartmentDto };
            _mockDepartmentService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<DepartmentDTO>>.Success(departments, "Success"));

            // Act
            var result = await _mockDepartmentService.Object.GetAllAsync();

            // Assert
            Assert.Single(result.Data);
        }

        #endregion

        #region Specialized Method Tests

        [Fact]
        public async Task GetAllDepartmentIncludeToBranchAsync_ShouldReturnDepartmentsWithBranchInfo()
        {
            // Arrange
            var departments = new List<DepartmentWithBranchDTO> { _sampleDepartmentWithBranchDto };
            _mockDepartmentService.Setup(x => x.GetAllDepartmentIncludeToBranchAsync())
                .ReturnsAsync(Response<List<DepartmentWithBranchDTO>>.Success(departments, "Success"));

            // Act
            var result = await _mockDepartmentService.Object.GetAllDepartmentIncludeToBranchAsync();

            // Assert
            Assert.Equal("Main Branch", result.Data.First().BranchName);
        }

        [Fact]
        public async Task GetAllDepartmentsByUserBranchAsync_ShouldFilterByBranch()
        {
            // Arrange
            var departments = new List<DepartmentWithBranchDTO> { _sampleDepartmentWithBranchDto };
            _mockDepartmentService.Setup(x => x.GetAllDepartmentsByUserBranchAsync(1))
                .ReturnsAsync(Response<List<DepartmentWithBranchDTO>>.Success(departments, "Success"));

            // Act
            var result = await _mockDepartmentService.Object.GetAllDepartmentsByUserBranchAsync(1);

            // Assert
            Assert.Single(result.Data);
            Assert.Equal(1, result.Data.First().Branch_Id);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnFailure_WhenNameIsEmpty()
        {
            // Arrange
            var invalidDto = new DepartmentDTO { Name = "", Branch_Id = 1 };
            _mockDepartmentService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<DepartmentDTO>.Failure("Name is required", "400"));

            // Act
            var result = await _mockDepartmentService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Status);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenDepartmentMissing()
        {
            // Arrange
            _mockDepartmentService.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync(Response<DepartmentDTO>.Failure("Not found", "404"));

            // Act
            var result = await _mockDepartmentService.Object.GetByIdAsync(99);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("404", result.Status);
        }

        #endregion

        #region Mock Verification Tests

        [Fact]
        public async Task ShouldVerifyUpdateAsyncWasCalled()
        {
            // Arrange
            var updateDto = new DepartmentDTO { Id_Department = 1, Name = "Updated" };
            _mockDepartmentService.Setup(x => x.UpdateAsync(updateDto))
                .ReturnsAsync(Response<DepartmentDTO>.Success(updateDto, "Updated"));

            // Act
            await _mockDepartmentService.Object.UpdateAsync(updateDto);

            // Assert
            _mockDepartmentService.Verify(x => x.UpdateAsync(updateDto), Times.Once);
        }

        [Fact]
        public async Task ShouldVerifyGetAllDepartmentsByUserBranchAsyncWasCalled()
        {
            // Arrange
            _mockDepartmentService.Setup(x => x.GetAllDepartmentsByUserBranchAsync(1))
                .ReturnsAsync(Response<List<DepartmentWithBranchDTO>>.Success(
                    new List<DepartmentWithBranchDTO>(), "Success"));

            // Act
            await _mockDepartmentService.Object.GetAllDepartmentsByUserBranchAsync(1);

            // Assert
            _mockDepartmentService.Verify(x => x.GetAllDepartmentsByUserBranchAsync(1), Times.Once);
        }

        #endregion
    }
}
