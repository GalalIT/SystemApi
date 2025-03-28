using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.ICompanyOparation
{
    public interface IAllCompanyOparation: IAddCompanyOparation, IDeleteCompanyOparation, IEditeCompanyOparation, IGetAllCompanyOparation, IGetByIdCompanyOparation
    { 
    }
}
