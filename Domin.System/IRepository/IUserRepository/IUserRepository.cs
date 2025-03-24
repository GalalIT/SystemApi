using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IUserRepository
{
    public interface IUserRepository
    {
        Task<int> GetUserBranchIdAsync(string userId);
    }

}
