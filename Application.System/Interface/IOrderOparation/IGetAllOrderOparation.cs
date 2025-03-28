using Application.System.DTO;
using Application.System.Interface.IBaseInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IOrderOparation
{
    public interface IGetAllOrderOparation : IBaseGetAllAsync<OrderDTO>
    {
    }
}
 