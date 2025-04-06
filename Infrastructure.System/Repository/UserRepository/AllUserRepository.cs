using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.UserRepository
{
    public class AllUserRepository : IAllUserRepository
    {
        private readonly AppDbContext context;

        public AllUserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<int> GetUserBranchIdAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return await context.Users
                                    .Where(u => u.Id == userId)
                                    .Select(u => u.Branch_Id)
                                    .FirstOrDefaultAsync();
            }

            return 0; // Return a default value if user ID is not valid
        }
    }
}
