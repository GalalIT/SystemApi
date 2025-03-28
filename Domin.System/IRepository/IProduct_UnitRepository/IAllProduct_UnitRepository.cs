using Domin.System.Entities;
using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IProduct_UnitRepository
{
    public interface IAllProduct_UnitRepository : IAllBaseRepository<Product_Unit>
    {
        Task<List<Product_Unit>> GetAllIncludeProdDepAsync();
        Task<List<Product_Unit>> GetProductUnitsByProductIdAsync(int productId);
    }
}
