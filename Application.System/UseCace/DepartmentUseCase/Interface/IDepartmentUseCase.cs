using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.DepartmentUseCase.Interface
{
    public interface IDepartmentUseCase
    {
        Task<Response<DepartmentDTO>> CreateDepartment(DepartmentDTO department);
        Task<Response<DepartmentDTO>> UpdateDepartment(DepartmentDTO department);
        Task<Response> DeleteDepartment(int id);
        Task<Response<DepartmentDTO>> GetDepartment(int id);
        Task<Response<List<DepartmentDTO>>> GetAllDepartments();
        Task<Response<List<DepartmentWithBranchDTO>>> GetAllDepartmentsByUserBranch(int userBranchId);
        Task<Response<List<DepartmentWithBranchDTO>>> GetAllDepartmentsWithBranchInfo();
    }
}
