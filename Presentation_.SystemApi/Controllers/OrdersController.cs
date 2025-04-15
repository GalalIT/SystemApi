using Application.System.DTO;
using Application.System.UseCace.OrderUseCase.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderUseCase _orderUseCase;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderUseCase orderUseCase, ILogger<OrdersController> logger)
        {
            _orderUseCase = orderUseCase;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Response<List<OrderDTO>>>> GetAllOrders()
        {
            try
            {
                var result = await _orderUseCase.GetOrdersAsync();
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, Response<List<OrderDTO>>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<OrderDTO>>> GetOrderById(int id)
        {
            try
            {
                var result = await _orderUseCase.GetOrderByIdAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {id}");
                return StatusCode(500, Response<OrderDTO>.Failure("Internal server error", "500"));
            }
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<Response<OrderDetailResponse>>> GetOrderWithDetails(int id)
        {
            try
            {
                var result = await _orderUseCase.GetOrderWithDetailsAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order details for {id}");
                return StatusCode(500, Response<OrderDetailResponse>.Failure("Internal server error", "500"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<Response<int>>> CreateOrder([FromBody] CreateCartDTO cartDto)
        {
            try
            {
                if (cartDto.ProductUnitIds == null || cartDto.Quantities == null || cartDto.Prices == null)
                {
                    return BadRequest(Response<int>.Failure("Product details are missing", "400"));
                }

                if (cartDto.ProductUnitIds.Count != cartDto.Quantities.Count ||
                    cartDto.ProductUnitIds.Count != cartDto.Prices.Count)
                {
                    return BadRequest(Response<int>.Failure("Mismatched product details", "400"));
                }

                var result = await _orderUseCase.ProcessOrderCreateAsync(cartDto);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, Response<int>.Failure("Internal server error", "500"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response<int>>> UpdateOrder(int id, [FromBody] CreateCartDTO cartDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(Response<int>.Failure("Invalid order ID", "400"));
                }

                if (cartDto.ProductUnitIds == null || cartDto.Quantities == null || cartDto.Prices == null)
                {
                    return BadRequest(Response<int>.Failure("Product details are missing", "400"));
                }

                if (cartDto.ProductUnitIds.Count != cartDto.Quantities.Count ||
                    cartDto.ProductUnitIds.Count != cartDto.Prices.Count)
                {
                    return BadRequest(Response<int>.Failure("Mismatched product details", "400"));
                }

                var result = await _orderUseCase.ProcessOrderUpdateAsync(id, cartDto);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                return StatusCode(500, Response<int>.Failure("Internal server error", "500"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(Response<int>.Failure("Invalid order ID", "400"));
                }

                var result = await _orderUseCase.DeleteOrderAsync(id);
                return StatusCode(int.Parse(result.Status), result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return StatusCode(500, Response<int>.Failure("Internal server error", "500"));
            }
        }
    }

}
