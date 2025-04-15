using Application.System.DTO;
using Application.System.UseCace.UserUseCases.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_.SystemApi.Controllers
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

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userUseCases.GetAllUsersAsync();

            return Ok(result.Data);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _userUseCases.LoginUserAsync(request);

            return Ok(result.Data);
        }

        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails()
        {
                var result = await _userUseCases.GetUserDetailAsync();

                return Ok(result.Data);
           
        }

    }

}
