using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUnitOperation
{
    public interface IGetAllUnitOperation : IBaseGetAllAsync<UnitDTO>
    {
        Task<Response<List<UnitDTO>>> GetAllUnitsByBranch(int branchId);
        Task<Response<List<UnitDTO>>> GetAllIncludeToBranchAsync();
    }
}
