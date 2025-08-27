using Application.System.DTO;
using Application.System.UseCace.UserUseCases.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation_.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserDto registerDto)
        {
            var result = await _userUseCases.RegisterUserAsync(registerDto);

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userUseCases.GetAllUsersAsync();
            if (!result.Succeeded)
            {
                return StatusCode(int.Parse(result.Status), new
                {
                    message = result.Message,
                    code = result.Status
                });
            }
            return Ok(result.Data);
        }

        //[EnableRateLimiting("LoginLimit")]
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _userUseCases.LoginUserAsync(request);
            if (!result.Succeeded)
            {
                return StatusCode(int.Parse(result.Status), new
                {
                    message = result.Message,
                    code = result.Status
                });
            }
            return Ok(result.Data);
        }

        [HttpGet("GetUserDetails")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDetails()
        {
                var result = await _userUseCases.GetUserDetailAsync();
                if (!result.Succeeded)
                {
                    return StatusCode(int.Parse(result.Status), new
                    {
                        message = result.Message,
                        code = result.Status
                    });
                }
            return Ok(result.Data);
           
        }

    }

}
