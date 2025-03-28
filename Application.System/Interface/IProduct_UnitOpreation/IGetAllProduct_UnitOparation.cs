using Application.System.DTO;
using Application.System.Interface.IBaseInterface;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProduct_UnitOpreation
{
    public interface IGetAllProduct_UnitOparation : IBaseGetAllAsync<ProductUnitDTO>
    {
        Task<Response<List<ProductUnitDTO>>> GetAllIncludeProdDepAsync();

    }
}
 