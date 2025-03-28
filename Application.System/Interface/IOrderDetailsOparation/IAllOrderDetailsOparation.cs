
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IOrderDetailsOparation
{
    public interface IAllOrderDetailsOparation: IAddOrderDetailsOparation, IDeleteOrderDetailsOparation, IEditeOrderDetailsOparation, IGetAllOrderDetailsOparation, IGetByIdOrderDetailsOparation
    {
    }
}
  