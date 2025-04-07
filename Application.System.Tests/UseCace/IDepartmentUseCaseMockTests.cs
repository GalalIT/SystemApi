using Application.System.DTO;
using Application.System.UseCace.DepartmentUseCase.Interface;
using Application.System.Utility;
using Moq;
namespace Application.System.Tests.UseCace
{
    public class IDepartmentUseCaseMockTests
    {
        private readonly Mock<IDepartmentUseCase> _mockDepartmentUseCase;
        private readonly DepartmentDTO _sampleDepartmentDto;
        private readonly DepartmentWithBranchDTO _sampleDepartmentWithBranchDto;

        public IDepartmentUseCaseMockTests()
        {
            _mockDepartmentUseCase = new Mock<IDepartmentUseCase>();

            _sampleDepartmentDto = new DepartmentDTO
            {
                Id_Department = 1,
                Name = "Test Department",
                Description = "Test Description",
                Branch_Id = 1
            };

            _sampleDepartmentWithBranchDto = new DepartmentWithBranchDTO
            {
                Id_Department = 1,
                Name = "Test Department",
                Description = "Test Description",
                Branch_Id = 1,
                BranchName = "Test Branch",
                BranchAddress = "123 Test St"
            };
        }

        #region CreateDepartment Tests

        [Fact]
        public async Task CreateDepartment_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<DepartmentDTO>.Success(_sampleDepartmentDto, "Department created");
            _mockDepartmentUseCase.Setup(x => x.CreateDepartment(It.IsAny<DepartmentDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.CreateDepartment(_sampleDepartmentDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Department", result.Data.Name);
            Assert.Equal(1, result.Data.Branch_Id);
        }

        [Fact]
        public async Task CreateDepartment_ShouldReturnFailure_WhenBranchNotExists()
        {
            // Arrange
            var expectedResponse = Response<DepartmentDTO>.Failure("Specified branch does not exist", "400");
            _mockDepartmentUseCase.Setup(x => x.CreateDepartment(It.IsAny<DepartmentDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.CreateDepartment(_sampleDepartmentDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Specified branch does not exist", result.Message);
        }

        #endregion

        #region UpdateDepartment Tests

        [Fact]
        public async Task UpdateDepartment_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var updatedDto = new DepartmentDTO
            {
                Id_Department = 1,
                Name = "Updated Department",
                Branch_Id = 1
            };

            var expectedResponse = Response<DepartmentDTO>.Success(updatedDto, "Department updated");
            _mockDepartmentUseCase.Setup(x => x.UpdateDepartment(updatedDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.UpdateDepartment(updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Department", result.Data.Name);
        }

        #endregion

        #region DeleteDepartment Tests

        [Fact]
        public async Task DeleteDepartment_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response.Success("Department deleted");
            _mockDepartmentUseCase.Setup(x => x.DeleteDepartment(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.DeleteDepartment(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Department deleted", result.Message);
        }

        [Fact]
        public async Task DeleteDepartment_ShouldReturnFailure_WhenHasProducts()
        {
            // Arrange
            var expectedResponse = Response.Failure("Cannot delete department with associated products or users", "400");
            _mockDepartmentUseCase.Setup(x => x.DeleteDepartment(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.DeleteDepartment(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Cannot delete department with associated products or users", result.Message);
        }

        #endregion

        #region GetDepartment Tests

        [Fact]
        public async Task GetDepartment_ShouldReturnDepartment_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<DepartmentDTO>.Success(_sampleDepartmentDto, "Department found");
            _mockDepartmentUseCase.Setup(x => x.GetDepartment(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.GetDepartment(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Department", result.Data.Name);
        }

        [Fact]
        public async Task GetDepartment_ShouldReturnFailure_WhenNotExists()
        {
            // Arrange
            var expectedResponse = Response<DepartmentDTO>.Failure("Department not found", "404");
            _mockDepartmentUseCase.Setup(x => x.GetDepartment(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.GetDepartment(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Department not found", result.Message);
        }

        #endregion

        #region GetAllDepartments Tests

        [Fact]
        public async Task GetAllDepartments_ShouldReturnAllDepartments()
        {
            // Arrange
            var departments = new List<DepartmentDTO> { _sampleDepartmentDto };
            var expectedResponse = Response<List<DepartmentDTO>>.Success(departments, "Departments retrieved");
            _mockDepartmentUseCase.Setup(x => x.GetAllDepartments())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.GetAllDepartments();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
        }

        #endregion

        #region GetAllDepartmentsByUserBranch Tests

        [Fact]
        public async Task GetAllDepartmentsByUserBranch_ShouldReturnDepartments_WhenValidBranch()
        {
            // Arrange
            var departments = new List<DepartmentWithBranchDTO> { _sampleDepartmentWithBranchDto };
            var expectedResponse = Response<List<DepartmentWithBranchDTO>>.Success(departments, "Departments retrieved");
            _mockDepartmentUseCase.Setup(x => x.GetAllDepartmentsByUserBranch(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.GetAllDepartmentsByUserBranch(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Branch", result.Data[0].BranchName);
        }

        [Fact]
        public async Task GetAllDepartmentsByUserBranch_ShouldReturnFailure_WhenInvalidBranch()
        {
            // Arrange
            var expectedResponse = Response<List<DepartmentWithBranchDTO>>.Failure("Invalid branch ID", "400");
            _mockDepartmentUseCase.Setup(x => x.GetAllDepartmentsByUserBranch(0))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.GetAllDepartmentsByUserBranch(0);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid branch ID", result.Message);
        }

        #endregion

        #region GetAllDepartmentsWithBranchInfo Tests

        [Fact]
        public async Task GetAllDepartmentsWithBranchInfo_ShouldReturnDepartmentsWithBranchDetails()
        {
            // Arrange
            var departments = new List<DepartmentWithBranchDTO> { _sampleDepartmentWithBranchDto };
            var expectedResponse = Response<List<DepartmentWithBranchDTO>>.Success(departments, "Departments with branch info retrieved");
            _mockDepartmentUseCase.Setup(x => x.GetAllDepartmentsWithBranchInfo())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockDepartmentUseCase.Object.GetAllDepartmentsWithBranchInfo();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Branch", result.Data[0].BranchName);
            Assert.Equal("123 Test St", result.Data[0].BranchAddress);
        }

        #endregion

        #region AnyProductsInDepartmentAsync Tests

        [Fact]
        public async Task AnyProductsInDepartmentAsync_ShouldReturnTrue_WhenProductsExist()
        {
            // Arrange
            _mockDepartmentUseCase.Setup(x => x.AnyProductsInDepartmentAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _mockDepartmentUseCase.Object.AnyProductsInDepartmentAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AnyProductsInDepartmentAsync_ShouldReturnFalse_WhenNoProducts()
        {
            // Arrange
            _mockDepartmentUseCase.Setup(x => x.AnyProductsInDepartmentAsync(1))
                .ReturnsAsync(false);

            // Act
            var result = await _mockDepartmentUseCase.Object.AnyProductsInDepartmentAsync(1);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
