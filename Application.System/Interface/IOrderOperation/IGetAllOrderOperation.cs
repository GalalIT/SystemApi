using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IOrderOperation
{
    public interface IGetAllOrderOperation : IBaseGetAllAsync<OrderDTO>
    {
        Task<Response<OrderDetailResponse>> GetOrderWithDetailsAsync(int orderId);

    }
}
 