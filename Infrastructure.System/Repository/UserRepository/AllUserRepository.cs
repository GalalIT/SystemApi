using Domin.System.Entities;
using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AllUserRepository> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AllUserRepository(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext context, ILogger<AllUserRepository> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
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
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }
        // Role-related implementations
        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            // Add null check for the logger itself as a safeguard
            if (_logger == null)
            {
                throw new InvalidOperationException("Logger dependency not initialized");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    _logger.LogWarning("Role name was null or empty");
                    return false;
                }

                var normalizedName = _roleManager.NormalizeKey(roleName);
                var exists = await _roleManager.Roles
                    .AnyAsync(r => r.NormalizedName == normalizedName);

                _logger.LogDebug("Role {RoleName} exists: {Exists}", roleName, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role {RoleName} exists", roleName);
                throw;
            }
        }
        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            var role = new IdentityRole(roleName);
            return await _roleManager.CreateAsync(role);
        }
    }
}
