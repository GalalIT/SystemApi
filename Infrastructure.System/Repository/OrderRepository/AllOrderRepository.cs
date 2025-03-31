using Domin.System.Entities;
using Domin.System.IRepository.IOrderRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.OrderRepository
{
    public class AllOrderRepository : BaseRepository<Order>, IAllOrderRepository
    {
        private readonly AppDbContext context;
        public AllOrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await context.orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.product_Unit)
                        .ThenInclude(pu => pu.Product)
                .Include(o => o.Branch)
                .Include(o => o.Company)
                .Include(o => o.applicationUser)
                .FirstOrDefaultAsync(o => o.Id_Order == orderId);
        }
    }
}
