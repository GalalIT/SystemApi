using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.UnitUseCase.Interface
{
    public interface IUnitUseCase
    {
        Task<Response<UnitDTO>> CreateUnitAsync(UnitDTO unitDTO);
        Task<Response> DeleteUnitAsync(int id);
        Task<Response<List<UnitDTO>>> GetAllUnitsAsync();
        Task<Response<List<UnitDTO>>> GetAllUnitsIncludeToBranchAsync();
        Task<Response<List<UnitDTO>>> GetAllUnitsByBranchAsync(int branchId);
        Task<Response<UnitDTO>> GetUnitByIdAsync(int id);
        Task<Response<UnitDTO>> UpdateUnitAsync(UnitDTO unitDTO);
    }
}
