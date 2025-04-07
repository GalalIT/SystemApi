using System;
using Application.System.DTO;
using Application.System.UseCace.CompanyUseCase.Interface;
using Application.System.Utility;
using Moq;
namespace Application.System.Tests.UseCace
{
    public class ICompanyUseCaseMockTests
    {
        private readonly Mock<ICompanyUseCase> _mockCompanyUseCase;
        private readonly CompanyDTO _sampleCompanyDto;

        public ICompanyUseCaseMockTests()
        {
            _mockCompanyUseCase = new Mock<ICompanyUseCase>();

            _sampleCompanyDto = new CompanyDTO
            {
                Id_Company = 1,
                Name = "Test Company",
                Description = "Test Description",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddYears(1),
                DiscountRate = 10
            };
        }

        #region CreateCompanyAsync Tests

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<CompanyDTO>.Success(_sampleCompanyDto, "Company created");
            _mockCompanyUseCase.Setup(x => x.CreateCompanyAsync(It.IsAny<CompanyDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.CreateCompanyAsync(_sampleCompanyDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Company", result.Data.Name);
            Assert.Equal("Company created", result.Message);
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnFailure_WhenNameMissing()
        {
            // Arrange
            var invalidDto = new CompanyDTO { Name = "", DiscountRate = 10 };
            var expectedResponse = Response<CompanyDTO>.Failure("Company name is required", "400");
            _mockCompanyUseCase.Setup(x => x.CreateCompanyAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.CreateCompanyAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Company name is required", result.Message);
        }

        [Fact]
        public async Task CreateCompanyAsync_ShouldReturnFailure_WhenInvalidDiscountRate()
        {
            // Arrange
            var invalidDto = new CompanyDTO { Name = "Test", DiscountRate = 150 };
            var expectedResponse = Response<CompanyDTO>.Failure("Discount rate must be between 0-100", "400");
            _mockCompanyUseCase.Setup(x => x.CreateCompanyAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.CreateCompanyAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Discount rate must be between 0-100", result.Message);
        }

        #endregion

        #region GetAllCompaniesAsync Tests

        [Fact]
        public async Task GetAllCompaniesAsync_ShouldReturnAllCompanies()
        {
            // Arrange
            var companies = new List<CompanyDTO> { _sampleCompanyDto };
            var expectedResponse = Response<List<CompanyDTO>>.Success(companies, "Companies retrieved");
            _mockCompanyUseCase.Setup(x => x.GetAllCompaniesAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.GetAllCompaniesAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
            Assert.Equal("Test Company", result.Data[0].Name);
        }

        #endregion

        #region DeleteCompanyAsync Tests

        [Fact]
        public async Task DeleteCompanyAsync_ShouldReturnSuccess_WhenExists()
        {
            // Arrange
            var expectedResponse = Response.Success("Company deleted");
            _mockCompanyUseCase.Setup(x => x.DeleteCompanyAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.DeleteCompanyAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Company deleted", result.Message);
        }

        [Fact]
        public async Task DeleteCompanyAsync_ShouldReturnFailure_WhenNotExists()
        {
            // Arrange
            var expectedResponse = Response.Failure("Company not found", "404");
            _mockCompanyUseCase.Setup(x => x.DeleteCompanyAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.DeleteCompanyAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Company not found", result.Message);
        }

        #endregion

        #region GetCompanyByIdAsync Tests

        [Fact]
        public async Task GetCompanyByIdAsync_ShouldReturnCompany_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<CompanyDTO>.Success(_sampleCompanyDto, "Company found");
            _mockCompanyUseCase.Setup(x => x.GetCompanyByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.GetCompanyByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Company", result.Data.Name);
        }

        [Fact]
        public async Task GetCompanyByIdAsync_ShouldReturnFailure_WhenNotExists()
        {
            // Arrange
            var expectedResponse = Response<CompanyDTO>.Failure("Company not found", "404");
            _mockCompanyUseCase.Setup(x => x.GetCompanyByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.GetCompanyByIdAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Company not found", result.Message);
        }

        #endregion

        #region UpdateCompanyAsync Tests

        [Fact]
        public async Task UpdateCompanyAsync_ShouldReturnUpdatedCompany_WhenValid()
        {
            // Arrange
            var updatedDto = new CompanyDTO
            {
                Id_Company = 1,
                Name = "Updated Company",
                Description = "Updated Description",
                DiscountRate = 15
            };

            var expectedResponse = Response<CompanyDTO>.Success(updatedDto, "Company updated");
            _mockCompanyUseCase.Setup(x => x.UpdateCompanyAsync(updatedDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.UpdateCompanyAsync(updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Company", result.Data.Name);
            Assert.Equal(15, result.Data.DiscountRate);
        }

        [Fact]
        public async Task UpdateCompanyAsync_ShouldReturnFailure_WhenNameMissing()
        {
            // Arrange
            var invalidDto = new CompanyDTO { Id_Company = 1, Name = "" };
            var expectedResponse = Response<CompanyDTO>.Failure("Company name is required", "400");
            _mockCompanyUseCase.Setup(x => x.UpdateCompanyAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.UpdateCompanyAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Company name is required", result.Message);
        }

        [Fact]
        public async Task UpdateCompanyAsync_ShouldReturnFailure_WhenInvalidDiscountRate()
        {
            // Arrange
            var invalidDto = new CompanyDTO
            {
                Id_Company = 1,
                Name = "Test",
                DiscountRate = -5
            };

            var expectedResponse = Response<CompanyDTO>.Failure("Discount rate must be between 0-100", "400");
            _mockCompanyUseCase.Setup(x => x.UpdateCompanyAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCompanyUseCase.Object.UpdateCompanyAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Discount rate must be between 0-100", result.Message);
        }

        #endregion
    }
}
