using Application.System.DTO;
using Application.System.Interface.ICartOperation;
using Application.System.UseCace.CartUseCase.Interface;
using Application.System.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.CartUseCase.Implement
{
    public class CartUseCase : ICartUseCase
    {
        private readonly ICartOperation _cartOperation;
        private readonly ILogger<CartUseCase> _logger;

        public CartUseCase(
            ICartOperation cartOperation,
            ILogger<CartUseCase> logger)
        {
            _cartOperation = cartOperation;
            _logger = logger;
        }

        public async Task<Response<int>> ProcessCartAsync(CreateCartDTO cartDto)
        {
            // Input validation
            if (cartDto == null)
            {
                _logger.LogWarning("Null cart DTO received");
                return Response<int>.Failure("Cart data is required", "400");
            }

            if (string.IsNullOrEmpty(cartDto.UserId))
            {
                _logger.LogWarning("Empty user ID in cart processing");
                return Response<int>.Failure("User ID is required", "400");
            }

            try
            {
                // Delegate processing to the operation layer
                var result = await _cartOperation.ProcessCartAsync(cartDto);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Successfully processed order #{OrderId} for user {UserId}",
                        result.Data, cartDto.UserId);
                }
                else
                {
                    _logger.LogError("Failed to process cart: {Message}", result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing cart for user {UserId}", cartDto.UserId);
                return Response<int>.Failure($"An error occurred: {ex.Message}", "500");
            }
        }
    }
}
