using Application.System.DTO;
using Application.System.Interface.IProduct_UnitOperation;
using Application.System.Services.ProductServices;
using Application.System.Utility;
using Moq;
using Application.System.Interface.IProductOperation;
namespace Application.System.Tests.Interface
{
    public class IAllProductOperationTests
    {
        private readonly Mock<IAllProductOperation> _mockProductService;
        private readonly ProductDTO _sampleProductDto;
        private readonly CreateProductWithUnitsDTO _sampleCreateWithUnitsDto;

        public IAllProductOperationTests()
        {
            _mockProductService = new Mock<IAllProductOperation>();

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
                IsActive = true,
                UnitIds = new List<int> { 1, 2 },
                SpecialPrices = new List<decimal> { 9.99m, 8.99m }
            };
        }

        #region CreateProductWithUnitsAsync Tests

        [Fact]
        public async Task CreateProductWithUnitsAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<ProductDTO>.Success(_sampleProductDto, "Created");
            _mockProductService.Setup(x => x.CreateProductWithUnitsAsync(It.IsAny<CreateProductWithUnitsDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductService.Object.CreateProductWithUnitsAsync(_sampleCreateWithUnitsDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Created", result.Message);
            Assert.Equal(1, result.Data.Id_Product);
        }

        [Fact]
        public async Task CreateProductWithUnitsAsync_ShouldReturnFailure_WhenInvalid()
        {
            // Arrange
            var invalidDto = new CreateProductWithUnitsDTO { Name = "" };
            var expectedResponse = Response<ProductDTO>.FailureAsync("Validation failed", "400");
            _mockProductService.Setup(x => x.CreateProductWithUnitsAsync(invalidDto))
                .Returns(expectedResponse);

            // Act
            var result = await _mockProductService.Object.CreateProductWithUnitsAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Validation failed", result.Message);
        }

        
        [Fact]
        public async Task UpdateProductWithUnitsAsync_ShouldReturnUpdatedProduct()
        {
            // Arrange
            var updatedDto = new CreateProductWithUnitsDTO
            {
                Name = "Updated Name",
                Department_Id = _sampleCreateWithUnitsDto.Department_Id,
                Price = _sampleCreateWithUnitsDto.Price,
                IsActive = _sampleCreateWithUnitsDto.IsActive,
                UnitIds = new List<int>(_sampleCreateWithUnitsDto.UnitIds),
                SpecialPrices = new List<decimal>(_sampleCreateWithUnitsDto.SpecialPrices)
            };

            var expectedProductDto = new ProductDTO
            {
                Id_Product = _sampleProductDto.Id_Product,
                Name = "Updated Name",
                Department_Id = _sampleProductDto.Department_Id,
                Price = _sampleProductDto.Price,
                IsActive = _sampleProductDto.IsActive
            };

            var expectedResponse = Response<ProductDTO>.Success(expectedProductDto, "Updated");

            _mockProductService.Setup(x => x.UpdateProductWithUnitsAsync(
                It.IsAny<int>(),
                It.Is<CreateProductWithUnitsDTO>(d =>
                    d.Name == "Updated Name" &&
                    d.Department_Id == _sampleCreateWithUnitsDto.Department_Id
                )))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockProductService.Object.UpdateProductWithUnitsAsync(1, updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Name", result.Data.Name);
            Assert.Equal(_sampleProductDto.Department_Id, result.Data.Department_Id);
            Assert.Equal(_sampleProductDto.Price, result.Data.Price);
        }
        #endregion

        #region GetProductsByBranchWithDepartments Tests

        [Fact]
        public async Task GetProductsByBranchWithDepartments_ShouldReturnProductsAndDepartments()
        {
            // Arrange
            var expectedResponse = new ProductBranchResponse
            {
                Products = new List<ProductDTO> { _sampleProductDto },
                Departments = new List<DepartmentDTO>
            {
                new DepartmentDTO { Id_Department = 1, Name = "Test Dept" }
            }
            };

            _mockProductService.Setup(x => x.GetProductsByBranchWithDepartments(1, null))
                .ReturnsAsync(Response<ProductBranchResponse>.Success(expectedResponse, "Success"));

            // Act
            var result = await _mockProductService.Object.GetProductsByBranchWithDepartments(1, null);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data.Products);
            Assert.Single(result.Data.Departments);
        }

        #endregion

        #region Basic CRUD Operation Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedProduct()
        {
            // Arrange
            _mockProductService.Setup(x => x.CreateAsync(_sampleProductDto))
                .ReturnsAsync(Response<ProductDTO>.Success(_sampleProductDto, "Created"));

            // Act
            var result = await _mockProductService.Object.CreateAsync(_sampleProductDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Test Product", result.Data.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            _mockProductService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<ProductDTO>.Success(_sampleProductDto, "Found"));

            // Act
            var result = await _mockProductService.Object.GetByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data.Id_Product);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedProduct()
        {
            // Arrange
            var updatedDto = new ProductDTO
            {
                Id_Product = _sampleProductDto.Id_Product,
                Name = "Updated Name", // Only changed property
                Department_Id = _sampleProductDto.Department_Id,
                Price = _sampleProductDto.Price,
                IsActive = _sampleProductDto.IsActive
            };

            _mockProductService.Setup(x => x.UpdateAsync(It.Is<ProductDTO>(d =>
                d.Id_Product == _sampleProductDto.Id_Product &&
                d.Name == "Updated Name"
            ))).ReturnsAsync(Response<ProductDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockProductService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Updated Name", result.Data.Name);

            // Verify other properties remain unchanged
            Assert.Equal(_sampleProductDto.Department_Id, result.Data.Department_Id);
            Assert.Equal(_sampleProductDto.Price, result.Data.Price);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess()
        {
            // Arrange
            _mockProductService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockProductService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
        }

        #endregion

        #region Query Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<ProductDTO> { _sampleProductDto };
            _mockProductService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<ProductDTO>>.Success(products, "Success"));

            // Act
            var result = await _mockProductService.Object.GetAllAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetAllWithIncludesAsync_ShouldReturnDetailedProducts()
        {
            // Arrange
            var detailedProducts = new List<ProductWithDetailsDto>
        {
            new ProductWithDetailsDto
            {
                Id = 1,
                Name = "Test",
                Department = new DepartmentWithBranchDTO(),
                Units = new List<ProductUnitDTO>()
            }
        };

            _mockProductService.Setup(x => x.GetAllWithIncludesAsync())
                .ReturnsAsync(Response<List<ProductWithDetailsDto>>.Success(detailedProducts, "Success"));

            // Act
            var result = await _mockProductService.Object.GetAllWithIncludesAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
        }

        #endregion

        #region Utility Method Tests

        [Fact]
        public async Task AnyProductsInDepartmentAsync_ShouldReturnBool()
        {
            // Arrange
            _mockProductService.Setup(x => x.AnyProductsInDepartmentAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _mockProductService.Object.AnyProductsInDepartmentAsync(1);

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}
