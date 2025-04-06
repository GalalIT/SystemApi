using Domin.System.Entities;
using Domin.System.IRepository.IUnitRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.UnitRepository
{
    public class AllUnitRepository: BaseRepository<Unit>, IAllUnitRepository
    {
        private readonly AppDbContext context;

        public AllUnitRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<List<Unit>> GetAllUnitsByBranch(int branchId)
        {
            // Directly filter units by branch
            return await context.units.Include(pu => pu.Branch)
                .Where(u => u.Branch_Id == branchId)
                .ToListAsync();
        }

        public async Task<List<Unit>> GetAllIncludeToBranchAsync()
        {
            var res = await context.Set<Unit>().Include(pu => pu.Branch).ToListAsync();
            return res;
        }
    }
}
