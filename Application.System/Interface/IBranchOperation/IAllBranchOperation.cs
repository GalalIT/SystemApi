using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IBranchOperation
{
    public interface IAllBranchOperation: IAddBranchOperation, IDeleteBreanchOperation, IEditBreanchOperation, IGetAllBreanchOperation, IGetByIdBreanchOperation
    {
    }
}
