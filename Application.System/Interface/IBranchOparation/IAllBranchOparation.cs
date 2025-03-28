using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IBranchOparation
{
    public interface IAllBranchOparation: IAddBranchOparation, IDeleteBreanchOparation, IEditBreanchOparation, IGetAllBreanchOparation, IGetByIdBreanchOparation
    {
    }
}
