using Application.System.DTO;
using Application.System.Interface.ICompanyOperation;
using Application.System.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Tests.Interface
{
    public class IAllCompanyOperationMockTests
    {
        private readonly Mock<IAllCompanyOperation> _mockCompanyService;
        private readonly CompanyDTO _sampleCompanyDto;

        public IAllCompanyOperationMockTests()
        {
            _mockCompanyService = new Mock<IAllCompanyOperation>();

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

        #region CRUD Operation Mocks

        [Fact]
        public async Task ShouldMockCreateAsync()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.CreateAsync(It.IsAny<CompanyDTO>()))
                .ReturnsAsync(Response<CompanyDTO>.Success(_sampleCompanyDto, "Created"));

            // Act
            var result = await _mockCompanyService.Object.CreateAsync(_sampleCompanyDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Company", result.Data.Name);
        }

        [Fact]
        public async Task ShouldMockGetByIdAsync()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<CompanyDTO>.Success(_sampleCompanyDto, "Found"));

            // Act
            var result = await _mockCompanyService.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(1, result.Data.Id_Company);
        }

        [Fact]
        public async Task ShouldMockGetAllAsync()
        {
            // Arrange
            var companies = new List<CompanyDTO> { _sampleCompanyDto };
            _mockCompanyService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<CompanyDTO>>.Success(companies, "Success"));

            // Act
            var result = await _mockCompanyService.Object.GetAllAsync();

            // Assert
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task ShouldMockUpdateAsync()
        {
            // Arrange
            var updatedDto = new CompanyDTO { Id_Company = 1, Name = "Updated Name" };
            _mockCompanyService.Setup(x => x.UpdateAsync(It.IsAny<CompanyDTO>()))
                .ReturnsAsync(Response<CompanyDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockCompanyService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.Equal("Updated Name", result.Data.Name);
        }

        [Fact]
        public async Task ShouldMockDeleteAsync()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockCompanyService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
        }

        #endregion

        #region Error Scenario Mocks

        [Fact]
        public async Task ShouldMockCreateAsyncFailure()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.CreateAsync(It.IsAny<CompanyDTO>()))
                .ReturnsAsync(Response<CompanyDTO>.Failure("Error", "400"));

            // Act
            var result = await _mockCompanyService.Object.CreateAsync(new CompanyDTO());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Status);
        }

        [Fact]
        public async Task ShouldMockGetByIdNotFound()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync(Response<CompanyDTO>.Failure("Not found", "404"));

            // Act
            var result = await _mockCompanyService.Object.GetByIdAsync(99);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("404", result.Status);
        }

        #endregion

        #region Advanced Mocking Scenarios

        [Fact]
        public async Task ShouldMockSequenceOfCalls()
        {
            // Arrange
            var firstResponse = Response<CompanyDTO>.Failure("Not found", "404");
            var secondResponse = Response<CompanyDTO>.Success(_sampleCompanyDto, "Found");

            _mockCompanyService.SetupSequence(x => x.GetByIdAsync(1))
                .ReturnsAsync(firstResponse)
                .ReturnsAsync(secondResponse);

            // Act
            var firstResult = await _mockCompanyService.Object.GetByIdAsync(1);
            var secondResult = await _mockCompanyService.Object.GetByIdAsync(1);

            // Assert
            Assert.False(firstResult.Succeeded);
            Assert.True(secondResult.Succeeded);
        }

        [Fact]
        public async Task ShouldVerifyMethodCalls()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.UpdateAsync(It.IsAny<CompanyDTO>()))
                .ReturnsAsync(Response<CompanyDTO>.Success(_sampleCompanyDto, "Updated"));

            // Act
            await _mockCompanyService.Object.UpdateAsync(_sampleCompanyDto);

            // Assert
            _mockCompanyService.Verify(x => x.UpdateAsync(_sampleCompanyDto), Times.Once);
            _mockCompanyService.Verify(x => x.CreateAsync(It.IsAny<CompanyDTO>()), Times.Never);
        }

        [Fact]
        public async Task ShouldMockException()
        {
            // Arrange
            _mockCompanyService.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _mockCompanyService.Object.GetAllAsync());
        }

        #endregion
    }
}
