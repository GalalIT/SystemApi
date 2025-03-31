using Application.System.DTO;
using Application.System.Interface.IOrderDetailsOperation;
using Application.System.Interface.IOrderOperation;
using Application.System.UseCace.OrderUseCase.Interface;
using Application.System.Utility;
using Domin.System.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.OrderUseCase.Implement
{
    public class OrderUseCase : IOrderUseCase
    {
        private readonly IAllOrderOperation _orderService;
        private readonly IAllOrderDetailsOperation _orderDetailsService;
        private readonly ILogger<OrderUseCase> _logger;

        public OrderUseCase(
            IAllOrderOperation orderService,
            IAllOrderDetailsOperation orderDetailsService, ILogger<OrderUseCase> logger)
        {
            _orderService = orderService;
            _orderDetailsService = orderDetailsService;
        }

        public async Task<Response> DeleteOrderAsync(int orderId)
        {
            try
            {
                // Validate input
                if (orderId <= 0)
                {
                    _logger.LogWarning("Attempted to delete order with invalid ID: {OrderId}", orderId);
                    return Response.Failure("Invalid order ID", "400");
                }

                _logger.LogInformation("Attempting to delete order {OrderId}", orderId);

                // Execute the operation
                var result = await _orderService.DeleteAsync(orderId);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to delete order {OrderId}: {Message}", orderId, result.Message);
                    return result;
                }

                _logger.LogInformation("Successfully deleted order {OrderId}", orderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting order {OrderId}", orderId);
                return Response.Failure("An unexpected error occurred while deleting the order", "500");
            }
        }

        public async Task<Response<OrderDetailResponse>> GetOrderWithDetailsAsync(int orderId)
        {
            try
            {
                // Input validation
                if (orderId <= 0)
                {
                    _logger.LogWarning("Invalid order ID requested: {OrderId}", orderId);
                    return Response<OrderDetailResponse>.Failure("Invalid order ID", "400");
                }

                _logger.LogInformation("Fetching order details for order ID: {OrderId}", orderId);

                // Delegate to the service layer
                var result = await _orderService.GetOrderWithDetailsAsync(orderId);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve order {OrderId}: {Message}",
                        orderId, result.Message);
                    return result;
                }

                _logger.LogInformation("Successfully retrieved order details for ID: {OrderId}", orderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving order {OrderId}", orderId);
                return Response<OrderDetailResponse>.Failure(
                    "An unexpected error occurred while retrieving order details",
                    "500");
            }
        }

        public async Task<Response<int>> ProcessOrderCreateAsync(CreateCartDTO cartDto)
        {
            // Validate inputs
            if (cartDto.ProductUnitIds == null || cartDto.Quantities == null ||
                cartDto.Prices == null)
            {
                return await Response<int>.FailureAsync("Product details are missing.", "400");
            }

            if (cartDto.ProductUnitIds.Count != cartDto.Quantities.Count ||
                cartDto.ProductUnitIds.Count != cartDto.Prices.Count)
            {
                return await Response<int>.FailureAsync("Mismatched product details.", "400");
            }

            try
            {
                // Create order
                var orderDto = new OrderDTO
                {
                    Total_Amount = cartDto.TotalAmount,
                    Branch_Id = cartDto.BranchId,
                    User_id = cartDto.UserId,
                    OrderType = cartDto.OrderType,
                    Total_AmountAfterDiscount = cartDto.PriceAfterDiscount,
                    Discount = cartDto.Discount,
                    OrderNumber = !string.IsNullOrEmpty(cartDto.OrderNumber) ? cartDto.OrderNumber : null,
                    Company_id = cartDto.CompanyId != 0 ? cartDto.CompanyId : null,
                    DateTime_Created = DateTime.Now
                };

                var orderResult = await _orderService.CreateAsync(orderDto);
                if (!orderResult.Succeeded)
                {
                    return await Response<int>.FailureAsync(orderResult.Message, orderResult.Status);
                }

                int orderId = orderResult.Data.Id_Order;

                // Create order details
                var orderDetailsResults = new List<Response<OrderDetailsDTO>>();
                for (int i = 0; i < cartDto.ProductUnitIds.Count; i++)
                {
                    var orderDetailDto = new OrderDetailsDTO
                    {
                        Product_Unit_id = cartDto.ProductUnitIds[i],
                        Quantity = cartDto.Quantities[i],
                        Total_Price = cartDto.Prices[i],
                        Description_product = cartDto.Descriptions[i],
                        Order_Id = orderId
                    };

                    var result = await _orderDetailsService.CreateAsync(orderDetailDto);
                    orderDetailsResults.Add(result);
                }

                // Check for any failures in order details creation
                var failedDetails = orderDetailsResults.Where(r => !r.Succeeded).ToList();
                if (failedDetails.Any())
                {
                    return await Response<int>.FailureAsync(
                        $"Failed to create {failedDetails.Count} order details. First error: {failedDetails.First().Message}",
                        "500");
                }

                return await Response<int>.SuccessAsync(orderId, "Order processed successfully");
            }
            catch (Exception ex)
            {
                return await Response<int>.FailureAsync($"Error processing cart: {ex.Message}", "500");
            }
        }
        public async Task<Response<int>> ProcessOrderUpdateAsync(int orderId, CreateCartDTO cartDto)
        {
            // Validate inputs
            if (orderId <= 0)
            {
                return await Response<int>.FailureAsync("Invalid order ID", "400");
            }

            if (cartDto.ProductUnitIds == null || cartDto.Quantities == null || cartDto.Prices == null)
            {
                return await Response<int>.FailureAsync("Product details are missing", "400");
            }

            if (cartDto.ProductUnitIds.Count != cartDto.Quantities.Count ||
                cartDto.ProductUnitIds.Count != cartDto.Prices.Count)
            {
                return await Response<int>.FailureAsync("Mismatched product details", "400");
            }

            try
            {
                // Get existing order with details
                var existingOrderResponse = await _orderService.GetOrderWithDetailsAsync(orderId);
                if (!existingOrderResponse.Succeeded)
                {
                    return await Response<int>.FailureAsync(existingOrderResponse.Message, existingOrderResponse.Status);
                }

                // Prepare order update
                var orderDto = new OrderDTO
                {
                    Id_Order = orderId,
                    Total_Amount = cartDto.TotalAmount,
                    Branch_Id = cartDto.BranchId,
                    User_id = cartDto.UserId,
                    OrderType = cartDto.OrderType,
                    Total_AmountAfterDiscount = cartDto.PriceAfterDiscount,
                    Discount = cartDto.Discount,
                    OrderNumber = !string.IsNullOrEmpty(cartDto.OrderNumber) ? cartDto.OrderNumber : null,
                    Company_id = cartDto.CompanyId != 0 ? cartDto.CompanyId : null,
                    DateTime_Created = existingOrderResponse.Data.OrderDate // Keep original creation date
                };

                // Update the order
                var orderUpdateResult = await _orderService.UpdateAsync(orderDto);
                if (!orderUpdateResult.Succeeded)
                {
                    return await Response<int>.FailureAsync(orderUpdateResult.Message, orderUpdateResult.Status);
                }

                // Process order details
                var existingDetails = existingOrderResponse.Data.Items.ToDictionary(
                    item => item.ProductId,
                    item => new {
                        Id = item.ProductId,
                        DetailId = item.ProductId // Assuming this maps to OrderDetails.Id_OrderDetail
                    });

                var orderDetailsResults = new List<Response<OrderDetailsDTO>>();
                var processedDetailIds = new List<int>();

                for (int i = 0; i < cartDto.ProductUnitIds.Count; i++)
                {
                    var orderDetailDto = new OrderDetailsDTO
                    {
                        Product_Unit_id = cartDto.ProductUnitIds[i],
                        Quantity = cartDto.Quantities[i],
                        Total_Price = cartDto.Prices[i],
                        Description_product = cartDto.Descriptions?[i],
                        Order_Id = orderId
                    };

                    // Check if this product already exists in the order
                    if (existingDetails.TryGetValue(cartDto.ProductUnitIds[i], out var existingDetail))
                    {
                        // Update existing detail
                        orderDetailDto.Id_OrderDetail = existingDetail.DetailId;
                        var result = await _orderDetailsService.UpdateAsync(orderDetailDto);
                        orderDetailsResults.Add(result);
                        processedDetailIds.Add(existingDetail.DetailId);
                    }
                    else
                    {
                        // Add new detail
                        var result = await _orderDetailsService.CreateAsync(orderDetailDto);
                        orderDetailsResults.Add(result);
                    }
                }

                // Delete any details that weren't included in the update
                var detailsToDelete = existingOrderResponse.Data.Items
                    .Where(item => !processedDetailIds.Contains(item.ProductId))
                    .ToList();

                foreach (var detailToDelete in detailsToDelete)
                {
                    await _orderDetailsService.DeleteAsync(detailToDelete.ProductId);
                }

                // Check for any failures in order details processing
                var failedDetails = orderDetailsResults.Where(r => !r.Succeeded).ToList();
                if (failedDetails.Any())
                {
                    return await Response<int>.FailureAsync(
                        $"Failed to process {failedDetails.Count} order details. First error: {failedDetails.First().Message}",
                        "500");
                }

                return await Response<int>.SuccessAsync(orderId, "Order updated successfully");
            }
            catch (Exception ex)
            {
                return await Response<int>.FailureAsync($"Error updating order: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<OrderDTO>>> GetOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Starting to retrieve all orders");

                var result = await _orderService.GetAllAsync();

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve orders: {Message}", result.Message);
                    return result;
                }

                _logger.LogInformation("Successfully retrieved {Count} orders", result.Data?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving all orders");
                return Response<List<OrderDTO>>.Failure(
                    "An unexpected error occurred while retrieving orders",
                    "500");
            }
        }
        public async Task<Response<OrderDTO>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                // Input validation
                if (orderId <= 0)
                {
                    _logger.LogWarning("Invalid order ID requested: {OrderId}", orderId);
                    return Response<OrderDTO>.Failure("Invalid order ID", "400");
                }

                _logger.LogInformation("Fetching order with ID: {OrderId}", orderId);

                // Delegate to service layer
                var result = await _orderService.GetByIdAsync(orderId);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve order {OrderId}: {Message}",
                        orderId, result.Message);
                    return result;
                }

                _logger.LogInformation("Successfully retrieved order {OrderId}", orderId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while retrieving order {OrderId}", orderId);
                return Response<OrderDTO>.Failure(
                    "An unexpected error occurred while retrieving the order",
                    "500");
            }
        }

    }
}
