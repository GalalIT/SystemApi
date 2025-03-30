using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IDepartmentOperation
{
    public interface IAllDepartmentOperation:IAddDepartmentOperation,IDeleteDepartmentOperation,IEditDepartmentOperation,IGetAllDepartmentOperation,IGetByIdDepartmentOperation
    {
    }
} 
