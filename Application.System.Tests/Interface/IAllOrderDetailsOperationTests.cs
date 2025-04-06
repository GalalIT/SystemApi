using Application.System.DTO;
using Application.System.Interface.IOrderDetailsOperation;
using Application.System.Services.OrderDetailsServices;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Tests.Interface
{
    public class IAllOrderDetailsOperationTests
    {
        private readonly Mock<IAllOrderDetailsOperation> _mockOrderDetailsService;
        private readonly OrderDetailsDTO _sampleOrderDetailDto;

        public IAllOrderDetailsOperationTests()
        {
            _mockOrderDetailsService = new Mock<IAllOrderDetailsOperation>();

            _sampleOrderDetailDto = new OrderDetailsDTO
            {
                Id_OrderDetail = 1,
                Description_product = "Special instructions",
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 5,
                Order_Id = 10
            };
        }

        #region CRUD Operation Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_ForValidOrderDetail()
        {
            // Arrange
            _mockOrderDetailsService.Setup(x => x.CreateAsync(It.IsAny<OrderDetailsDTO>()))
                .ReturnsAsync(Response<OrderDetailsDTO>.Success(_sampleOrderDetailDto, "Created"));

            // Act
            var result = await _mockOrderDetailsService.Object.CreateAsync(_sampleOrderDetailDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateAsync_ShouldFail_WhenQuantityInvalid(int quantity)
        {
            // Arrange
            var invalidDto = new OrderDetailsDTO { Quantity = quantity, Total_Price = 10, Product_Unit_id = 1 };
            _mockOrderDetailsService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<OrderDetailsDTO>.Failure("Invalid quantity", "400"));

            // Act
            var result = await _mockOrderDetailsService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Status);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedOrderDetail()
        {
            // Arrange
            var updatedDto = new OrderDetailsDTO
            {
                Id_OrderDetail = 1,
                Quantity = 3,
                Total_Price = 150.75m
            };
            _mockOrderDetailsService.Setup(x => x.UpdateAsync(It.IsAny<OrderDetailsDTO>()))
                .ReturnsAsync(Response<OrderDetailsDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockOrderDetailsService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.Equal(3, result.Data.Quantity);
            Assert.Equal(150.75m, result.Data.Total_Price);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenOrderDetailExists()
        {
            // Arrange
            _mockOrderDetailsService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockOrderDetailsService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrderDetail_WhenExists()
        {
            // Arrange
            _mockOrderDetailsService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<OrderDetailsDTO>.Success(_sampleOrderDetailDto, "Found"));

            // Act
            var result = await _mockOrderDetailsService.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(5, result.Data.Product_Unit_id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrderDetails()
        {
            // Arrange
            var orderDetails = new List<OrderDetailsDTO> { _sampleOrderDetailDto };
            _mockOrderDetailsService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<OrderDetailsDTO>>.Success(orderDetails, "Success"));

            // Act
            var result = await _mockOrderDetailsService.Object.GetAllAsync();

            // Assert
            Assert.Single(result.Data);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenTotalPriceInvalid()
        {
            // Arrange
            var invalidDto = new OrderDetailsDTO { Quantity = 1, Total_Price = 0, Product_Unit_id = 1 };
            _mockOrderDetailsService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<OrderDetailsDTO>.Failure("Invalid price", "400"));

            // Act
            var result = await _mockOrderDetailsService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenOrderDetailMissing()
        {
            // Arrange
            _mockOrderDetailsService.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync(Response<OrderDetailsDTO>.Failure("Not found", "404"));

            // Act
            var result = await _mockOrderDetailsService.Object.GetByIdAsync(99);

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
            var updateDto = new OrderDetailsDTO { Id_OrderDetail = 1, Quantity = 5 };
            _mockOrderDetailsService.Setup(x => x.UpdateAsync(updateDto))
                .ReturnsAsync(Response<OrderDetailsDTO>.Success(updateDto, "Updated"));

            // Act
            await _mockOrderDetailsService.Object.UpdateAsync(updateDto);

            // Assert
            _mockOrderDetailsService.Verify(x => x.UpdateAsync(updateDto), Times.Once);
        }

        [Fact]
        public async Task ShouldVerifyDeleteAsyncWasCalled()
        {
            // Arrange
            _mockOrderDetailsService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            await _mockOrderDetailsService.Object.DeleteAsync(1);

            // Assert
            _mockOrderDetailsService.Verify(x => x.DeleteAsync(1), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task CreateAsync_ShouldHandleNullDescription()
        {
            // Arrange
            var dto = new OrderDetailsDTO
            {
                Quantity = 1,
                Total_Price = 10,
                Product_Unit_id = 1,
                Description_product = null
            };
            _mockOrderDetailsService.Setup(x => x.CreateAsync(dto))
                .ReturnsAsync(Response<OrderDetailsDTO>.Success(dto, "Created"));

            // Act
            var result = await _mockOrderDetailsService.Object.CreateAsync(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Data.Description_product);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPreserveDescription_WhenNull()
        {
            // Arrange
            var originalEntity = new OrderDetails
            {
                Id_OrderDetail = 1,
                Description_product = "Original notes",
                Quantity = 2,
                Total_Price = 100,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            var updateDto = new OrderDetailsDTO
            {
                Id_OrderDetail = 1,
                Description_product = null,
                Quantity = 3,
                Total_Price = 150,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            // Setup mock repository behavior
            var mockUnitOfWork = new Mock<IUnitOfRepository>();
            mockUnitOfWork.Setup(u => u._OrderDetails.GetByIdAsync(1))
                .ReturnsAsync(originalEntity);

            OrderDetails updatedEntity = null;
            mockUnitOfWork.Setup(u => u._OrderDetails.UpdateAsync(It.IsAny<OrderDetails>()))
                .Callback<OrderDetails>(e => updatedEntity = e)
                .ReturnsAsync((OrderDetails e) => e); // Return the updated entity

            var service = new AllOrderDetailsServices(mockUnitOfWork.Object);

            // Act
            var result = await service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(updatedEntity);
            Assert.Equal(null, updatedEntity.Description_product);
            Assert.Equal(3, updatedEntity.Quantity);
            Assert.Equal(150, updatedEntity.Total_Price);
            Assert.True(result.Succeeded);
            Assert.Equal(null, result.Data.Description_product);
        }
        #endregion
    }
}
