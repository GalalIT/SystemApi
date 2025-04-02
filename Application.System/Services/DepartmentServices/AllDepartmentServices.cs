using Application.System.DTO;
using Application.System.Interface.IDepartmentOperation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.DepartmentServices
{
    public class AllDepartmentServices : IAllDepartmentOperation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllDepartmentServices(IUnitOfRepository unitOfRepository)
        {
            _unitOfWork = unitOfRepository;
        }

        public async Task<Response<DepartmentDTO>> CreateAsync(DepartmentDTO departmentDTO)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(departmentDTO.Name))
                    return Response<DepartmentDTO>.Failure("Department name is required", "400");

                if (departmentDTO.Branch_Id <= 0)
                    return Response<DepartmentDTO>.Failure("Branch ID is invalid", "400");

                // Map DTO to entity
                var department = new Department
                {
                    Name = departmentDTO.Name,
                    Description = departmentDTO.Description,
                    Branch_Id = departmentDTO.Branch_Id
                };

                await _unitOfWork._Department.AddAsync(department);

                // Update DTO with generated ID
                departmentDTO.Id_Department = department.Id_Department;
                return Response<DepartmentDTO>.Success(departmentDTO, "Department created successfully");
            }
            catch (Exception ex)
            {
                return Response<DepartmentDTO>.Failure($"Failed to create department: {ex.Message}", "500");
            }
        }
        public async Task<Response<DepartmentDTO>> UpdateAsync(DepartmentDTO departmentDTO)
        {
            try
            {
                var department = await _unitOfWork._Department.GetByIdAsync(departmentDTO.Id_Department);
                if (department == null)
                    return Response<DepartmentDTO>.Failure("Department not found", "404");

                // Validate required fields
                if (string.IsNullOrEmpty(departmentDTO.Name))
                    return Response<DepartmentDTO>.Failure("Department name is required", "400");

                if (departmentDTO.Branch_Id <= 0)
                    return Response<DepartmentDTO>.Failure("Branch ID is invalid", "400");

                // Update properties
                department.Name = departmentDTO.Name;
                department.Description = departmentDTO.Description;
                department.Branch_Id = departmentDTO.Branch_Id;

                await _unitOfWork._Department.UpdateAsync(department);
                return Response<DepartmentDTO>.Success(MapToDTO(department), "Department updated successfully");
            }
            catch (Exception ex)
            {
                return Response<DepartmentDTO>.Failure($"Failed to update department: {ex.Message}", "500");
            }
        }
        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var department = await _unitOfWork._Department.GetByIdAsync(id);
                if (department == null)
                    return Response.Failure("Department not found", "404");

                await _unitOfWork._Department.DeleteAsync(id);
                return Response.Success("Department deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete department: {ex.Message}", "500");
            }
        }
        public async Task<Response<DepartmentDTO>> GetByIdAsync(int id)
        {
            try
            {
                var department = await _unitOfWork._Department.GetByIdAsync(id);
                if (department == null)
                    return Response<DepartmentDTO>.Failure("Department not found", "404");

                return Response<DepartmentDTO>.Success(MapToDTO(department), "Department retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<DepartmentDTO>.Failure($"Failed to retrieve department: {ex.Message}", "500");
            }
        }
        public async Task<Response<List<DepartmentDTO>>> GetAllAsync()
        {
            try
            {
                var departments = await _unitOfWork._Department.GetAllAsync();
                var departmentDTOs = departments.Select(MapToDTO).ToList();
                return Response<List<DepartmentDTO>>.Success(departmentDTOs, "All departments retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<DepartmentDTO>>.Failure($"Failed to retrieve departments: {ex.Message}", "500");
            }
        }
        public async Task<Response<List<DepartmentWithBranchDTO>>> GetAllDepartmentIncludeToBranchAsync()
        {
            try
            {
                var departments = await _unitOfWork._Department.GetAllDepartmentIncludeToBranchAsync();

                if (departments == null || !departments.Any())
                {
                    return Response<List<DepartmentWithBranchDTO>>.Success(
                        new List<DepartmentWithBranchDTO>(),
                        "No departments found",
                        "200");
                }

                var departmentDTOs = departments.Select(MapToDTOWithBranch).ToList();
                return Response<List<DepartmentWithBranchDTO>>.Success(
                    departmentDTOs,
                    "Departments retrieved with branch relations",
                    "200");
            }
            catch (Exception ex)
            {
                return Response<List<DepartmentWithBranchDTO>>.Failure(
                    $"Failed to retrieve departments: {ex.Message}",
                    "500");
            }
        }
        public async Task<Response<List<DepartmentWithBranchDTO>>> GetAllDepartmentsByUserBranchAsync(int userBranchId)
        {
            try
            {
                if (userBranchId <= 0)
                    return Response<List<DepartmentWithBranchDTO>>.Failure("Invalid branch ID", "400");

                var departments = await _unitOfWork._Department.GetAllDepartmentsByUserBranchAsync(userBranchId);

                var departmentDTOs = departments.Select(MapToDTOWithBranch).ToList();

                return Response<List<DepartmentWithBranchDTO>>.Success(departmentDTOs, $"Retrieved {departmentDTOs.Count} departments for branch");
            }
            catch (Exception ex)
            {
                return Response<List<DepartmentWithBranchDTO>>.Failure($"Failed to retrieve departments: {ex.Message}", "500");
            }
        }
        private DepartmentWithBranchDTO MapToDTOWithBranch(Department department)
        {
            return new DepartmentWithBranchDTO
            {
                // Base DepartmentDTO properties
                Id_Department = department.Id_Department,
                Name = department.Name,
                Description = department.Description,
                Branch_Id = department.Branch_Id,

                // Additional branch info
                BranchName = department.Branch?.Name,
                BranchAddress = department.Branch?.Address
            };
        }
        private DepartmentDTO MapToDTO(Department department)
        {
            return new DepartmentDTO
            {
                Id_Department = department.Id_Department,
                Name = department.Name,
                Description = department.Description,
                Branch_Id = department.Branch_Id,
            };
        }
        //private DepartmentDTO MapToDTOWithBranch(Department department)
        //{
        //    var dto = MapToDTO(department);
        //    // Add branch name if included in the query
        //    dto.BranchName = department.Branch?.Name;
        //    return dto;
        //}
    }
}
