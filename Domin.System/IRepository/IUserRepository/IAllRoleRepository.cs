using Domin.System.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IUserRepository
{
    public interface IAllRoleRepository
    {
        Task<bool> RoleExistsAsync(string roleName);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task<IdentityResult> CreateRoleAsync(string roleName);

    }
}
