using Application.System.Interface.IDepartmentOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProductOperation
{
    public interface IAllProductOperation: IAddProductOperation, IDeleteProductOperation, IEditProductOperation, IGetAllProductOperation, IGetByIdProductOperation
    {
        Task<bool> HasRelatedRecords(int productId);
    }
}
 