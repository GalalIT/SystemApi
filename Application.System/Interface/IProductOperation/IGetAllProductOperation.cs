using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProductOperation
{
    public interface IGetAllProductOperation : IBaseGetAllAsync<ProductDTO>
    {
        Task<Response<List<ProductWithUnitsDto>>> GetAllIncludeToUnitAsync();
        Task<Response<List<ProductDTO>>> GetAllIncludeToDepartmentAsync();
        Task<Response<List<ProductWithUnitsDto>>> GetAllProductsByUserBranchAsync(int userBranchId);
        Task<Response<ProductBranchResponse>> GetProductsByBranchWithDepartments(int userBranchId, int? departmentId);
        Task<Response<List<ProductWithDetailsDto>>> GetAllWithIncludesAsync();
    }
}
 