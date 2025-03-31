using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.OrderUseCase.Interface
{
    public interface IOrderUseCase
    {
        Task<Response<int>> ProcessOrderCreateAsync(CreateCartDTO cartDto);
        Task<Response> DeleteOrderAsync(int orderId);
        Task<Response<OrderDetailResponse>> GetOrderWithDetailsAsync(int orderId);
        Task<Response<int>> ProcessOrderUpdateAsync(int orderId, CreateCartDTO cartDto);
        Task<Response<List<OrderDTO>>> GetOrdersAsync();
        Task<Response<OrderDTO>> GetOrderByIdAsync(int orderId);
    }
}
