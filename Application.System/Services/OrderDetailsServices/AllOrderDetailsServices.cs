using Application.System.DTO;
using Application.System.Interface.IOrderDetailsOperation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.OrderDetailsServices
{
    public class AllOrderDetailsServices : IAllOrderDetailsOperation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllOrderDetailsServices(IUnitOfRepository unitOfRepository)
        {
            _unitOfWork = unitOfRepository;
        }

        public async Task<OrderDetails> AddAsync(OrderDetails entity)
        {
            await _unitOfWork._OrderDetails.AddAsync(entity);
            return entity;
        }

        public async Task<Response<OrderDetailsDTO>> CreateAsync(OrderDetailsDTO orderDetailsDTO)
        {
            try
            {
                // Validation
                if (orderDetailsDTO.Quantity <= 0)
                    return Response<OrderDetailsDTO>.Failure("Quantity must be greater than 0", "400");

                if (orderDetailsDTO.Total_Price <= 0)
                    return Response<OrderDetailsDTO>.Failure("Total price must be greater than 0", "400");

                if (orderDetailsDTO.Product_Unit_id <= 0)
                    return Response<OrderDetailsDTO>.Failure("Product unit ID is required", "400");

                // Map DTO to entity
                var orderDetails = new OrderDetails
                {
                    Description_product = orderDetailsDTO.Description_product,
                    Quantity = orderDetailsDTO.Quantity,
                    Total_Price = orderDetailsDTO.Total_Price,
                    Product_Unit_id = orderDetailsDTO.Product_Unit_id,
                    Order_Id = orderDetailsDTO.Order_Id
                };

                await _unitOfWork._OrderDetails.AddAsync(orderDetails);

                // Update DTO with generated ID
                orderDetailsDTO.Id_OrderDetail = orderDetails.Id_OrderDetail;
                return Response<OrderDetailsDTO>.Success(orderDetailsDTO, "Order detail created successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailsDTO>.Failure($"Failed to create order detail: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var orderDetail = await _unitOfWork._OrderDetails.GetByIdAsync(id);
                if (orderDetail == null)
                    return Response.Failure("Order detail not found", "404");

                await _unitOfWork._OrderDetails.DeleteAsync(id);
                return Response.Success("Order detail deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete order detail: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<OrderDetailsDTO>>> GetAllAsync()
        {
            try
            {
                var orderDetails = await _unitOfWork._OrderDetails.GetAllAsync();
                var orderDetailsDTOs = orderDetails.Select(MapToDTO).ToList();
                return Response<List<OrderDetailsDTO>>.Success(orderDetailsDTOs, "All order details retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<OrderDetailsDTO>>.Failure($"Failed to retrieve order details: {ex.Message}", "500");
            }
        }

        public async Task<Response<OrderDetailsDTO>> GetByIdAsync(int id)
        {
            try
            {
                var orderDetail = await _unitOfWork._OrderDetails.GetByIdAsync(id);
                if (orderDetail == null)
                    return Response<OrderDetailsDTO>.Failure("Order detail not found", "404");

                return Response<OrderDetailsDTO>.Success(MapToDTO(orderDetail), "Order detail retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailsDTO>.Failure($"Failed to retrieve order detail: {ex.Message}", "500");
            }
        }

        public async Task<Response<OrderDetailsDTO>> UpdateAsync(OrderDetailsDTO orderDetailsDTO)
        {
            try
            {
                var orderDetail = await _unitOfWork._OrderDetails.GetByIdAsync(orderDetailsDTO.Id_OrderDetail);
                if (orderDetail == null)
                    return Response<OrderDetailsDTO>.Failure("Order detail not found", "404");

                // Validate required fields
                if (orderDetailsDTO.Quantity <= 0)
                    return Response<OrderDetailsDTO>.Failure("Quantity must be greater than 0", "400");

                if (orderDetailsDTO.Total_Price <= 0)
                    return Response<OrderDetailsDTO>.Failure("Total price must be greater than 0", "400");

                // Update properties
                orderDetail.Description_product = orderDetailsDTO.Description_product;
                orderDetail.Quantity = orderDetailsDTO.Quantity;
                orderDetail.Total_Price = orderDetailsDTO.Total_Price;
                orderDetail.Product_Unit_id = orderDetailsDTO.Product_Unit_id;
                orderDetail.Order_Id = orderDetailsDTO.Order_Id;

                await _unitOfWork._OrderDetails.UpdateAsync(orderDetail);
                return Response<OrderDetailsDTO>.Success(MapToDTO(orderDetail), "Order detail updated successfully");
            }
            catch (Exception ex)
            {
                return Response<OrderDetailsDTO>.Failure($"Failed to update order detail: {ex.Message}", "500");
            }
        }

        private OrderDetailsDTO MapToDTO(OrderDetails orderDetails)
        {
            return new OrderDetailsDTO
            {
                Id_OrderDetail = orderDetails.Id_OrderDetail,
                Description_product = orderDetails.Description_product,
                Quantity = orderDetails.Quantity,
                Total_Price = orderDetails.Total_Price,
                Product_Unit_id = orderDetails.Product_Unit_id,
                Order_Id = orderDetails.Order_Id
            };
        }
    }
}
