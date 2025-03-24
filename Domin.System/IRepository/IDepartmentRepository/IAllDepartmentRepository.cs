using Domin.System.Entities;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IDepartmentRepository
{
    public interface IAllDepartmentRepository : IAllBaseRepository<Department>
    {
        Task<List<Department>> GetAllDepartmentsByUserBranchAsync(int userBranchId); // New method
        Task<List<Department>> GetAllDepartmentIncludeToBranchAsync();

    }
}
