using Application.System.DTO;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUnitOfRepository;
using Infrastructure.System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
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
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUnitOfRepository unitOfRepository) // Change here
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _unitOfWork = unitOfRepository;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register(CreateUserDto registerDto)
        {



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "هذا البريد موجود بالفعل."
                });
            }
            var existingUserByUsername = await _userManager.FindByNameAsync(new MailAddress(registerDto.Email).User);
            if (existingUserByUsername != null)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "اسم المستخدم موجود بالفعل."
                });
            }
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                UserName=new MailAddress(registerDto.Email).User,
                Branch_Id = registerDto.Branch_Id

            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            
            // Optionally, send a confirmation email
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
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
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(logInDTO.UsernameOrEmail) || string.IsNullOrWhiteSpace(logInDTO.Password))
            {
                return BadRequest(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Username/email and password are required."
                });
            }

            // Try to find the user by email or username
            var user = await _userManager.FindByEmailAsync(logInDTO.UsernameOrEmail)
                     ?? await _userManager.FindByNameAsync(logInDTO.UsernameOrEmail);

            if (user == null)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid login credentials."
                });
            }

            // Validate password
            if (!await _userManager.CheckPasswordAsync(user, logInDTO.Password))
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid login credentials."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user); // Pass roles in if needed

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
            var users = await _userManager.Users
                                    .Include(u => u.Branch)  // Load branch data
                                    .ToListAsync();
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
                AccessFailedCount = user.AccessFailedCount,
                Branch_Id=user.Branch_Id,
                Branch = user.Branch != null ? new BranchUserDto
                {
                    Id = user.Branch.Id_Branch,
                    Name = user.Branch.Name
                } : null
            });
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Branch)  // Load branch data
                .ToListAsync();

            var userDetails = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDetails.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfilePicture = user.ProfilePicture,
                    IsActive = user.IsActive,
                    Branch_Id = user.Branch_Id,  // Now will show correct value
                    Branch = user.Branch != null ? new BranchUserDto
                    {
                        Id = user.Branch.Id_Branch,
                        Name = user.Branch.Name
                    } : null,
                    Roles = roles.ToArray()
                });
            }

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
