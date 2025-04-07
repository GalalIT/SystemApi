using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.System.DTO;
using Application.System.UseCace.ProductUseCase.Interface;
using Application.System.Utility;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
namespace Application.System.Tests.UseCace
{
    public class IProductUseCaseMockTests
    {
        private readonly Mock<IProductUseCase> _mockProductUseCase;
        private readonly ProductDTO _sampleProductDto;
        private readonly CreateProductWithUnitsDTO _sampleCreateWithUnitsDto;
        private readonly ProductBranchResponse _sampleBranchResponse;
        private readonly ProductWithDetailsDto _sampleProductWithDetailsDto;

        public IProductUseCaseMockTests()
        {
            _mockProductUseCase = new Mock<IProductUseCase>();

            _sampleProductDto = new ProductDTO
            {
                Id_Product = 1,
                Name = "Test Product",
                Department_Id = 1,
                Price = 10.99m,
                IsActive = true
            };

            _sampleCreateWithUnitsDto = new CreateProductWithUnitsDTO
            {
                Name = "Test Product with Units",
                Department_Id = 1,
                Price = 10.99m,
                UnitIds = new List<int> { 1, 2 },
                SpecialPrices = new List<decimal> { 9.99m, 8.99m }
            };

            _sampleBranchResponse = new ProductBranchResponse
            {
                Products = new List<ProductDTO> { _sampleProductDto },
                Departments = new List<DepartmentDTO>
            {
                new DepartmentDTO { Id_Department = 1, Name = "Test Dept" }
            }
            };

            _sampleProductWithDetailsDto = new ProductWithDetailsDto
            {
                Id = 1,
                Name = "Test Product",
                Price = 10.99m,
                Department = new DepartmentWithBranchDTO
                {
                    Id_Department = 1,
                    Name = "Test Dept"
                },
                Units = new List<ProductUnitDTO>
            {
                new ProductUnitDTO { UnitId = 1, SpecialPrice = 9.99m }
            }
            };
        }

        #region GetProductByIdAsync Tests

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<ProductDTO>.Success(_sampleProductDto, "Product found");
            _mockProductUseCase.Setup(x => x.GetProductByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.GetProductByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Product", result.Data.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnFailure_WhenNotExists()
        {
            // Arrange
            var expectedResponse = Response<ProductDTO>.Failure("Product not found", "404");
            _mockProductUseCase.Setup(x => x.GetProductByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.GetProductByIdAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Product not found", result.Message);
        }

        #endregion

        #region DeleteProductsAsync Tests

        [Fact]
        public async Task DeleteProductsAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response.Success("Product deleted");
            _mockProductUseCase.Setup(x => x.DeleteProductsAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.DeleteProductsAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Product deleted", result.Message);
        }

        [Fact]
        public async Task DeleteProductsAsync_ShouldReturnFailure_WhenInOrders()
        {
            // Arrange
            var expectedResponse = Response.Failure("Cannot delete product that exists in orders", "400");
            _mockProductUseCase.Setup(x => x.DeleteProductsAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.DeleteProductsAsync(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Cannot delete product that exists in orders", result.Message);
        }

        #endregion

        #region GetProductsByBranchWithDepartments Tests

        [Fact]
        public async Task GetProductsByBranchWithDepartments_ShouldReturnProducts_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<ProductBranchResponse>.Success(_sampleBranchResponse, "Success");
            _mockProductUseCase.Setup(x => x.GetProductsByBranchWithDepartments(1, null))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.GetProductsByBranchWithDepartments(1, null);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data.Products);
            Assert.Single(result.Data.Departments);
        }

        [Fact]
        public async Task GetProductsByBranchWithDepartments_ShouldReturnFailure_WhenInvalidBranch()
        {
            // Arrange
            var expectedResponse = Response<ProductBranchResponse>.Failure("Invalid branch ID", "400");
            _mockProductUseCase.Setup(x => x.GetProductsByBranchWithDepartments(0, null))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.GetProductsByBranchWithDepartments(0, null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid branch ID", result.Message);
        }

        #endregion

        #region UpdateProductWithUnitsAsync Tests

        [Fact]
        public async Task UpdateProductWithUnitsAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<ProductDTO>.Success(_sampleProductDto, "Product updated");
            _mockProductUseCase.Setup(x => x.UpdateProductWithUnitsAsync(1, _sampleCreateWithUnitsDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.UpdateProductWithUnitsAsync(1, _sampleCreateWithUnitsDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Product", result.Data.Name);
        }

        [Fact]
        public async Task UpdateProductWithUnitsAsync_ShouldReturnFailure_WhenMissingName()
        {
            // Arrange
            var invalidDto = new CreateProductWithUnitsDTO { Name = "" };
            var expectedResponse = Response<ProductDTO>.Failure("Product name is required", "400");
            _mockProductUseCase.Setup(x => x.UpdateProductWithUnitsAsync(1, invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.UpdateProductWithUnitsAsync(1, invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Product name is required", result.Message);
        }

        #endregion

        #region CreateProductWithUnitsAsync Tests

        [Fact]
        public async Task CreateProductWithUnitsAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<ProductDTO>.Success(_sampleProductDto, "Product created");
            _mockProductUseCase.Setup(x => x.CreateProductWithUnitsAsync(_sampleCreateWithUnitsDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.CreateProductWithUnitsAsync(_sampleCreateWithUnitsDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data.Id_Product);
        }

        [Fact]
        public async Task CreateProductWithUnitsAsync_ShouldReturnFailure_WhenNoUnits()
        {
            // Arrange
            var invalidDto = new CreateProductWithUnitsDTO
            {
                Name = "Test",
                Department_Id = 1,
                Price = 10,
                UnitIds = new List<int>(),
                SpecialPrices = new List<decimal>()
            };

            var expectedResponse = Response<ProductDTO>.Failure("At least one unit must be specified", "400");
            _mockProductUseCase.Setup(x => x.CreateProductWithUnitsAsync(invalidDto))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.CreateProductWithUnitsAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("At least one unit must be specified", result.Message);
        }

        #endregion

        #region GetAllProductsWithDetailsAsync Tests

        [Fact]
        public async Task GetAllProductsWithDetailsAsync_ShouldReturnProductsWithDetails()
        {
            // Arrange
            var products = new List<ProductWithDetailsDto> { _sampleProductWithDetailsDto };
            var expectedResponse = Response<List<ProductWithDetailsDto>>.Success(products, "Success");
            _mockProductUseCase.Setup(x => x.GetAllProductsWithDetailsAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductUseCase.Object.GetAllProductsWithDetailsAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Product", result.Data[0].Name);
            Assert.Equal("Test Dept", result.Data[0].Department.Name);
            Assert.Single(result.Data[0].Units);
        }

        #endregion
    }
}
