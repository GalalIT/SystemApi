using Application.System.DTO;
using Application.System.Interface.IBranchOperation;
using Application.System.Interface.IDepartmentOperation;
using Application.System.Interface.IProductOperation;
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
        private readonly IAllBranchOperation _branchService;
        private readonly IAllProductOperation _productOperation;
        public DepartmentUseCase(
            IAllDepartmentOperation departmentService,
            IAllBranchOperation branchService, 
            IAllProductOperation productOperation,
            ILogger<DepartmentUseCase> logger)
        {
            _departmentService = departmentService;
            _branchService = branchService;
            _productOperation = productOperation;
            _logger = logger;
        }

        public async Task<Response<DepartmentDTO>> CreateDepartment(DepartmentDTO department)
        {
            try
            {
                var branchResponse = await _branchService.GetByIdAsync(department.Branch_Id);
                if (!branchResponse.Succeeded)
                {
                    _logger.LogWarning("----------------- Attempt to create department with non-existent branch: {BranchId} ( DepartmentUseCase )=> ( CreateDepartment )  -----------------", department.Branch_Id);
                    return Response<DepartmentDTO>.Failure("Specified branch does not exist", "400");
                }
                // Additional validation
                if (await DepartmentNameExists(department.Name, department.Branch_Id))
                {
                    _logger.LogWarning("----------------- Duplicate department creation attempted: {Name} in branch {BranchId} ( DepartmentUseCase )=> ( CreateDepartment ) -----------------",
                        department.Name, department.Branch_Id);
                    return Response<DepartmentDTO>.Failure("Department name already exists in this branch", "409");
                }

                var result = await _departmentService.CreateAsync(department);

                if (result.Succeeded)
                {
                    _logger.LogInformation("----------------- Department created: ID {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment ) -----------------", result.Data.Id_Department);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "----------------- Error creating department ( DepartmentUseCase )=> ( CreateDepartment ) -----------------");
                return Response<DepartmentDTO>.Failure("An error occurred while creating department", "500");
            }
        }

        public async Task<Response> DeleteDepartment(int id)
        {
            try
            {
                // Check for associated products/users first
                if (await AnyProductsInDepartmentAsync(id))
                {
                    _logger.LogWarning("Attempt to delete department with associated data: {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment )", id);
                    return Response.Failure("Cannot delete department with associated products or users", "400");
                }

                var result = await _departmentService.DeleteAsync(id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Department deleted: ID {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment ) ", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment )", id);
                return Response.Failure("An error occurred while deleting department", "500");
            }
        }

        public async Task<Response<List<DepartmentDTO>>> GetAllDepartments()
        {
            try
            {
                var result = await _departmentService.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} departments ( DepartmentUseCase )=> ( CreateDepartment )", result.Data?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments ( DepartmentUseCase )=> ( CreateDepartment )");
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
                    _logger.LogWarning("Department not found: ID {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment )", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment )", id);
                return Response<DepartmentDTO>.Failure("Error retrieving department", "500");
            }
        }

        public async Task<Response<List<DepartmentWithBranchDTO>>> GetAllDepartmentsByUserBranch(int branchId)
        {
            try
            {
                if (branchId <= 0)
                {
                    _logger.LogWarning("Invalid branch ID requested: {BranchId} ( DepartmentUseCase )=> ( CreateDepartment ) ", branchId);
                    return Response<List<DepartmentWithBranchDTO>>.Failure("Invalid branch ID", "400");
                }

                var result = await _departmentService.GetAllDepartmentsByUserBranchAsync(branchId);
                _logger.LogInformation("Retrieved {Count} departments for branch {BranchId} ( DepartmentUseCase )=> ( CreateDepartment )",
                    result.Data?.Count ?? 0, branchId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments for branch {BranchId} ( DepartmentUseCase )=> ( CreateDepartment )", branchId);
                return Response<List<DepartmentWithBranchDTO>>.Failure("Error retrieving departments", "500");
            }
        }

        public async Task<Response<List<DepartmentWithBranchDTO>>> GetAllDepartmentsWithBranchInfo()
        {
            try
            {
                var response = await _departmentService.GetAllDepartmentIncludeToBranchAsync();
                Console.WriteLine(response);
                if (!response.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve departments: {Message} (Status: {Status}) ( DepartmentUseCase )=> ( CreateDepartment )",
                        response.Message, response.Status);
                    return Response<List<DepartmentWithBranchDTO>>.Failure(response.Message, response.Status);
                }

                _logger.LogInformation("Retrieved {Count} departments with branch info ( DepartmentUseCase )=> ( CreateDepartment )", response.Data.Count);
                return Response<List<DepartmentWithBranchDTO>>.Success(response.Data, response.Message, response.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments with branch info ( DepartmentUseCase )=> ( CreateDepartment )");
                return Response<List<DepartmentWithBranchDTO>>.Failure(
                    "Error retrieving department details",
                    "500");
            }
        }

        public async Task<Response<DepartmentDTO>> UpdateDepartment(DepartmentDTO department)
        {
            try
            {
                var branchResponse = await _branchService.GetByIdAsync(department.Branch_Id);
                if (!branchResponse.Succeeded)
                {
                    _logger.LogWarning("Attempt to update department with non-existent branch: {BranchId} ( DepartmentUseCase )=> ( CreateDepartment )", department.Branch_Id);
                    return Response<DepartmentDTO>.Failure("Specified branch does not exist", "400");
                }
                // Check for name conflicts
                if (await DepartmentNameExists(department.Name, department.Branch_Id, department.Id_Department))
                {
                    _logger.LogWarning("Duplicate department name during update: {Name} in branch {BranchId} ( DepartmentUseCase )=> ( CreateDepartment )",
                        department.Name, department.Branch_Id);
                    return Response<DepartmentDTO>.Failure("Department name already exists in this branch", "409");
                }

                var result = await _departmentService.UpdateAsync(department);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Department updated: ID {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment )", department.Id_Department);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {DepartmentId} ( DepartmentUseCase )=> ( CreateDepartment )", department.Id_Department);
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

        public async Task<bool> AnyProductsInDepartmentAsync(int departmentId)
        {
            try
            {
                return await _productOperation.AnyProductsInDepartmentAsync(departmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for products in department {DepartmentId}", departmentId);
                // If we can't verify, assume there are products to prevent accidental deletion
                return true;
            }
        }
    }
}
