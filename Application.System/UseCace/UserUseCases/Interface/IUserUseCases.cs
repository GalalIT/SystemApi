using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.UserUseCases.Interface
{
    public interface IUserUseCases
    {
        Task<Response<CreateUserDto>> RegisterUserAsync(CreateUserDto registerDto);
        Task<Response<AuthResponseDTO>> LoginUserAsync(LoginDto loginDto);
        Task<Response<List<UserResponseDto>>> GetAllUsersAsync();
        Task<Response<UserResponseDto>> GetUserDetailAsync(string? userId = null);
    }
}
