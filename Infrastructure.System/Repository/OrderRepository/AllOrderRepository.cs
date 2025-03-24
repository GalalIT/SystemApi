using Domin.System.Entities;
using Domin.System.IRepository.IOrderRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.OrderRepository
{
    public class AllOrderRepository : BaseRepository<Order>, IAllOrderRepository
    {
        public AllOrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
