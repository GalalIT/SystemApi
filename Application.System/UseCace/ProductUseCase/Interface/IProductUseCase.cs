using Application.System.DTO;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.ProductUseCase.Interface
{
    public interface IProductUseCase
    {
        Task<Response<ProductDTO>> GetProductByIdAsync(int id);
        Task<Response> DeleteProductsAsync(int id);
        Task<Response<ProductBranchResponse>> GetProductsByBranchWithDepartments(int userBranchId, int? departmentId);
        Task<Response<ProductDTO>> UpdateProductWithUnitsAsync(int productId, CreateProductWithUnitsDTO productDto);
        Task<Response<ProductDTO>> CreateProductWithUnitsAsync(CreateProductWithUnitsDTO productDto);
        Task<Response<List<ProductWithDetailsDto>>> GetAllProductsWithDetailsAsync();
    }
}
