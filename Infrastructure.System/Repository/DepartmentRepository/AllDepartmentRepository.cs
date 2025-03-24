using Domin.System.Entities;
using Domin.System.IRepository.IDepartmentRepository;
using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using Infrastructure.System.Repository.UserRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.DepartmentRepository
{
    public class AllDepartmentRepository : BaseRepository<Department>, IAllDepartmentRepository
    {
        private readonly AppDbContext context;

        public AllDepartmentRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Department>> GetAllDepartmentsByUserBranchAsync(int userbranchId)
        {
            // Get the user's branch
            var userBranchId = userbranchId;
            if (userBranchId == 0)
            {
                Console.WriteLine("User's branch is not found.");
                return new List<Department>(); // No branch found
            }

            var res = await context.Set<Department>()
                                .Where(d => d.Branch_Id == userBranchId)
                                .Include(d => d.Branch)
                                .ToListAsync();

            Console.WriteLine($"Departments found: {res.Count}");
            return res;

        }
        public async Task<List<Department>> GetAllDepartmentIncludeToBranchAsync()
        {
            var res = await context.Set<Department>().Include(pu => pu.Branch).ToListAsync();
            return res;
        }
    }
}
