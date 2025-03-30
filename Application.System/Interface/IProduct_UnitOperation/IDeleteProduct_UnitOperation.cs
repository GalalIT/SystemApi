using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProduct_UnitOperation
{
    public interface IDeleteProduct_UnitOperation : IBaseDeleteAsync<ProductUnitDTO>
    {
    }
}
 