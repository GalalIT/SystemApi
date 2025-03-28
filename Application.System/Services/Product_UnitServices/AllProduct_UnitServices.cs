using Application.System.DTO;
using Application.System.Interface.IProduct_UnitOpreation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.Product_UnitServices
{
    public class AllProduct_UnitServices : IAllProduct_UnitOparation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllProduct_UnitServices(IUnitOfRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<ProductUnitDTO>> CreateAsync(ProductUnitDTO productUnitDTO)
        {
            try
            {
                // Validation
                if (productUnitDTO.ProductId <= 0)
                    return Response<ProductUnitDTO>.Failure("Product ID is required", "400");

                if (productUnitDTO.UnitId <= 0)
                    return Response<ProductUnitDTO>.Failure("Unit ID is required", "400");

                if (productUnitDTO.SpecialPrice <= 0)
                    return Response<ProductUnitDTO>.Failure("Price must be greater than 0", "400");

                // Map DTO to entity
                var productUnit = new Product_Unit
                {
                    ProductId = productUnitDTO.ProductId,
                    UnitId = productUnitDTO.UnitId,
                    SpecialPrice = productUnitDTO.SpecialPrice
                };

                await _unitOfWork._ProductUnit.AddAsync(productUnit);

                // Update DTO with generated ID
                productUnitDTO.Id = productUnit.Id;
                return Response<ProductUnitDTO>.Success(productUnitDTO, "Product unit created successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductUnitDTO>.Failure($"Failed to create product unit: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                var productUnit = await _unitOfWork._ProductUnit.GetByIdAsync(id);
                if (productUnit == null)
                    return Response.Failure("Product unit not found", "404");

                await _unitOfWork._ProductUnit.DeleteAsync(id);
                return Response.Success("Product unit deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete product unit: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<ProductUnitDTO>>> GetAllAsync()
        {
            try
            {
                var productUnits = await _unitOfWork._ProductUnit.GetAllAsync();
                var productUnitDTOs = productUnits.Select(MapToDTO).ToList();
                return Response<List<ProductUnitDTO>>.Success(productUnitDTOs, "All product units retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<ProductUnitDTO>>.Failure($"Failed to retrieve product units: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<ProductUnitDTO>>> GetAllIncludeProdDepAsync()
        {
            try
            {
                var productUnits = await _unitOfWork._ProductUnit.GetAllIncludeProdDepAsync();
                var productUnitDTOs = productUnits.Select(pu => new ProductUnitDTO
                {
                    Id = pu.Id,
                    ProductId = pu.ProductId,
                    UnitId = pu.UnitId,
                    SpecialPrice = pu.SpecialPrice,
                    // Add these if you want to include them in DTO
                    // ProductName = pu.Product?.Name,
                    // UnitName = pu.Unit?.Name
                }).ToList();

                return Response<List<ProductUnitDTO>>.Success(productUnitDTOs, "Product units with details retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<ProductUnitDTO>>.Failure($"Failed to retrieve product units with details: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductUnitDTO>> GetByIdAsync(int id)
        {
            try
            {
                var productUnit = await _unitOfWork._ProductUnit.GetByIdAsync(id);
                if (productUnit == null)
                    return Response<ProductUnitDTO>.Failure("Product unit not found", "404");

                return Response<ProductUnitDTO>.Success(MapToDTO(productUnit), "Product unit retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductUnitDTO>.Failure($"Failed to retrieve product unit: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<ProductUnitDTO>>> GetProductUnitsByProductIdAsync(int productId)
        {
            try
            {
                if (productId <= 0)
                    return Response<List<ProductUnitDTO>>.Failure("Invalid product ID", "400");

                var productUnits = await _unitOfWork._ProductUnit.GetProductUnitsByProductIdAsync(productId);
                var productUnitDTOs = productUnits.Select(MapToDTO).ToList();

                return Response<List<ProductUnitDTO>>.Success(productUnitDTOs, $"Product units for product {productId} retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<ProductUnitDTO>>.Failure($"Failed to retrieve product units: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductUnitDTO>> UpdateAsync(ProductUnitDTO productUnitDTO)
        {
            try
            {
                var productUnit = await _unitOfWork._ProductUnit.GetByIdAsync(productUnitDTO.Id);
                if (productUnit == null)
                    return Response<ProductUnitDTO>.Failure("Product unit not found", "404");

                // Validate required fields
                if (productUnitDTO.ProductId <= 0)
                    return Response<ProductUnitDTO>.Failure("Product ID is required", "400");

                if (productUnitDTO.UnitId <= 0)
                    return Response<ProductUnitDTO>.Failure("Unit ID is required", "400");

                if (productUnitDTO.SpecialPrice <= 0)
                    return Response<ProductUnitDTO>.Failure("Price must be greater than 0", "400");

                // Update properties
                productUnit.ProductId = productUnitDTO.ProductId;
                productUnit.UnitId = productUnitDTO.UnitId;
                productUnit.SpecialPrice = productUnitDTO.SpecialPrice;

                await _unitOfWork._ProductUnit.UpdateAsync(productUnit);
                return Response<ProductUnitDTO>.Success(MapToDTO(productUnit), "Product unit updated successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductUnitDTO>.Failure($"Failed to update product unit: {ex.Message}", "500");
            }
        }

        private ProductUnitDTO MapToDTO(Product_Unit productUnit)
        {
            return new ProductUnitDTO
            {
                Id = productUnit.Id,
                ProductId = productUnit.ProductId,
                UnitId = productUnit.UnitId,
                SpecialPrice = productUnit.SpecialPrice
            };
        }
    }
}
