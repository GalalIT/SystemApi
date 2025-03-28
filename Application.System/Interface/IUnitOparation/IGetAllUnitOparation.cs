using Application.System.DTO;
using Application.System.Interface.IBaseInterface;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUnitOparation
{
    public interface IGetAllUnitOparation : IBaseGetAllAsync<UnitDTO>
    {
        Task<Response<List<UnitDTO>>> GetAllUnitsByBranch(int branchId);
        Task<Response<List<UnitDTO>>> GetAllIncludeToBranchAsync();
    }
}
