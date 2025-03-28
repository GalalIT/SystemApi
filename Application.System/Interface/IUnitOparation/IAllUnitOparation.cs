
using Application.System.Interface.IUnitOparation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUnitOparation
{
    public interface IAllUnitOparation: IAddUnitOparation, IDeleteUnitOparation, IEditeUnitOparation, IGetAllUnitOparation, IGetByIdUnitOparation
    {
    }
}
