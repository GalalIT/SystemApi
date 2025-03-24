using Domin.System.Entities;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IBranchRepository
{
    public interface IAllBranchRepository: IAllBaseRepository<Branch>
    {
        Task<string> GetBranchNameById(int branchId);
    }
}
