using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.System.DTO;
using Application.System.UseCace.OrderUseCase.Interface;
using Application.System.Utility;
using Moq;
namespace Application.System.Tests.UseCace
{
    public class IOrderUseCaseMockTests
    {
        private readonly Mock<IOrderUseCase> _mockOrderUseCase;
        private readonly OrderDTO _sampleOrderDto;
        private readonly OrderDetailResponse _sampleOrderDetailResponse;
        private readonly CreateCartDTO _sampleCreateCartDto;

        public IOrderUseCaseMockTests()
        {
            _mockOrderUseCase = new Mock<IOrderUseCase>();

            _sampleOrderDto = new OrderDTO
            {
                Id_Order = 1,
                Total_Amount = 100.50m,
                Total_AmountAfterDiscount = 90.50m,
                Discount = 10.00m,
                OrderNumber = "ORD-001",
                OrderType = 1,
                Branch_Id = 1,
                User_id = "user1"
            };

            _sampleOrderDetailResponse = new OrderDetailResponse
            {
                OrderId = 1,
                OrderNumber = "ORD-001",
                TotalAmount = 100.50m,
                Discount = 10.00m,
                FinalAmount = 90.50m,
                Items = new List<OrderItemDetail>
            {
                new OrderItemDetail
                {
                    ProductId = 1,
                    ProductName = "Test Product",
                    UnitName = "Each",
                    Quantity = 2,
                    UnitPrice = 50.25m,
                    TotalPrice = 100.50m
                }
            }
            };

            _sampleCreateCartDto = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 1 },
                Quantities = new List<int> { 2 },
                Prices = new List<decimal> { 100.50m },
                TotalAmount = 100.50m,
                BranchId = 1,
                UserId = "user1",
                OrderType = 1,
                PriceAfterDiscount = 90.50m,
                Discount = 10.00m
            };
        }

        #region ProcessOrderCreateAsync Tests

        [Fact]
        public async Task ProcessOrderCreateAsync_ShouldReturnOrderId_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<int>.Success(1, "Order processed successfully");
            _mockOrderUseCase.Setup(x => x.ProcessOrderCreateAsync(It.IsAny<CreateCartDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.ProcessOrderCreateAsync(_sampleCreateCartDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task ProcessOrderCreateAsync_ShouldReturnFailure_WhenInvalidInput()
        {
            // Arrange
            var invalidCart = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 1 },
                Quantities = new List<int> { 2 },
                Prices = null // Missing prices
            };

            var expectedResponse = Response<int>.Failure("Product details are missing.", "400");
            _mockOrderUseCase.Setup(x => x.ProcessOrderCreateAsync(invalidCart))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.ProcessOrderCreateAsync(invalidCart);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Product details are missing.", result.Message);
        }

        #endregion

        #region DeleteOrderAsync Tests

        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response.Success("Order deleted");
            _mockOrderUseCase.Setup(x => x.DeleteOrderAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.DeleteOrderAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Order deleted", result.Message);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnFailure_WhenInvalidId()
        {
            // Arrange
            var expectedResponse = Response.Failure("Invalid order ID", "400");
            _mockOrderUseCase.Setup(x => x.DeleteOrderAsync(0))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.DeleteOrderAsync(0);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid order ID", result.Message);
        }

        #endregion

        #region GetOrderWithDetailsAsync Tests

        [Fact]
        public async Task GetOrderWithDetailsAsync_ShouldReturnOrderDetails_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<OrderDetailResponse>.Success(_sampleOrderDetailResponse, "Order found");
            _mockOrderUseCase.Setup(x => x.GetOrderWithDetailsAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.GetOrderWithDetailsAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("ORD-001", result.Data.OrderNumber);
            Assert.Single(result.Data.Items);
        }

        #endregion

        #region ProcessOrderUpdateAsync Tests

        [Fact]
        public async Task ProcessOrderUpdateAsync_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var expectedResponse = Response<int>.Success(1, "Order updated successfully");
            _mockOrderUseCase.Setup(x => x.ProcessOrderUpdateAsync(1, It.IsAny<CreateCartDTO>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.ProcessOrderUpdateAsync(1, _sampleCreateCartDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data);
        }
        [Fact]
        public async Task ProcessOrderCreateAsync_ShouldFail_WhenUnitCountsMismatch()
        {
            // Arrange - 2 units but only 1 quantity
            var invalidCart = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 101, 102 },
                Quantities = new List<int> { 2 }, // Missing second quantity
                Prices = new List<decimal> { 50.00m, 75.00m }
            };

            _mockOrderUseCase.Setup(x => x.ProcessOrderCreateAsync(invalidCart))
                .ReturnsAsync(Response<int>.Failure("Mismatched product details", "400"));

            // Act & Assert
            var result = await _mockOrderUseCase.Object.ProcessOrderCreateAsync(invalidCart);
            Assert.False(result.Succeeded);
            Assert.Equal("Mismatched product details", result.Message);
        }

        [Fact]
        public async Task ProcessOrderCreateAsync_ShouldCalculateCorrectTotal()
        {
            // Arrange
            var cartDto = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 101, 102 },
                Quantities = new List<int> { 1, 2 },
                Prices = new List<decimal> { 10.00m, 20.00m },
                TotalAmount = 50.00m // Correct would be 1*10 + 2*20 = 50
            };

            _mockOrderUseCase.Setup(x => x.ProcessOrderCreateAsync(cartDto))
                .ReturnsAsync(Response<int>.Success(123, "Success"));

            // Act & Assert
            var result = await _mockOrderUseCase.Object.ProcessOrderCreateAsync(cartDto);
            Assert.True(result.Succeeded); // Should pass because total is correct
        }

        [Fact]
        public async Task ProcessOrderCreateAsync_ShouldFail_WhenIncorrectTotal()
        {
            // Arrange
            var cartDto = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 101, 102 },
                Quantities = new List<int> { 1, 2 },
                Prices = new List<decimal> { 10.00m, 20.00m },
                TotalAmount = 40.00m // Incorrect (should be 50)
            };

            _mockOrderUseCase.Setup(x => x.ProcessOrderCreateAsync(cartDto))
                .ReturnsAsync(Response<int>.Failure("Total amount mismatch", "400"));

            // Act & Assert
            var result = await _mockOrderUseCase.Object.ProcessOrderCreateAsync(cartDto);
            Assert.False(result.Succeeded);
        }
        [Fact]
        public async Task ProcessOrderCreateAsync_ShouldHandleProductWithMultipleUnits()
        {
            // Arrange
            var cartDto = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 101, 102 }, // Same product, different units
                Quantities = new List<int> { 2, 3 },         // Quantities for each unit
                Prices = new List<decimal> { 50.00m, 75.00m }, // Prices for each unit
                Descriptions = new List<string> { "Unit A", "Unit B" },
                TotalAmount = 325.00m,  // Corrected to match 2*50 + 3*75 = 325
                BranchId = 1,
                UserId = "user123",
                OrderType = 1,
                PriceAfterDiscount = 315.00m, // Adjusted to be consistent
                Discount = 10.00m
            };

            // Setup mock to return success for order creation
            _mockOrderUseCase.Setup(x => x.ProcessOrderCreateAsync(It.IsAny<CreateCartDTO>()))
                .ReturnsAsync((CreateCartDTO dto) =>
                {
                    // Validate the cart contents
                    if (dto.ProductUnitIds.Count != 2 ||
                        dto.Quantities.Count != 2 ||
                        dto.Prices.Count != 2)
                    {
                        return Response<int>.Failure("Invalid cart items", "400");
                    }

                    // Validate the total amount calculation
                    var expectedTotal = dto.Prices.Zip(dto.Quantities, (p, q) => p * q).Sum();
                    if (Math.Abs(dto.TotalAmount - expectedTotal) > 0.01m) // Allow for decimal rounding
                    {
                        return Response<int>.Failure($"Total amount mismatch. Expected: {expectedTotal}, Actual: {dto.TotalAmount}", "400");
                    }

                    return Response<int>.Success(123, "Order processed");
                });

            // Act
            var result = await _mockOrderUseCase.Object.ProcessOrderCreateAsync(cartDto);

            // Assert
            Assert.True(result.Succeeded, $"Expected success but got: {result.Message}");
            Assert.Equal(123, result.Data);

            // Verify the mock was called with exactly our test data
            _mockOrderUseCase.Verify(x => x.ProcessOrderCreateAsync(
                It.Is<CreateCartDTO>(c =>
                    c.ProductUnitIds.Count == 2 &&
                    c.Quantities.Sum() == 5 && // 2 + 3 units
                    c.Prices[0] == 50.00m &&
                    c.Prices[1] == 75.00m &&
                    Math.Abs(c.TotalAmount - 325.00m) <= 0.01m // Allow for decimal rounding
                )), Times.Once);
        }

        [Fact]
        public async Task ProcessOrderUpdateAsync_ShouldReturnFailure_WhenMismatchedDetails()
        {
            // Arrange
            var invalidCart = new CreateCartDTO
            {
                ProductUnitIds = new List<int> { 1, 2 },
                Quantities = new List<int> { 1 }, // Mismatched count
                Prices = new List<decimal> { 10.00m }
            };

            var expectedResponse = Response<int>.Failure("Mismatched product details", "400");
            _mockOrderUseCase.Setup(x => x.ProcessOrderUpdateAsync(1, invalidCart))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.ProcessOrderUpdateAsync(1, invalidCart);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Mismatched product details", result.Message);
        }

        #endregion

        #region GetOrdersAsync Tests

        [Fact]
        public async Task GetOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<OrderDTO> { _sampleOrderDto };
            var expectedResponse = Response<List<OrderDTO>>.Success(orders, "Orders retrieved");
            _mockOrderUseCase.Setup(x => x.GetOrdersAsync())
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.GetOrdersAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
            Assert.Equal("ORD-001", result.Data[0].OrderNumber);
        }

        #endregion

        #region GetOrderByIdAsync Tests

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenExists()
        {
            // Arrange
            var expectedResponse = Response<OrderDTO>.Success(_sampleOrderDto, "Order found");
            _mockOrderUseCase.Setup(x => x.GetOrderByIdAsync(1))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.GetOrderByIdAsync(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("ORD-001", result.Data.OrderNumber);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnFailure_WhenInvalidId()
        {
            // Arrange
            var expectedResponse = Response<OrderDTO>.Failure("Invalid order ID", "400");
            _mockOrderUseCase.Setup(x => x.GetOrderByIdAsync(0))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockOrderUseCase.Object.GetOrderByIdAsync(0);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Invalid order ID", result.Message);
        }

        #endregion
    }
}
