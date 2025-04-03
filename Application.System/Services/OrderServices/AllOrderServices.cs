using Application.System.DTO;
using Application.System.Interface.IOrderOperation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.OrderServices
{
    public class AllOrderServices : IAllOrderOperation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllOrderServices(IUnitOfRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var order = await _unitOfWork._Order.GetByIdAsync(id);
                if (order == null)
                    return Response.Failure("Order not found", "404");

                await _unitOfWork._Order.DeleteAsync(id);
                return Response.Success("Order deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete order: {ex.Message}", "500");
            }
        }


        public async Task<Response<OrderDTO>> CreateAsync(OrderDTO orderDTO)
        {
            try
            {
                // Validation
                if (orderDTO.Total_Amount <= 50)
                    return Response<OrderDTO>.Failure("Total amount must be greater than 50", "400");

                if (orderDTO.Branch_Id <= 0)
                    return Response<OrderDTO>.Failure("Branch ID is required", "400");

                if (string.IsNullOrEmpty(orderDTO.User_id))
                    return Response<OrderDTO>.Failure("User ID is required", "400");

                // Map DTO to entity
                var order = new Order
                {
                    Total_Amount = orderDTO.Total_Amount,
                    Total_AmountAfterDiscount = orderDTO.Total_AmountAfterDiscount,
                    Discount = orderDTO.Discount,
                    DateTime_Created = orderDTO.DateTime_Created,
                    OrderNumber = orderDTO.OrderNumber,
                    OrderType = orderDTO.OrderType,
                    Branch_Id = orderDTO.Branch_Id,
                    Company_id = orderDTO.Company_id,
                    User_id = orderDTO.User_id
                };

                await _unitOfWork._Order.AddAsync(order);

                // Update DTO with generated ID
                orderDTO.Id_Order = order.Id_Order;
                return Response<OrderDTO>.Success(orderDTO, "Order created successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDTO>.Failure($"Failed to create order: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<OrderDTO>>> GetAllAsync()
        {
            try
            {
                var orders = await _unitOfWork._Order.GetAllAsync();
                var orderDTOs = orders.Select(MapToDTO).ToList();
                return Response<List<OrderDTO>>.Success(orderDTOs, "All orders retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<OrderDTO>>.Failure($"Failed to retrieve orders: {ex.Message}", "500");
            }
        }

        public async Task<Response<OrderDTO>> GetByIdAsync(int id)
        {
            try
            {
                var order = await _unitOfWork._Order.GetByIdAsync(id);
                if (order == null)
                    return Response<OrderDTO>.Failure("Order not found", "404");

                return Response<OrderDTO>.Success(MapToDTO(order), "Order retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDTO>.Failure($"Failed to retrieve order: {ex.Message}", "500");
            }
        }

        public async Task<Response<OrderDetailResponse>> GetOrderWithDetailsAsync(int orderId)
        {
            try
            {
                if (orderId <= 0)
                    return Response<OrderDetailResponse>.Failure("Invalid order ID", "400");

                var order = await _unitOfWork._Order.GetOrderWithDetailsAsync(orderId);

                if (order == null)
                    return Response<OrderDetailResponse>.Failure("Order not found", "404");

                var response = new OrderDetailResponse
                {
                    OrderId = order.Id_Order,
                    OrderNumber = order.OrderNumber,
                    OrderDate = order.DateTime_Created,
                    TotalAmount = order.Total_Amount,
                    Discount = order.Discount,
                    FinalAmount = order.Total_AmountAfterDiscount,
                    BranchName = order.Branch?.Name,
                    CompanyName = order.Company?.Name,
                    CustomerName = order.applicationUser?.UserName,
                    Items = order.OrderDetails.Select(od => new OrderItemDetail
                    {
                        ProductId = od.product_Unit.Product.Id_Product,
                        ProductName = od.product_Unit.Product.Name,
                        UnitName = od.product_Unit.Unit?.Name,
                        Quantity = od.Quantity,
                        UnitPrice = od.Quantity > 0 ? od.Total_Price / od.Quantity : 0,
                        TotalPrice = od.Total_Price,
                        Description = od.Description_product
                    }).ToList()
                };

                return Response<OrderDetailResponse>.Success(response);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
                return Response<OrderDetailResponse>.Failure("Error retrieving order details", "500");
            }
        }

        public async Task<Response<OrderDTO>> UpdateAsync(OrderDTO orderDTO)
        {
            try
            {
                var order = await _unitOfWork._Order.GetByIdAsync(orderDTO.Id_Order);
                if (order == null)
                    return Response<OrderDTO>.Failure("Order not found", "404");

                // Validate required fields
                if (orderDTO.Total_Amount <= 50)
                    return Response<OrderDTO>.Failure("Total amount must be greater than 50", "400");

                if (orderDTO.Branch_Id <= 0)
                    return Response<OrderDTO>.Failure("Branch ID is required", "400");

                // Update properties
                order.Total_Amount = orderDTO.Total_Amount;
                order.Total_AmountAfterDiscount = orderDTO.Total_AmountAfterDiscount;
                order.Discount = orderDTO.Discount;
                order.DateTime_Created = orderDTO.DateTime_Created;
                order.OrderNumber = orderDTO.OrderNumber;
                order.OrderType = orderDTO.OrderType;
                order.Branch_Id = orderDTO.Branch_Id;
                order.Company_id = orderDTO.Company_id;
                order.User_id = orderDTO.User_id;

                await _unitOfWork._Order.UpdateAsync(order);
                return Response<OrderDTO>.Success(MapToDTO(order), "Order updated successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDTO>.Failure($"Failed to update order: {ex.Message}", "500");
            }
        }

        public async Task<bool> ProductExistsInAnyOrderAsync(int productId)
        {
            
            return await _unitOfWork._Order.AnyOrderDetailsWithProductAsync(productId);
        }
        private OrderDTO MapToDTO(Order order)
        {
            return new OrderDTO
            {
                Id_Order = order.Id_Order,
                Total_Amount = order.Total_Amount,
                Total_AmountAfterDiscount = order.Total_AmountAfterDiscount,
                Discount = order.Discount,
                DateTime_Created = order.DateTime_Created,
                OrderNumber = order.OrderNumber,
                OrderType = order.OrderType,
                Branch_Id = order.Branch_Id,
                Company_id = order.Company_id,
                User_id = order.User_id
            };
        }
    }
}
