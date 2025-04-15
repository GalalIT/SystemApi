using Application.System.DTO;
using Application.System.Utility;
using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUserOperation
{
    public interface IAllUserOperation: ICurrentUserContextOperation
    {
        Task<Response<CreateUserDto>> RegisterAsync(CreateUserDto registerDto);
        Task<Response<AuthResponseDTO>> LoginAsync(LoginDto loginDto);
        Task<Response<UserResponseDto>> GetUserDetailAsync(string userId);
        Task<Response<List<UserResponseDto>>> GetAllUsersAsync();
        

    }
}
