using Domin.System.Entities;
using Domin.System.IRepository.IBranchRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.BranchRepository
{
    public class AllBranchRepository: BaseRepository<Branch>, IAllBranchRepository
    {
        private readonly AppDbContext context;

        public AllBranchRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<string> GetBranchNameById(int branchId)
        {
            var res= await context.branches
                                .Where(b => b.Id_Branch == branchId)
                                .Select(b => b.Name)
                                .FirstOrDefaultAsync();
            return res;
        }
        
    }
}
