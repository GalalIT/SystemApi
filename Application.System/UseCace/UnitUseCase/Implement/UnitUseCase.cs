using Application.System.DTO;
using Application.System.Interface.IUnitOperation;
using Application.System.UseCace.UnitUseCase.Interface;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.UnitUseCase.Implement
{
    public class UnitUseCase : IUnitUseCase
    {
        private readonly IAllUnitOperation _unitOperation;

        public UnitUseCase(IAllUnitOperation unitOperation)
        {
            _unitOperation = unitOperation;
        }

        public async Task<Response<UnitDTO>> CreateUnitAsync(UnitDTO unitDTO)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(unitDTO.Name))
                    return Response<UnitDTO>.Failure("Unit name is required", "400");

                if (unitDTO.Branch_Id <= 0)
                    return Response<UnitDTO>.Failure("Branch ID is invalid", "400");

                return await _unitOperation.CreateAsync(unitDTO);
            }
            catch (Exception ex)
            {
                return Response<UnitDTO>.Failure($"Failed to create unit: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteUnitAsync(int id)
        {
            try
            {
                var unitResponse = await _unitOperation.GetByIdAsync(id);
                if (!unitResponse.Succeeded)
                    return Response.Failure("Unit not found", "404");

                return await _unitOperation.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete unit: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<UnitDTO>>> GetAllUnitsAsync()
        {
            try
            {
                return await _unitOperation.GetAllAsync();
            }
            catch (Exception ex)
            {
                return Response<List<UnitDTO>>.Failure($"Failed to retrieve units: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<UnitDTO>>> GetAllUnitsIncludeToBranchAsync()
        {
            try
            {
                return await _unitOperation.GetAllIncludeToBranchAsync();
            }
            catch (Exception ex)
            {
                return Response<List<UnitDTO>>.Failure(
                    $"Failed to retrieve units with branch details: {ex.Message}",
                    "500");
            }
        }

        public async Task<Response<List<UnitDTO>>> GetAllUnitsByBranchAsync(int branchId)
        {
            try
            {
                if (branchId <= 0)
                    return Response<List<UnitDTO>>.Failure("Invalid branch ID", "400");

                return await _unitOperation.GetAllUnitsByBranch(branchId);
            }
            catch (Exception ex)
            {
                return Response<List<UnitDTO>>.Failure(
                    $"Failed to retrieve units by branch: {ex.Message}",
                    "500");
            }
        }

        public async Task<Response<UnitDTO>> GetUnitByIdAsync(int id)
        {
            try
            {
                return await _unitOperation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                return Response<UnitDTO>.Failure($"Failed to retrieve unit: {ex.Message}", "500");
            }
        }

        public async Task<Response<UnitDTO>> UpdateUnitAsync(UnitDTO unitDTO)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(unitDTO.Name))
                    return Response<UnitDTO>.Failure("Unit name is required", "400");

                if (unitDTO.Branch_Id <= 0)
                    return Response<UnitDTO>.Failure("Branch ID is invalid", "400");

                var existingUnit = await _unitOperation.GetByIdAsync(unitDTO.Id_Unit);
                if (!existingUnit.Succeeded)
                    return Response<UnitDTO>.Failure("Unit not found", "404");

                return await _unitOperation.UpdateAsync(unitDTO);
            }
            catch (Exception ex)
            {
                return Response<UnitDTO>.Failure($"Failed to update unit: {ex.Message}", "500");
            }
        }
    }
}
