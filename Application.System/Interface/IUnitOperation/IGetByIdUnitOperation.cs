using Application.System.Interface.IBaseOperation;
using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUnitOperation
{
    public interface IGetByIdUnitOperation : IBaseGetByIdAsync<UnitDTO>
    {
    }
}
