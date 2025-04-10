using Domin.System.Entities;
using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.UserRepository
{
    public class AllUserRepository : IAllUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AllUserRepository(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<int> GetUserBranchIdAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return await _context.Users
                                    .Where(u => u.Id == userId)
                                    .Select(u => u.Branch_Id)
                                    .FirstOrDefaultAsync();
            }

            return 0; // Return a default value if user ID is not valid
        }


        public async Task<IdentityResult> AddAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            return await _userManager.DeleteAsync(user);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _userManager.FindByIdAsync(id) != null;
        }

        public async Task<IEnumerable<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return await _userManager.Users.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.Include(u => u.Branch).ToListAsync();
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserWithBranchAsync(string id)
        {
            return await _userManager.Users
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByBranchAsync(int branchId)
        {
            return await _userManager.Users
                .Where(u => u.Branch_Id == branchId)
                .Include(u => u.Branch)
                .ToListAsync();
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateProfilePictureAsync(string userId, byte[] profilePicture)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            user.ProfilePicture = profilePicture;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ToggleUserStatusAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            user.IsActive = isActive;
            return await _userManager.UpdateAsync(user);
        }
    }
}
