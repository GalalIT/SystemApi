using Domin.System.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IUserRepository
{
    public interface IAllUserRepository
    {
        Task<int> GetUserBranchIdAsync(string userId);
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<ApplicationUser> GetByUserNameAsync(string userName);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<IdentityResult> AddAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<IdentityResult> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<ApplicationUser> GetUserWithBranchAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetUsersByBranchAsync(int branchId);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<IdentityResult> UpdateProfilePictureAsync(string userId, byte[] profilePicture);
        Task<IdentityResult> ToggleUserStatusAsync(string userId, bool isActive);
    }

}
