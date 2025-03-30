using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProduct_UnitOperation
{
    public interface IGetAllProduct_UnitOperation : IBaseGetAllAsync<ProductUnitDTO>
    {
        Task<Response<List<ProductUnitDTO>>> GetAllIncludeProdDepAsync();

    }
}
 