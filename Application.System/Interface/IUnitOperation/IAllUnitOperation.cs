
using Application.System.Interface.IUnitOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUnitOperation
{
    public interface IAllUnitOperation: IAddUnitOperation, IDeleteUnitOperation, IEditUnitOperation, IGetAllUnitOperation, IGetByIdUnitOperation
    {
    }
}
