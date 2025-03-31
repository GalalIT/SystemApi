using Application.System.DTO;
using Application.System.Interface.IDepartmentOperation;
using Application.System.UseCace.DepartmentUseCase.Interface;
using Application.System.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.DepartmentUseCase.Implement
{
    public class DepartmentUseCase : IDepartmentUseCase
    {
        private readonly IAllDepartmentOperation _departmentService;
        private readonly ILogger<DepartmentUseCase> _logger;

        public DepartmentUseCase(
            IAllDepartmentOperation departmentService,
            ILogger<DepartmentUseCase> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        public async Task<Response<DepartmentDTO>> CreateDepartment(DepartmentDTO department)
        {
            try
            {
                // Additional validation
                if (await DepartmentNameExists(department.Name, department.Branch_Id))
                {
                    _logger.LogWarning("Duplicate department creation attempted: {Name} in branch {BranchId}",
                        department.Name, department.Branch_Id);
                    return Response<DepartmentDTO>.Failure("Department name already exists in this branch", "409");
                }

                var result = await _departmentService.CreateAsync(department);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Department created: ID {DepartmentId}", result.Data.Id_Department);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return Response<DepartmentDTO>.Failure("An error occurred while creating department", "500");
            }
        }

        public async Task<Response> DeleteDepartment(int id)
        {
            try
            {
                // Check for associated products/users first
                if (await HasAssociatedData(id))
                {
                    _logger.LogWarning("Attempt to delete department with associated data: {DepartmentId}", id);
                    return Response.Failure("Cannot delete department with associated products or users", "400");
                }

                var result = await _departmentService.DeleteAsync(id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Department deleted: ID {DepartmentId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {DepartmentId}", id);
                return Response.Failure("An error occurred while deleting department", "500");
            }
        }

        public async Task<Response<List<DepartmentDTO>>> GetAllDepartments()
        {
            try
            {
                var result = await _departmentService.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} departments", result.Data?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return Response<List<DepartmentDTO>>.Failure("Error retrieving departments", "500");
            }
        }

        public async Task<Response<DepartmentDTO>> GetDepartment(int id)
        {
            try
            {
                var result = await _departmentService.GetByIdAsync(id);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Department not found: ID {DepartmentId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department {DepartmentId}", id);
                return Response<DepartmentDTO>.Failure("Error retrieving department", "500");
            }
        }

        public async Task<Response<List<DepartmentDTO>>> GetAllDepartmentsByUserBranch(int branchId)
        {
            try
            {
                if (branchId <= 0)
                {
                    _logger.LogWarning("Invalid branch ID requested: {BranchId}", branchId);
                    return Response<List<DepartmentDTO>>.Failure("Invalid branch ID", "400");
                }

                var result = await _departmentService.GetAllDepartmentsByUserBranchAsync(branchId);
                _logger.LogInformation("Retrieved {Count} departments for branch {BranchId}",
                    result.Data?.Count ?? 0, branchId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments for branch {BranchId}", branchId);
                return Response<List<DepartmentDTO>>.Failure("Error retrieving departments", "500");
            }
        }

        public async Task<Response<List<DepartmentDTO>>> GetAllDepartmentsWithBranchInfo()
        {
            try
            {
                var response = await _departmentService.GetAllDepartmentIncludeToBranchAsync();

                if (!response.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve departments: {Message} (Status: {Status})",
                        response.Message, response.Status);
                    return Response<List<DepartmentDTO>>.Failure(response.Message, response.Status);
                }

                _logger.LogInformation("Retrieved {Count} departments with branch info", response.Data.Count);
                return Response<List<DepartmentDTO>>.Success(response.Data, response.Message, response.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments with branch info");
                return Response<List<DepartmentDTO>>.Failure(
                    "Error retrieving department details",
                    "500");
            }
        }

        public async Task<Response<DepartmentDTO>> UpdateDepartment(DepartmentDTO department)
        {
            try
            {
                // Check for name conflicts
                if (await DepartmentNameExists(department.Name, department.Branch_Id, department.Id_Department))
                {
                    _logger.LogWarning("Duplicate department name during update: {Name} in branch {BranchId}",
                        department.Name, department.Branch_Id);
                    return Response<DepartmentDTO>.Failure("Department name already exists in this branch", "409");
                }

                var result = await _departmentService.UpdateAsync(department);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Department updated: ID {DepartmentId}", department.Id_Department);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {DepartmentId}", department.Id_Department);
                return Response<DepartmentDTO>.Failure("An error occurred while updating department", "500");
            }
        }

        private async Task<bool> DepartmentNameExists(string name, int branchId, int? excludeId = null)
        {
            var departments = await _departmentService.GetAllAsync();
            return departments.Data?.Any(d =>
                d.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                d.Branch_Id == branchId &&
                (excludeId == null || d.Id_Department != excludeId.Value)) ?? false;
        }

        private async Task<bool> HasAssociatedData(int departmentId)
        {
            // Implement logic to check for products/users in this department
            // This would require additional services/repositories
            return false; // Placeholder
        }
    }
}
