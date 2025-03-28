using Application.System.DTO;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IProduct_UnitOpreation
{
    public interface IAllProduct_UnitOparation : IAddProduct_UnitOparation, IDeleteProduct_UnitOparation, IEditProduct_UnitOparation, IGetAllProduct_UnitOparation, IGetByIdProduct_UnitOparation
    {
        Task<Response<List<ProductUnitDTO>>> GetProductUnitsByProductIdAsync(int productId);

    }
} 
 