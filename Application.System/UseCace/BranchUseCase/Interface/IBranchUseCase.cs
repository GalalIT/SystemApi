using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.BranchUseCase.Interface
{
    public interface IBranchUseCase
    {
        Task<Response<BranchDTO>> AddBranchAsync(BranchDTO branchDTO);
        Task<Response<List<BranchDTO>>> GetAllBranchesAsync();
        Task<Response> DeleteBranchAsync(int id);
        Task<Response<BranchDTO>> GetBranchByIdAsync(int id);
        Task<Response<string>> GetBranchNameByIdAsync(int branchId);
        Task<Response<BranchDTO>> UpdateBranchAsync(BranchDTO branchDTO);
    }
}
