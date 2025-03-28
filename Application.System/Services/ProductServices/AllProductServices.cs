using Application.System.DTO;
using Application.System.Interface.IProductOparation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.ProductServices
{
    public class AllProductServices : IAllProductOparation
    {
        private readonly IUnitOfRepository _unitOfWork;

        public AllProductServices(IUnitOfRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<ProductBranchResponse>> GetProductsByBranchWithDepartments(int userBranchId, int? departmentId)
        {
            try
            {
                // Validate branch ID
                if (userBranchId <= 0)
                    return Response<ProductBranchResponse>.Failure("Invalid branch ID", "400");

                // Get all products for the branch
                var products = await _unitOfWork._Product.GetAllProductsByUserBranchAsync(userBranchId);

                // Filter by department if specified
                if (departmentId.HasValue && departmentId.Value != 0)
                {
                    products = products.Where(p => p.Department_Id == departmentId.Value).ToList();
                }

                // Get all departments for the branch
                var departments = await _unitOfWork._Department.GetAllDepartmentsByUserBranchAsync(userBranchId);

                // Map to DTOs
                var productDTOs = products.Select(p => new ProductDTO
                {
                    Id_Product = p.Id_Product,
                    Name = p.Name,
                    Department_Id = p.Department_Id,
                    DepartmentName = p.Department?.Name,
                    Price = p.Price,
                    IsActive = p.IsActive
                }).ToList();

                var departmentDTOs = departments.Select(d => new DepartmentDTO
                {
                    Id_Department = d.Id_Department,
                    Name = d.Name,
                    Description = d.Description,
                    Branch_Id = d.Branch_Id
                }).ToList();

                // Create response
                var response = new ProductBranchResponse
                {
                    Products = productDTOs,
                    Departments = departmentDTOs,
                    SelectedDepartmentId = departmentId
                };

                return Response<ProductBranchResponse>.Success(response, "Products retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductBranchResponse>.Failure($"Failed to retrieve products: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductDTO>> CreateAsync(ProductDTO productDTO)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(productDTO.Name))
                    return Response<ProductDTO>.Failure("Product name is required", "400");

                if (productDTO.Department_Id <= 0)
                    return Response<ProductDTO>.Failure("Department ID is required", "400");

                if (productDTO.Price <= 0)
                    return Response<ProductDTO>.Failure("Price must be greater than 0", "400");

                // Map DTO to entity
                var product = new Product
                {
                    Name = productDTO.Name,
                    Department_Id = productDTO.Department_Id,
                    Price = productDTO.Price,
                    IsActive = productDTO.IsActive ?? true // Default to true if not specified
                };

                await _unitOfWork._Product.AddAsync(product);

                // Update DTO with generated ID
                productDTO.Id_Product = product.Id_Product;
                return Response<ProductDTO>.Success(productDTO, "Product created successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Failure($"Failed to create product: {ex.Message}", "500");
            }
        }

        public async Task<Response> DeleteAsync(int id)
        {
            try
            {
                // First check if product has related records
                bool hasRelations = await HasRelatedRecords(id);
                if (hasRelations)
                    return Response.Failure("Cannot delete product with related records", "400");

                var product = await _unitOfWork._Product.GetByIdAsync(id);
                if (product == null)
                    return Response.Failure("Product not found", "404");

                await _unitOfWork._Product.DeleteAsync(id);
                return Response.Success("Product deleted successfully");
            }
            catch (Exception ex)
            {
                return Response.Failure($"Failed to delete product: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<ProductDTO>>> GetAllAsync()
        {
            try
            {
                var products = await _unitOfWork._Product.GetAllAsync();
                var productDTOs = products.Select(MapToDTO).ToList();
                return Response<List<ProductDTO>>.Success(productDTOs, "All products retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<List<ProductDTO>>.Failure($"Failed to retrieve products: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<Product>>> GetAllIncludeToDepartmentAsync()
        {
            try
            {
                var products = await _unitOfWork._Product.GetAllIncludeToDepartmentAsync();
                return Response<List<Product>>.Success(products, "Products with department details retrieved");
            }
            catch (Exception ex)
            {
                return Response<List<Product>>.Failure($"Failed to retrieve products: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<Product>>> GetAllIncludeToUnitAsync()
        {
            try
            {
                var products = await _unitOfWork._Product.GetAllIncludeToUnitAsync();
                return Response<List<Product>>.Success(products, "Products with unit details retrieved");
            }
            catch (Exception ex)
            {
                return Response<List<Product>>.Failure($"Failed to retrieve products: {ex.Message}", "500");
            }
        }

        public async Task<Response<List<Product>>> GetAllProductsByUserBranchAsync(int userBranchId)
        {
            try
            {
                if (userBranchId <= 0)
                    return Response<List<Product>>.Failure("Invalid branch ID", "400");

                var products = await _unitOfWork._Product.GetAllProductsByUserBranchAsync(userBranchId);
                return Response<List<Product>>.Success(products, $"Products for branch {userBranchId} retrieved");
            }
            catch (Exception ex)
            {
                return Response<List<Product>>.Failure($"Failed to retrieve products: {ex.Message}", "500");
            }
        }

        public async Task<Response<ProductDTO>> GetByIdAsync(int id)
        {
            try
            {
                var product = await _unitOfWork._Product.GetByIdAsync(id);
                if (product == null)
                    return Response<ProductDTO>.Failure("Product not found", "404");

                return Response<ProductDTO>.Success(MapToDTO(product), "Product retrieved successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Failure($"Failed to retrieve product: {ex.Message}", "500");
            }
        }

        public async Task<bool> HasRelatedRecords(int productId)
        {
            try
            {
                // Check if product has any related Product_Unit records
                var productUnits = await _unitOfWork._ProductUnit.GetProductUnitsByProductIdAsync(productId);
                return productUnits.Any();
            }
            catch
            {
                // If there's an error checking, assume it has relations to be safe
                return true;
            }
        }

        public async Task<Response<ProductDTO>> UpdateAsync(ProductDTO productDTO)
        {
            try
            {
                var product = await _unitOfWork._Product.GetByIdAsync(productDTO.Id_Product);
                if (product == null)
                    return Response<ProductDTO>.Failure("Product not found", "404");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(productDTO.Name))
                    return Response<ProductDTO>.Failure("Product name is required", "400");

                if (productDTO.Department_Id <= 0)
                    return Response<ProductDTO>.Failure("Department ID is required", "400");

                if (productDTO.Price <= 0)
                    return Response<ProductDTO>.Failure("Price must be greater than 0", "400");

                // Update properties
                product.Name = productDTO.Name;
                product.Department_Id = productDTO.Department_Id;
                product.Price = productDTO.Price;
                product.IsActive = productDTO.IsActive ?? product.IsActive;

                await _unitOfWork._Product.UpdateAsync(product);
                return Response<ProductDTO>.Success(MapToDTO(product), "Product updated successfully");
            }
            catch (Exception ex)
            {
                return Response<ProductDTO>.Failure($"Failed to update product: {ex.Message}", "500");
            }
        }

        private ProductDTO MapToDTO(Product product)
        {
            return new ProductDTO
            {
                Id_Product = product.Id_Product,
                Name = product.Name,
                Department_Id = product.Department_Id,
                Price = product.Price,
                IsActive = product.IsActive
            };
        }
    }
}
