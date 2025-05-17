using Application.System.DTO;
using Application.System.Interface.IUserOperation;
using Application.System.UseCace.UserUseCases.Interface;
using Application.System.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.UserUseCases.Implement
{
    public class UserUseCases : IUserUseCases
    {
        private readonly IAllUserOperation _userService;
        private readonly ILogger<UserUseCases> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authService;

        public UserUseCases( IHttpContextAccessor httpContextAccessor, IAllUserOperation userService, ILogger<UserUseCases> logger, IAuthorizationService authService)
        {
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;

        }

        public async Task<Response<CreateUserDto>> RegisterUserAsync(CreateUserDto registerDto)
        {
            using (_logger.BeginScope("Registration for {Email}", registerDto.Email))
                try
                {
                    // Additional business logic can be added here
                    if (registerDto == null)
                    {
                        return Response<CreateUserDto>.Failure("Registration data is required", "400");
                    }

                    // Delegate to service layer
                    return await _userService.RegisterAsync(registerDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Registration failed for {Email}", registerDto.Email);
                    return Response<CreateUserDto>.Failure("Registration failed. Please try again later.", "500");
                }
        }

        public async Task<Response<List<UserResponseDto>>> GetAllUsersAsync()
        {
            try
            {
                // 1. Authorization check
                //var authResult = await _authService.AuthorizeAsync(
                //    _httpContextAccessor.HttpContext.User,
                //    "ViewAllUsersPolicy");

                //if (!authResult.Succeeded)
                //{
                //    _logger.LogWarning("Unauthorized access attempt to GetAllUsers");
                //    return Response<List<UserResponseDto>>.Failure("Unauthorized", "403");
                //}
                // 2. Business logic (if any)
                // e.g., filter based on tenant/organization

                // 3. Get data from service layer
                var result = await _userService.GetAllUsersAsync();

                // 4. Transform data if needed
                _logger.LogInformation("Retrieved {Count} users", result.Data?.Count ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return Response<List<UserResponseDto>>.Failure("An error occurred while retrieving users", "500");
            }
        }

        public async Task<Response<AuthResponseDTO>> LoginUserAsync(LoginDto request)
        {
            using (_logger.BeginScope("Login attempt for {Identifier}", request.UsernameOrEmail))
            {
                // Input validation
                //if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
                //    return Response<AuthResponseDTO>.Failure("Username/email required111", "400");

                //if (string.IsNullOrWhiteSpace(request.Password))
                //    return Response<AuthResponseDTO>.Failure("Password required", "400");

                try
                {
                    // Delegate to service layer
                    var loginResult = await _userService.LoginAsync(request);

                    if (!loginResult.Succeeded)
                    {
                        _logger.LogWarning("Login failed for {Identifier}", request.UsernameOrEmail);
                        return loginResult; // Preserve original error response
                    }

                    // Additional business logic can be added here
                    

                    return loginResult;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login processing error");
                    return Response<AuthResponseDTO>.Failure("Login service unavailable", "503");
                }
            }
        }

        public async Task<Response<UserResponseDto>> GetUserDetailAsync(string? userId = null)
        {
            using (_logger.BeginScope("Fetching user details for {UserId}", userId ?? "current-user"))
            {
                try
                {
                    // 1. Determine target user ID
                    var targetUserId = userId ?? _userService.UserId;

                    if (string.IsNullOrEmpty(targetUserId))
                    {
                        _logger.LogWarning("No user ID available");
                        return Response<UserResponseDto>.Failure("User identification failed", "400");

                    }

                    // 2. Delegate to service layer
                    var result = await _userService.GetUserDetailAsync(targetUserId);

                    if (!result.Succeeded)
                    {
                        _logger.LogWarning("Failed to fetch user details: {Message}", result.Message);
                        return result; // Preserve original error response
                    }

                    _logger.LogInformation("Successfully retrieved details for user {UserId}", targetUserId);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching user details");
                    return Response<UserResponseDto>.Failure("Error retrieving user data", "500");
                }
            }
        }


    }
}