using Application.System.DTO;
using Application.System.Interface.IBaseOperation;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProductOperation
{
    public interface IAddProductOperation : IBaseCreateAsync<ProductDTO>
    {
        Task<Response<ProductDTO>> CreateWithUnitsAsync(CreateProductWithUnitsDTO productDto);

    }
}
  