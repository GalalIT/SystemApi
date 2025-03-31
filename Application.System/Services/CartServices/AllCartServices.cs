using Application.System.DTO;
using Application.System.Interface.ICartOperation;
using Application.System.Interface.IOrderDetailsOperation;
using Application.System.Interface.IOrderOperation;
using Application.System.Utility;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.CartServices
{
    public class AllCartServices : ICartOperation
    {
        private readonly IAllOrderOperation _orderService;
        private readonly IAllOrderDetailsOperation _orderDetailsService;

        public AllCartServices(
            IAllOrderOperation orderService,
            IAllOrderDetailsOperation orderDetailsService)
        {
            _orderService = orderService;
            _orderDetailsService = orderDetailsService;
        }

        public async Task<Response<int>> ProcessCartAsync(CreateCartDTO cartDto)
        {
            // Validate inputs
            if (cartDto.ProductUnitIds == null || cartDto.Quantities == null ||
                cartDto.Prices == null )
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

    }
}
