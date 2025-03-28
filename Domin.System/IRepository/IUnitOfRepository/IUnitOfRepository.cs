
using Domin.System.IRepository.IBranchRepository;
using Domin.System.IRepository.IProduct_UnitRepository;
using Domin.System.IRepository.ICompanyRepository;
using Domin.System.IRepository.IDepartmentRepository;
using Domin.System.IRepository.IOrderDetailsRepository;
using Domin.System.IRepository.IOrderRepository;
using Domin.System.IRepository.IProductRepository;
using Domin.System.IRepository.IUnitRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IUnitOfRepository
{
    public interface IUnitOfRepository
    {
        IAllBranchRepository _Branch { get; }
        IAllCompanyRepository _Company { get; }
        IAllDepartmentRepository _Department { get; }
        IAllOrderDetailsRepository _OrderDetails { get; }
        IAllOrderRepository _Order { get; }
        IAllProductRepository _Product { get; }
        IAllUnitRepository _Unit { get; }
        IAllProduct_UnitRepository _ProductUnit { get; }
    }
}
