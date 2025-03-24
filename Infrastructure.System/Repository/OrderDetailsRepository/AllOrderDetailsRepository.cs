using Domin.System.Entities;
using Domin.System.IRepository.IOrderDetailsRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.OrderDetailsRepository
{
    public class AllOrderDetailsRepository : BaseRepository<OrderDetails>, IAllOrderDetailsRepository
    {
        public AllOrderDetailsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
