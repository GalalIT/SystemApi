using Application.System.DTO;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.SystemApi.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Presentation.SystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // Change here
        private readonly IConfiguration _configuration;
        private readonly IUnitOfRepository _unitOfWork;
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUnitOfRepository unitOfRepository) // Change here
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _unitOfWork = unitOfRepository;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(CreateUserDto registerDto)
        {



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                UserName = registerDto.Email,
                Branch_Id= registerDto.Branch_Id

            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await AssignRolesToUser(user, registerDto.Roles);

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "Account Created Successfully!"
            });
        }
        [AllowAnonymous]
        [HttpPost("login")]
public async Task<ActionResult<AuthResponseDTO>> LogIn(LoginDto logInDTO)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Try to find the user by email first
    var user = await _userManager.FindByEmailAsync(logInDTO.UsernameOrEmail);

    // If not found, try by username
    if (user == null)
    {
        user = await _userManager.FindByNameAsync(logInDTO.UsernameOrEmail);
    }

    if (user == null)
    {
        return Unauthorized(new AuthResponseDTO
        {
            IsSuccess = false,
            Message = "User not found with this email or username."
        });
    }

    var isPasswordValid = await _userManager.CheckPasswordAsync(user, logInDTO.Password);

    if (!isPasswordValid)
    {
        return Unauthorized(new AuthResponseDTO
        {
            IsSuccess = false,
            Message = "Invalid password."
        });
    }

    var roles = await _userManager.GetRolesAsync(user);
    var token = GenerateToken(user); // Ensure this method accepts roles

    return Ok(new AuthResponseDTO
    {
        Token = token,
        IsSuccess = true,
        Message = "Login successful"
    });
}

        [Authorize]
        [HttpGet("detail")]
        public async Task<ActionResult<UserResponseDto>> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user is null)
            {
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }
            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                IsActive = user.IsActive,
                Roles = [.. await _userManager.GetRolesAsync(user)],
                PhonNumber = user.PhoneNumber,
                TowFactorEnable = user.TwoFactorEnabled,
                PhonNumberConfirm = user.PhoneNumberConfirmed,
                AccessFailedCount = user.EmailConfirmed,
                Branch_Id=user.Branch_Id
            });
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            // Fetch all users
            var users = await _userManager.Users.ToListAsync();

            // Fetch roles for all users concurrently
            var userRolesTasks = users
                .Select(async u => new
                {
                    User = u,
                    Roles = await _userManager.GetRolesAsync(u) // Fetch roles asynchronously
                })
                .ToList();

            // Wait for all role-fetching tasks to complete
            var userRoles = await Task.WhenAll(userRolesTasks);

            // Map users and their roles to UserDetailDTO
            var userDetails = userRoles
                .Select(ur => new UserResponseDto
                {
                    Id = ur.User.Id,
                    ProfilePicture=ur.User.ProfilePicture,
                    Email = ur.User.Email,
                    Name = ur.User.Name,
                    Roles = ur.Roles.ToArray() // Convert roles to an array
                })
                .ToList();

            return Ok(userDetails);
        }




        private async Task AssignRolesToUser(ApplicationUser user, ICollection<string> roles)
        {
            if (roles == null || !roles.Any())
            {
                await _userManager.AddToRoleAsync(user, "User");
                return;
            }

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWTSetting:SecretKey"]);
            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Name, user.Name ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWTSetting:ValidAudience"]),
        new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWTSetting:ValidIssuer"])
    };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }




    }
}
