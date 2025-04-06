using Application.System.DTO;
using Application.System.Interface.IProduct_UnitOperation;
using Application.System.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Tests.Interface
{
    public class IAllProduct_UnitOperationTests
    {
        private readonly Mock<IAllProduct_UnitOperation> _mockProductUnitService;
        private readonly ProductUnitDTO _sampleProductUnitDto;

        public IAllProduct_UnitOperationTests()
        {
            _mockProductUnitService = new Mock<IAllProduct_UnitOperation>();

            _sampleProductUnitDto = new ProductUnitDTO
            {
                Id = 1,
                ProductId = 10,
                UnitId = 5,
                SpecialPrice = 99.99m
            };
        }

        #region CRUD Operation Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_ForValidProductUnit()
        {
            // Arrange
            _mockProductUnitService.Setup(x => x.CreateAsync(It.IsAny<ProductUnitDTO>()))
                .ReturnsAsync(Response<ProductUnitDTO>.Success(_sampleProductUnitDto, "Created"));

            // Act
            var result = await _mockProductUnitService.Object.CreateAsync(_sampleProductUnitDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(10, result.Data.ProductId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ShouldFail_WhenProductIdInvalid(int productId)
        {
            // Arrange
            var invalidDto = new ProductUnitDTO { ProductId = productId, UnitId = 1, SpecialPrice = 10 };
            _mockProductUnitService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<ProductUnitDTO>.Failure("Invalid product ID", "400"));

            // Act
            var result = await _mockProductUnitService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Status);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedProductUnit()
        {
            // Arrange
            var updatedDto = new ProductUnitDTO
            {
                Id = 1,
                SpecialPrice = 129.99m,
                ProductId = 10,
                UnitId = 5
            };
            _mockProductUnitService.Setup(x => x.UpdateAsync(It.IsAny<ProductUnitDTO>()))
                .ReturnsAsync(Response<ProductUnitDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockProductUnitService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.Equal(129.99m, result.Data.SpecialPrice);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenProductUnitExists()
        {
            // Arrange
            _mockProductUnitService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockProductUnitService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProductUnit_WhenExists()
        {
            // Arrange
            _mockProductUnitService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<ProductUnitDTO>.Success(_sampleProductUnitDto, "Found"));

            // Act
            var result = await _mockProductUnitService.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(5, result.Data.UnitId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProductUnits()
        {
            // Arrange
            var productUnits = new List<ProductUnitDTO> { _sampleProductUnitDto };
            _mockProductUnitService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<ProductUnitDTO>>.Success(productUnits, "Success"));

            // Act
            var result = await _mockProductUnitService.Object.GetAllAsync();

            // Assert
            Assert.Single(result.Data);
        }

        #endregion

        #region Specialized Method Tests

        [Fact]
        public async Task GetAllIncludeProdDepAsync_ShouldReturnProductUnitsWithDetails()
        {
            // Arrange
            var productUnits = new List<ProductUnitDTO> { _sampleProductUnitDto };
            _mockProductUnitService.Setup(x => x.GetAllIncludeProdDepAsync())
                .ReturnsAsync(Response<List<ProductUnitDTO>>.Success(productUnits, "Success"));

            // Act
            var result = await _mockProductUnitService.Object.GetAllIncludeProdDepAsync();

            // Assert
            Assert.Equal(99.99m, result.Data.First().SpecialPrice);
        }

        [Fact]
        public async Task GetProductUnitsByProductIdAsync_ShouldFilterByProduct()
        {
            // Arrange
            var productUnits = new List<ProductUnitDTO> { _sampleProductUnitDto };
            _mockProductUnitService.Setup(x => x.GetProductUnitsByProductIdAsync(10))
                .ReturnsAsync(Response<List<ProductUnitDTO>>.Success(productUnits, "Success"));

            // Act
            var result = await _mockProductUnitService.Object.GetProductUnitsByProductIdAsync(10);

            // Assert
            Assert.Single(result.Data);
            Assert.Equal(10, result.Data.First().ProductId);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenSpecialPriceInvalid()
        {
            // Arrange
            var invalidDto = new ProductUnitDTO { ProductId = 1, UnitId = 1, SpecialPrice = 0 };
            _mockProductUnitService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<ProductUnitDTO>.Failure("Invalid price", "400"));

            // Act
            var result = await _mockProductUnitService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenProductUnitMissing()
        {
            // Arrange
            _mockProductUnitService.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync(Response<ProductUnitDTO>.Failure("Not found", "404"));

            // Act
            var result = await _mockProductUnitService.Object.GetByIdAsync(99);

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
            var updateDto = new ProductUnitDTO { Id = 1, SpecialPrice = 150 };
            _mockProductUnitService.Setup(x => x.UpdateAsync(updateDto))
                .ReturnsAsync(Response<ProductUnitDTO>.Success(updateDto, "Updated"));

            // Act
            await _mockProductUnitService.Object.UpdateAsync(updateDto);

            // Assert
            _mockProductUnitService.Verify(x => x.UpdateAsync(updateDto), Times.Once);
        }

        [Fact]
        public async Task ShouldVerifyGetProductUnitsByProductIdAsyncWasCalled()
        {
            // Arrange
            _mockProductUnitService.Setup(x => x.GetProductUnitsByProductIdAsync(10))
                .ReturnsAsync(Response<List<ProductUnitDTO>>.Success(
                    new List<ProductUnitDTO>(), "Success"));

            // Act
            await _mockProductUnitService.Object.GetProductUnitsByProductIdAsync(10);

            // Assert
            _mockProductUnitService.Verify(x => x.GetProductUnitsByProductIdAsync(10), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task CreateAsync_ShouldHandleMinimumSpecialPrice()
        {
            // Arrange
            var dto = new ProductUnitDTO
            {
                ProductId = 1,
                UnitId = 1,
                SpecialPrice = 0.01m // Minimum valid price
            };
            _mockProductUnitService.Setup(x => x.CreateAsync(dto))
                .ReturnsAsync(Response<ProductUnitDTO>.Success(dto, "Created"));

            // Act
            var result = await _mockProductUnitService.Object.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(0.01m, result.Data.SpecialPrice);
        }

        [Fact]
        public async Task GetProductUnitsByProductIdAsync_ShouldReturnEmpty_WhenNoMatches()
        {
            // Arrange
            _mockProductUnitService.Setup(x => x.GetProductUnitsByProductIdAsync(99))
                .ReturnsAsync(Response<List<ProductUnitDTO>>.Success(
                    new List<ProductUnitDTO>(), "No matches"));

            // Act
            var result = await _mockProductUnitService.Object.GetProductUnitsByProductIdAsync(99);

            // Assert
            Assert.Empty(result.Data);
        }

        #endregion
    }
}
