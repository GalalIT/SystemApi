using Domin.System.Entities;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IUnitRepository
{
    public interface IAllUnitRepository : IAllBaseRepository<Unit>
    {
        Task<List<Unit>> GetAllUnitsByBranch(int branchId);
        Task<List<Unit>> GetAllIncludeToBranchAsync();
    }
}
