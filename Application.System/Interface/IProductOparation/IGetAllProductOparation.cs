using Application.System.DTO;
using Application.System.Interface.IBaseInterface;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProductOparation
{
    public interface IGetAllProductOparation : IBaseGetAllAsync<ProductDTO>
    {
        Task<Response<List<Product>>> GetAllIncludeToUnitAsync();
        Task<Response<List<Product>>> GetAllIncludeToDepartmentAsync();
        Task<Response<List<Product>>> GetAllProductsByUserBranchAsync(int userBranchId);
        Task<Response<ProductBranchResponse>> GetProductsByBranchWithDepartments(int userBranchId, int? departmentId);
    }
}
 