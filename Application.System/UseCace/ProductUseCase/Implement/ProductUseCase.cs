using Application.System.DTO;
using Application.System.Interface.IOrderOperation;
using Application.System.Interface.IProductOperation;
using Application.System.UseCace.ProductUseCase.Interface;
using Application.System.Utility;
using Domin.System.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.ProductUseCase.Implement
{
    public class ProductUseCase : IProductUseCase
    {
        private readonly IAllProductOperation _productOperation;
         private readonly ILogger<ProductUseCase> _logger; // Uncomment if you have logging
        private readonly IAllOrderOperation _orderOperation;
        public ProductUseCase(IAllProductOperation productOperation , 
            ILogger<ProductUseCase> logger ,
            IAllOrderOperation orderOperation)
        {
            _productOperation = productOperation;
            _orderOperation = orderOperation;
            _logger = logger;
        }
        public async Task<Response<ProductDTO>> GetProductByIdAsync(int id)
        {
            try
            {
                return await _productOperation.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Failure($"Failed to get product: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteProductsAsync(int id)
        {
            try
            {
                if (await _orderOperation.ProductExistsInAnyOrderAsync(id))
                {
                    _logger.LogWarning("Attempt to delete product with associated orders: {ProductId}", id);
                    return Response.Failure("Cannot delete product that exists in orders", "400");
                }
                var product = await _productOperation.GetByIdAsync(id);
                if (!product.Succeeded)
                    return Response.Failure("Product not found", "404");

                return await _productOperation.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete product: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductBranchResponse>> GetProductsByBranchWithDepartments(int userBranchId, int? departmentId)
        {
            try
            {
                if (userBranchId <= 0)
                    return Response<ProductBranchResponse>.Failure("Invalid branch ID", "400");

                return await _productOperation.GetProductsByBranchWithDepartments(userBranchId, departmentId);
            }
            catch (Exception ex)
            {
                return Response<ProductBranchResponse>.Failure($"Failed to retrieve products: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductDTO>> UpdateProductWithUnitsAsync(int productId, CreateProductWithUnitsDTO productDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return Response<ProductDTO>.Failure("Product name is required", "400");

                if (productDto.Department_Id <= 0)
                    return Response<ProductDTO>.Failure("Department ID is required", "400");

                if (productDto.Price <= 0)
                    return Response<ProductDTO>.Failure("Price must be greater than 0", "400");

                if (productDto.UnitIds == null || productDto.SpecialPrices == null)
                    return Response<ProductDTO>.Failure("Unit details are required", "400");

                if (productDto.UnitIds.Count != productDto.SpecialPrices.Count)
                    return Response<ProductDTO>.Failure("Mismatched unit and price data", "400");

                return await _productOperation.UpdateProductWithUnitsAsync(productId, productDto);
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Failure($"Failed to update product: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductDTO>> CreateProductWithUnitsAsync(CreateProductWithUnitsDTO productDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return Response<ProductDTO>.Failure("Product name is required", "400");

                if (productDto.Department_Id <= 0)
                    return Response<ProductDTO>.Failure("Department ID is required", "400");

                if (productDto.Price <= 0)
                    return Response<ProductDTO>.Failure("Price must be greater than 0", "400");

                if (productDto.UnitIds == null || productDto.SpecialPrices == null)
                    return Response<ProductDTO>.Failure("Unit details are required", "400");

                if (productDto.UnitIds.Count != productDto.SpecialPrices.Count)
                    return Response<ProductDTO>.Failure("Mismatched unit and price data", "400");

                if (productDto.UnitIds.Count == 0)
                    return Response<ProductDTO>.Failure("At least one unit must be specified", "400");

                return await _productOperation.CreateProductWithUnitsAsync(productDto);
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Failure($"Failed to create product: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<ProductWithDetailsDto>>> GetAllProductsWithDetailsAsync()
        {
            try
            {
                return await _productOperation.GetAllWithIncludesAsync();
            }
            catch (Exception ex)
            {
                return Response<List<ProductWithDetailsDto>>.Failure(
                    $"Failed to retrieve product details: {ex.Message}",
                    "500");
            }
        }
    }
}
