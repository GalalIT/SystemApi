using Application.System.DTO;
using Application.System.UseCace.UserUseCases.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserUseCases _userUseCases;
        private readonly ILogger<UsersController> _logger;
        public UsersController(
        IUserUseCases userUseCases,
        ILogger<UsersController> logger)
        {
            _userUseCases = userUseCases;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto registerDto)
        {
            var result = await _userUseCases.RegisterUserAsync(registerDto);

            if (!result.Succeeded)
            {
                return Problem(
                    title: result.Message,
                    statusCode: int.Parse(result.Status));
            }

            // Return 201 Created with just the location header
            return Created(
                uri: $"/api/user/{result.Data.Email}",
                value: result.Data);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userUseCases.GetAllUsersAsync();

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to get users: {Message}", result.Message);
                return StatusCode(int.Parse(result.Status), new ProblemDetails
                {
                    Title = result.Message,
                    Status = int.Parse(result.Status)
                });
            }

            return Ok(result.Data);
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            _logger.LogInformation("Login attempt from {RemoteIP}", HttpContext.Connection.RemoteIpAddress);

            var result = await _userUseCases.LoginUserAsync(request);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed for {Username}: {Reason}",
                    request.UsernameOrEmail,
                    result.Message);

                return Problem(
                    title: result.Message,
                    statusCode: int.Parse(result.Status),
                    instance: HttpContext.Request.Path);
            }

            // Set secure cookie if using cookie-based auth
            Response.Cookies.Append(
                "refreshToken",
                result.Data.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

            _logger.LogInformation("Successful login for user {UserId}", result.Data.UserId);
            return Ok(result.Data);
        }

        [HttpGet("GetUserDetails")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserDetails()
        {
            try
            {
               
                var result = await _userUseCases.GetUserDetailAsync();

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to fetch user details: {Message}", result.Message);
                    return Problem(
                        title: result.Message,
                        statusCode: int.Parse(result.Status),
                        instance: HttpContext.Request.Path);
                }

                _logger.LogInformation("Successfully retrieved user details");
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching user details");
                return Problem(
                    title: "An unexpected error occurred",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.Request.Path);
            }
        }

    }
}
