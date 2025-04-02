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
        Task<Response<List<Product>>> GetAllIncludeToUnitAsync();
        Task<Response<List<Product>>> GetAllIncludeToDepartmentAsync();
        Task<Response<List<Product>>> GetAllProductsByUserBranchAsync(int userBranchId);
        Task<Response<ProductBranchResponse>> GetProductsByBranchWithDepartments(int userBranchId, int? departmentId);
        Task<Response<List<ProductWithDetailsDto>>> GetAllWithIncludesAsync();
    }
}
 