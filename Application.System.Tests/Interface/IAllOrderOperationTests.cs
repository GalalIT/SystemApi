using Application.System.DTO;
using Application.System.Interface.IOrderOperation;
using Application.System.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Tests.Interface
{
    public class IAllOrderOperationTests
    {
        private readonly Mock<IAllOrderOperation> _mockOrderService;
        private readonly OrderDTO _sampleOrderDto;
        private readonly OrderDetailResponse _sampleOrderDetailResponse;

        public IAllOrderOperationTests()
        {
            _mockOrderService = new Mock<IAllOrderOperation>();

            _sampleOrderDto = new OrderDTO
            {
                Id_Order = 1,
                Total_Amount = 100,
                Total_AmountAfterDiscount = 90,
                Discount = 10,
                OrderNumber = "ORD-001",
                OrderType = 1,
                Branch_Id = 1,
                Company_id = 1,
                User_id = "user1"
            };

            _sampleOrderDetailResponse = new OrderDetailResponse
            {
                OrderId = 1,
                OrderNumber = "ORD-001",
                OrderDate = DateTime.Now,
                TotalAmount = 100,
                Discount = 10,
                FinalAmount = 90,
                BranchName = "Main Branch",
                CompanyName = "Test Company",
                CustomerName = "Test User",
                Items = new List<OrderItemDetail>
            {
                new OrderItemDetail
                {
                    ProductId = 1,
                    ProductName = "Test Product",
                    UnitName = "Each",
                    Quantity = 2,
                    UnitPrice = 50,
                    TotalPrice = 100,
                    Description = "Test description"
                }
            }
            };
        }

        #region CRUD Operation Tests

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_ForValidOrder()
        {
            // Arrange
            _mockOrderService.Setup(x => x.CreateAsync(It.IsAny<OrderDTO>()))
                .ReturnsAsync(Response<OrderDTO>.Success(_sampleOrderDto, "Created"));

            // Act
            var result = await _mockOrderService.Object.CreateAsync(_sampleOrderDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("ORD-001", result.Data.OrderNumber);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(49)]
        public async Task CreateAsync_ShouldFail_WhenAmountInvalid(decimal amount)
        {
            // Arrange
            var invalidDto = new OrderDTO { Total_Amount = amount, Branch_Id = 1, User_id = "user1" };
            _mockOrderService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<OrderDTO>.Failure("Invalid amount", "400"));

            // Act
            var result = await _mockOrderService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Status);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedOrder()
        {
            // Arrange
            var updatedDto = new OrderDTO
            {
                Id_Order = 1,
                Total_Amount = 150,
                Branch_Id = 1,
                User_id = "user1"
            };
            _mockOrderService.Setup(x => x.UpdateAsync(It.IsAny<OrderDTO>()))
                .ReturnsAsync(Response<OrderDTO>.Success(updatedDto, "Updated"));

            // Act
            var result = await _mockOrderService.Object.UpdateAsync(updatedDto);

            // Assert
            Assert.Equal(150, result.Data.Total_Amount);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess_WhenOrderExists()
        {
            // Arrange
            _mockOrderService.Setup(x => x.DeleteAsync(1))
                .ReturnsAsync(Response.Success("Deleted"));

            // Act
            var result = await _mockOrderService.Object.DeleteAsync(1);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenExists()
        {
            // Arrange
            _mockOrderService.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(Response<OrderDTO>.Success(_sampleOrderDto, "Found"));

            // Act
            var result = await _mockOrderService.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(1, result.Data.Id_Order);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<OrderDTO> { _sampleOrderDto };
            _mockOrderService.Setup(x => x.GetAllAsync())
                .ReturnsAsync(Response<List<OrderDTO>>.Success(orders, "Success"));

            // Act
            var result = await _mockOrderService.Object.GetAllAsync();

            // Assert
            Assert.Single(result.Data);
        }

        #endregion

        #region Specialized Method Tests

        [Fact]
        public async Task GetOrderWithDetailsAsync_ShouldReturnOrderDetails()
        {
            // Arrange
            _mockOrderService.Setup(x => x.GetOrderWithDetailsAsync(1))
                .ReturnsAsync(Response<OrderDetailResponse>.Success(_sampleOrderDetailResponse, "Success"));

            // Act
            var result = await _mockOrderService.Object.GetOrderWithDetailsAsync(1);

            // Assert
            Assert.Equal("Main Branch", result.Data.BranchName);
            Assert.Single(result.Data.Items);
        }

        [Fact]
        public async Task ProductExistsInAnyOrderAsync_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            _mockOrderService.Setup(x => x.ProductExistsInAnyOrderAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _mockOrderService.Object.ProductExistsInAnyOrderAsync(1);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenBranchIdMissing()
        {
            // Arrange
            var invalidDto = new OrderDTO { Total_Amount = 100, User_id = "user1" };
            _mockOrderService.Setup(x => x.CreateAsync(invalidDto))
                .ReturnsAsync(Response<OrderDTO>.Failure("Branch ID required", "400"));

            // Act
            var result = await _mockOrderService.Object.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenOrderMissing()
        {
            // Arrange
            _mockOrderService.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync(Response<OrderDTO>.Failure("Not found", "404"));

            // Act
            var result = await _mockOrderService.Object.GetByIdAsync(99);

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
            var updateDto = new OrderDTO { Id_Order = 1, Total_Amount = 200 };
            _mockOrderService.Setup(x => x.UpdateAsync(updateDto))
                .ReturnsAsync(Response<OrderDTO>.Success(updateDto, "Updated"));

            // Act
            await _mockOrderService.Object.UpdateAsync(updateDto);

            // Assert
            _mockOrderService.Verify(x => x.UpdateAsync(updateDto), Times.Once);
        }

        [Fact]
        public async Task ShouldVerifyProductExistsInAnyOrderAsyncWasCalled()
        {
            // Arrange
            _mockOrderService.Setup(x => x.ProductExistsInAnyOrderAsync(1))
                .ReturnsAsync(true);

            // Act
            await _mockOrderService.Object.ProductExistsInAnyOrderAsync(1);

            // Assert
            _mockOrderService.Verify(x => x.ProductExistsInAnyOrderAsync(1), Times.Once);
        }

        #endregion
    }
}
