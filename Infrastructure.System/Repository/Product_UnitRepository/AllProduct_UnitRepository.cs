using Domin.System.Entities;
using Domin.System.IRepository.IProduct_UnitRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.Product_UnitRepository
{
    public class AllProduct_UnitRepository : BaseRepository<Product_Unit>, IAllProduct_UnitRepository
    {
        private readonly AppDbContext context;

        public AllProduct_UnitRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Product_Unit>> GetAllIncludeProdDepAsync()
        {
            try
            {
                var res = await context.Set<Product_Unit>().Include(pu => pu.Product).ThenInclude(p => p.Department).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in AllProduct_UnitRepository<GetAllIncludeProdDepAsync>!!!!");
                Console.WriteLine(ex.Message);
                return default;
            }
        }
        public async Task<List<Product_Unit>> GetProductUnitsByProductIdAsync(int productId)
        {
            return await context.Product_Units
                .Where(pu => pu.ProductId == productId)
                .ToListAsync();
        }

    }
}
