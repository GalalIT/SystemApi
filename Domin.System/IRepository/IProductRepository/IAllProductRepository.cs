using Domin.System.Entities;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IProductRepository
{
    public interface IAllProductRepository : IAllBaseRepository<Product>
    {
        Task<List<Product>> GetAllIncludeToUnitAsync();
        Task<List<Product>> GetAllIncludeToDepartmentAsync();
        Task<List<Product>> GetAllProductsByUserBranchAsync(int userBranchId);
        Task<bool> HasRelatedRecords(int productId);
        Task<List<Product>> GetAllWithIncludesAsync();
    }
}
