using Application.System.DTO;
using Application.System.Interface.IUnitOperation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.UnitServices
{
    public class AllUnitServices : IAllUnitOperation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllUnitServices(IUnitOfRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Create a new Unit
        public async Task<Response<UnitDTO>> CreateAsync(UnitDTO unitDTO)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrEmpty(unitDTO.Name))
                {
                    return Response<UnitDTO>.Failure("Unit name is required", "400");
                }

                if (unitDTO.Branch_Id <= 0)
                {
                    return Response<UnitDTO>.Failure("Branch ID is invalid", "400");
                }

                // Map UnitDTO to Unit entity
                var unit = new Unit
                {
                    Name = unitDTO.Name,
                    Branch_Id = unitDTO.Branch_Id
                };

                // Add the Unit entity to the repository
                await _unitOfWork._Unit.AddAsync(unit);

                // Map the saved Unit entity back to UnitDTO
                unitDTO.Id_Unit = unit.Id_Unit;
                return Response<UnitDTO>.Success(unitDTO, "Unit created successfully");
            }
            catch (Exception ex)
            {
                return Response<UnitDTO>.Failure($"Failed to create unit: {ex.Message}", "500");
            }
        }

        // Delete a Unit by ID
        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var unit = await _unitOfWork._Unit.GetByIdAsync(id);
                if (unit == null)
                {
                    return Response.Failure("Unit not found", "404");
                }

                await _unitOfWork._Unit.DeleteAsync(id);
                return Response.Success("Unit deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete unit: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<UnitDTO>>> GetAllAsync()
        {
            try
            {
                var units = await _unitOfWork._Unit.GetAllAsync();
                var unitDTOs = units.Select(MapToDTO).ToList();
                return Response<List<UnitDTO>>.Success(unitDTOs, "All units retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<UnitDTO>>.Failure($"Failed to retrieve units: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<UnitWithBranchNameDTO>>> GetAllIncludeToBranchAsync()
        {
            try
            {
                var units = await _unitOfWork._Unit.GetAllIncludeToBranchAsync();
                var unitDTOs = units.Select(MapBranchToDTO).ToList();
                return Response<List<UnitWithBranchNameDTO>>.Success(unitDTOs, "All units with branch information retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<UnitWithBranchNameDTO>>.Failure($"Failed to retrieve units with branch details: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<UnitWithBranchNameDTO>>> GetAllUnitsByBranch(int branchId)
        {
            try
            {
                var units = await _unitOfWork._Unit.GetAllUnitsByBranch(branchId);
                var unitDTOs = units.Select(MapBranchToDTO).ToList();
                return Response<List<UnitWithBranchNameDTO>>.Success(unitDTOs, "Units retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<UnitWithBranchNameDTO>>.Failure($"Failed to retrieve units: {ex.Message}", "500");
            }
        }

        public async Task<Response<UnitDTO>> GetByIdAsync(int id)
        {
            try
            {
                var unit = await _unitOfWork._Unit.GetByIdAsync(id);
                if (unit == null)
                {
                    return Response<UnitDTO>.Failure("Unit not found", "404");
                }

                var unitDTO = MapToDTO(unit);
                return Response<UnitDTO>.Success(unitDTO, "Unit retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<UnitDTO>.Failure($"Failed to retrieve unit: {ex.Message}", "500");
            }
        }

        public async Task<Response<UnitDTO>> UpdateAsync(UnitDTO unitDTO)
        {
            try
            {
                var unit = await _unitOfWork._Unit.GetByIdAsync(unitDTO.Id_Unit);
                if (unit == null)
                {
                    return Response<UnitDTO>.Failure("Unit not found", "404");
                }

                // Update the Unit entity with data from UnitDTO
                unit.Name = unitDTO.Name;
                unit.Branch_Id = unitDTO.Branch_Id;

                await _unitOfWork._Unit.UpdateAsync(unit);
                return Response<UnitDTO>.Success(unitDTO, "Unit updated successfully");
            }
            catch (Exception ex)
            {
                return Response<UnitDTO>.Failure($"Failed to update unit: {ex.Message}", "500");
            }
        }

        private UnitDTO MapToDTO(Unit unit)
        {
            return new UnitDTO
            {
                Id_Unit = unit.Id_Unit,       // Map the Unit ID
                Name = unit.Name,             // Map the Unit name
                Branch_Id = unit.Branch_Id,   // Map the Branch ID
                //BranchName = unit.Branch?.Name // Map the Branch name (if Branch is included)
            };
        }
        private UnitWithBranchNameDTO MapBranchToDTO(Unit unit)
        {
            return new UnitWithBranchNameDTO
            {
                Id_Unit = unit.Id_Unit,       // Map the Unit ID
                Name = unit.Name,             // Map the Unit name
                Branch_Id = unit.Branch_Id,   // Map the Branch ID
                BranchName = unit.Branch?.Name // Map the Branch name (if Branch is included)
            };
        }
    }
}
