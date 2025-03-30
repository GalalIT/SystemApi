using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IOrderOperation
{
    public interface IAllOrderOperation: IAddOrderOperation, IDeleteOrderOperation, IEditOrderOperation, IGetAllOrderOperation, IGetByIdOrderOperation
    {
    }
}
 