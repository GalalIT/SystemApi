using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.ICompanyOperation
{
    public interface IAllCompanyOperation: IAddCompanyOperation, IDeleteCompanyOperation, IEditCompanyOperation, IGetAllCompanyOperation, IGetByIdCompanyOperation
    { 
    }
}
