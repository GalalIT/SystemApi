using Application.System.DTO;
using Application.System.UseCace.ProductUseCase.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductUseCase _productUseCase;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductUseCase productUseCase, ILogger<ProductsController> logger)
        {
            _productUseCase = productUseCase;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Response<ProductDTO>>> CreateProduct([FromBody] CreateProductWithUnitsDTO productDto)
        {
            try
            {
                var result = await _productUseCase.CreateProductWithUnitsAsync(productDto);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, Response<ProductDTO>.Failure("Internal server error", "500"));
            }
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult<Response<ProductDTO>>> UpdateProduct(int productId, [FromBody] CreateProductWithUnitsDTO productDto)
        {
            try
            {
                if (productId <= 0)
                    return BadRequest(Response<ProductDTO>.Failure("Invalid product ID", "400"));

                var result = await _productUseCase.UpdateProductWithUnitsAsync(productId, productDto);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {productId}");
                return StatusCode(500, Response<ProductDTO>.Failure("Internal server error", "500"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            try
            {
                var result = await _productUseCase.DeleteProductsAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {id}");
                return StatusCode(500, Response<int>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<ProductDTO>>> GetProductById(int id)
        {
            try
            {
                var result = await _productUseCase.GetProductByIdAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product {id}");
                return StatusCode(500, Response<ProductDTO>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("branch/{userBranchId}")]
        public async Task<ActionResult<Response<ProductBranchResponse>>> GetProductsByBranch(int userBranchId, [FromQuery] int? departmentId)
        {
            try
            {
                var result = await _productUseCase.GetProductsByBranchWithDepartments(userBranchId, departmentId);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for branch {userBranchId}");
                return StatusCode(500, Response<ProductBranchResponse>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("details")]
        public async Task<ActionResult<Response<List<ProductWithDetailsDto>>>> GetAllProductsWithDetails()
        {
            try
            {
                var result = await _productUseCase.GetAllProductsWithDetailsAsync();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product details");
                return StatusCode(500, Response<List<ProductWithDetailsDto>>.Failure("Internal server error", "500"));
            }
        }
    }

}
