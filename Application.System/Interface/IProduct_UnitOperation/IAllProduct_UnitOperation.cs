using Application.System.DTO;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProduct_UnitOperation
{
    public interface IAllProduct_UnitOperation : IAddProduct_UnitOperation, IDeleteProduct_UnitOperation, IEditProduct_UnitOperation, IGetAllProduct_UnitOperation, IGetByIdProduct_UnitOperation
    {
        Task<Response<List<ProductUnitDTO>>> GetProductUnitsByProductIdAsync(int productId);

    }
} 
 