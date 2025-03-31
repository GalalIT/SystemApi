using Domin.System.Entities;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IOrderRepository
{
    public interface IAllOrderRepository : IAllBaseRepository<Order>
    {
        Task<Order> GetOrderWithDetailsAsync(int orderId);

    }
}
