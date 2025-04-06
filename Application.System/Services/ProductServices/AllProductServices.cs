using Application.System.DTO;
using Application.System.Interface.IProduct_UnitOperation;
using Application.System.Interface.IProductOperation;
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
    public class AllProductServices : IAllProductOperation
    {
        private readonly IUnitOfRepository _unitOfWork;
        private readonly IAllProduct_UnitOperation _productUnitService;

        public AllProductServices(
            IUnitOfRepository unitOfWork,
            IAllProduct_UnitOperation productUnitService)
        {
            _unitOfWork = unitOfWork;
            _productUnitService = productUnitService;
        }
        public async Task<Response<ProductDTO>> CreateProductWithUnitsAsync(CreateProductWithUnitsDTO productDto)
        {
            try
            {
                // Validate main product data
                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return await Response<ProductDTO>.FailureAsync("Product name is required", "400");

                if (productDto.Department_Id <= 0)
                    return await Response<ProductDTO>.FailureAsync("Department ID is required", "400");

                if (productDto.Price <= 0)
                    return await Response<ProductDTO>.FailureAsync("Price must be greater than 0", "400");

                // Validate units data
                if (productDto.UnitIds == null || productDto.SpecialPrices == null)
                    return await Response<ProductDTO>.FailureAsync("Unit details are required", "400");

                if (productDto.UnitIds.Count != productDto.SpecialPrices.Count)
                    return await Response<ProductDTO>.FailureAsync("Mismatched unit and price data", "400");

                if (productDto.UnitIds.Count == 0)
                    return await Response<ProductDTO>.FailureAsync("At least one unit must be specified", "400");

                // Create the main product
                var product = new Product
                {
                    Name = productDto.Name,
                    Department_Id = productDto.Department_Id,
                    Price = productDto.Price,
                    IsActive = productDto.IsActive ?? true
                };

                await _unitOfWork._Product.AddAsync(product);
                var productId = product.Id_Product;

                // Create product units
                var unitResults = new List<Response<ProductUnitDTO>>();
                for (int i = 0; i < productDto.UnitIds.Count; i++)
                {
                    var productUnitDto = new ProductUnitDTO
                    {
                        ProductId = productId,
                        UnitId = productDto.UnitIds[i],
                        SpecialPrice = productDto.SpecialPrices[i]
                    };

                    var result = await _productUnitService.CreateAsync(productUnitDto);
                    unitResults.Add(result);
                }

                // Check for any failures in unit creation
                var failedUnits = unitResults.Where(r => !r.Succeeded).ToList();
                if (failedUnits.Any())
                {
                    // Rollback product creation if any unit failed
                    await _unitOfWork._Product.DeleteAsync(productId);

                    return await Response<ProductDTO>.FailureAsync(
                        $"Product created but failed to add {failedUnits.Count} units. First error: {failedUnits.First().Message}",
                        "500");
                }

                // Map to DTO for response
                var productDtoResponse = new ProductDTO
                {
                    Id_Product = product.Id_Product,
                    Name = product.Name,
                    Department_Id = product.Department_Id,
                    Price = product.Price,
                    IsActive = product.IsActive
                };

                return await Response<ProductDTO>.SuccessAsync(productDtoResponse, "Product with units created successfully");
            }
            catch (Exception ex)
            {
                return await Response<ProductDTO>.FailureAsync($"Failed to create product: {ex.Message}", "500");
            }
        }
        public async Task<Response<ProductDTO>> UpdateProductWithUnitsAsync(int productId, CreateProductWithUnitsDTO productDto)
        {
            try
            {
                // Validate main product data
                if (string.IsNullOrWhiteSpace(productDto.Name))
                    return await Response<ProductDTO>.FailureAsync("Product name is required", "400");

                if (productDto.Department_Id <= 0)
                    return await Response<ProductDTO>.FailureAsync("Department ID is required", "400");

                if (productDto.Price <= 0)
                    return await Response<ProductDTO>.FailureAsync("Price must be greater than 0", "400");

                if (productDto.UnitIds == null || productDto.SpecialPrices == null)
                    return await Response<ProductDTO>.FailureAsync("Unit details are required", "400");

                if (productDto.UnitIds.Count != productDto.SpecialPrices.Count)
                    return await Response<ProductDTO>.FailureAsync("Mismatched unit and price data", "400");

                // Check if product exists
                var product = await _unitOfWork._Product.GetByIdAsync(productId);
                if (product == null)
                    return await Response<ProductDTO>.FailureAsync("Product not found", "404");

                // Update product details
                product.Name = productDto.Name;
                product.Department_Id = productDto.Department_Id;
                product.Price = productDto.Price;
                product.IsActive = productDto.IsActive ?? true;

                await _unitOfWork._Product.UpdateAsync(product);

                // Get existing product units directly from repository
                var existingUnits = await _unitOfWork._ProductUnit.GetProductUnitsByProductIdAsync(productId);
                var existingUnitIds = existingUnits.Select(u => u.UnitId).ToList();

                var unitResults = new List<Response<ProductUnitDTO>>();

                // Process each unit in the DTO
                for (int i = 0; i < productDto.UnitIds.Count; i++)
                {
                    var unitId = productDto.UnitIds[i];
                    var specialPrice = productDto.SpecialPrices[i];

                    var existingUnit = existingUnits.FirstOrDefault(u => u.UnitId == unitId);
                    if (existingUnit != null)
                    {
                        // Update existing unit
                        var updateDto = new ProductUnitDTO
                        {
                            Id = existingUnit.Id,
                            ProductId = productId,
                            UnitId = unitId,
                            SpecialPrice = specialPrice
                        };
                        var updateResult = await _productUnitService.UpdateAsync(updateDto);
                        unitResults.Add(updateResult);
                    }
                    else
                    {
                        // Create new unit
                        var newUnit = new ProductUnitDTO
                        {
                            ProductId = productId,
                            UnitId = unitId,
                            SpecialPrice = specialPrice
                        };
                        var createResult = await _productUnitService.CreateAsync(newUnit);
                        unitResults.Add(createResult);
                    }
                }

                // Find units to remove (exist in DB but not in DTO)
                var unitsToRemove = existingUnits
                    .Where(u => !productDto.UnitIds.Contains(u.UnitId))
                    .ToList();

                foreach (var unit in unitsToRemove)
                {
                    await _productUnitService.DeleteAsync(unit.Id);
                }

                // Check for any failures in unit operations
                var failedUnits = unitResults.Where(r => !r.Succeeded).ToList();
                if (failedUnits.Any())
                {
                    return await Response<ProductDTO>.FailureAsync(
                        $"Product updated but {failedUnits.Count} unit operations failed. First error: {failedUnits.First().Message}",
                        "500");
                }

                // Return updated product
                var responseDto = new ProductDTO
                {
                    Id_Product = product.Id_Product,
                    Name = product.Name,
                    Department_Id = product.Department_Id,
                    Price = product.Price,
                    IsActive = product.IsActive
                };

                return await Response<ProductDTO>.SuccessAsync(responseDto, "Product with units updated successfully");
            }
            catch (Exception ex)
            {
                return await Response<ProductDTO>.FailureAsync($"Failed to update product: {ex.Message}", "500");
            }
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

        public async Task<Response<List<ProductWithDetailsDto>>> GetAllWithIncludesAsync()
        {
            try
            {
                // Get products with all includes from repository
                var products = await _unitOfWork._Product.GetAllWithIncludesAsync();

                // Map to DTO
                var result = products.Select(p => new ProductWithDetailsDto
                {
                    Id = p.Id_Product,
                    Name = p.Name,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    Department = p.Department != null ? new DepartmentWithBranchDTO
                    {
                        Id_Department = p.Department.Id_Department,
                        Name = p.Department.Name,
                        Description = p.Department.Description,
                        Branch_Id=p.Department.Branch_Id,
                        BranchName = p.Department.Branch.Name,


                    } : null,
                    Units = p.ProductUnits?.Select(pu => new ProductUnitDTO
                    {
                        Id = pu.Id,
                        ProductId = pu.ProductId,
                        UnitId = pu.UnitId,
                        SpecialPrice = pu.SpecialPrice
                    }).ToList()
                }).ToList();

                return Response<List<ProductWithDetailsDto>>.Success(
                    result,
                    "Products with complete details retrieved successfully");
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                //_logger.LogError(ex, "Error retrieving products with details");

                return Response<List<ProductWithDetailsDto>>.Failure(
                    $"Failed to retrieve product details: {ex.Message}",
                    "500");
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

        public async Task<bool> AnyProductsInDepartmentAsync(int departmentId)
        {
            return await _unitOfWork._Product.AnyAsync(p => p.Department_Id == departmentId);
        }
    }
}
