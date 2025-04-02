using Application.System.DTO;
using Application.System.Interface.IBranchOperation;
using Application.System.UseCace.BranchUseCase.Interface;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.BranchUseCase.Implement
{
    public class BranchUseCase : IBranchUseCase
    {
        private readonly IAllBranchOperation _branchOperation;

        public BranchUseCase(IAllBranchOperation branchOperation)
        {
            _branchOperation = branchOperation;
        }

        public async Task<Response<BranchDTO>> CreateBranchAsync(BranchDTO branchDTO)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(branchDTO.Name))
                    return await Response<BranchDTO>.FailureAsync("Branch name is required", "400");

                if (string.IsNullOrEmpty(branchDTO.Address))
                    return await Response<BranchDTO>.FailureAsync("Address is required", "400");

                

                var result = await _branchOperation.CreateAsync(branchDTO);
                return result;
            }
            catch (Exception ex)
            {
                // Log the error properly in production (using ILogger)
                return await Response<BranchDTO>.FailureAsync($"Failed to add branch: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<BranchDTO>>> GetAllBranchesAsync()
        {
            try
            {
                return await _branchOperation.GetAllAsync();
            }
            catch (Exception ex)
            {
                return await Response<List<BranchDTO>>.FailureAsync($"Failed to get branches: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteBranchAsync(int id)
        {
            try
            {
                var branch = await _branchOperation.GetByIdAsync(id);
                if (!branch.Succeeded)
                    return Response.Failure("Branch not found", "404");

                return await _branchOperation.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                //return await Response.FailureAsync($"Failed to delete branch: {ex.Message}", "500");
                return Response.Failure($"Failed to delete branch: {ex.Message}", "500");
            }
        }

        public async Task<Response<BranchDTO>> GetBranchByIdAsync(int id)
        {
            try
            {
                return await _branchOperation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                return await Response<BranchDTO>.FailureAsync($"Failed to get branch: {ex.Message}", "500");
            }
        }

        public async Task<Response<string>> GetBranchNameByIdAsync(int branchId)
        {
            try
            {
                var name = await _branchOperation.GetBranchNameById(branchId);
                if (string.IsNullOrEmpty(name))
                    return await Response<string>.FailureAsync("Branch not found", "404");

                return await Response<string>.SuccessAsync(name, "Branch name retrieved");
            }
            catch (Exception ex)
            {
                return await Response<string>.FailureAsync($"Failed to get branch name: {ex.Message}", "500");
            }
        }

        public async Task<Response<BranchDTO>> UpdateBranchAsync(BranchDTO branchDTO)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(branchDTO.Name))
                    return await Response<BranchDTO>.FailureAsync("Branch name is required", "400");

                if (string.IsNullOrEmpty(branchDTO.Address))
                    return await Response<BranchDTO>.FailureAsync("Address is required", "400");

                var existingBranch = await _branchOperation.GetByIdAsync(branchDTO.Id_Branch);
                if (!existingBranch.Succeeded)
                    return await Response<BranchDTO>.FailureAsync("Branch not found", "404");

                return await _branchOperation.UpdateAsync(branchDTO);
            }
            catch (Exception ex)
            {
                return await Response<BranchDTO>.FailureAsync($"Failed to update branch: {ex.Message}", "500");
            }
        }
    }
}
