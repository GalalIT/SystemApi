using Application.System.DTO;
using Application.System.Interface.IBaseInterface;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IDepartmentOparation
{
    public interface IGetAllDepartmentOparation : IBaseGetAllAsync<DepartmentDTO>
    {
        Task<Response<List<DepartmentDTO>>> GetAllDepartmentsByUserBranchAsync(int userBranchId); // New method
        Task<Response<List<DepartmentDTO>>> GetAllDepartmentIncludeToBranchAsync();

    }
}
 