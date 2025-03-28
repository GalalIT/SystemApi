using Application.System.Interface.IDepartmentOparation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProductOparation
{
    public interface IAllProductOparation: IAddProductOparation, IDeleteProductOparation, IEditProductOparation, IGetAllProductOparation, IGetByIdProductOparation
    {
        Task<bool> HasRelatedRecords(int productId);
    }
}
 