using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUserOperation
{
    public interface IAllUserOperation
    {
        Task<Response<IEnumerable<UserResponseDto>>> GetAllAsync();
        Task<Response<UserResponseDto>> GetByIdAsync(string id);
        Task<Response<UserResponseDto>> GetByUserNameAsync(string username);
        Task<Response<IEnumerable<UserResponseDto>>> GetUsersByBranchAsync(int branchId);
        Task<Response<UserResponseDto>> CreateAsync(CreateUserDto dto);
        Task<Response> UpdateAsync(UpdateUserDto dto);
        Task<Response> DeleteAsync(string id);
        Task<Response> ChangePasswordAsync(ChangePasswordDto dto);
        Task<Response> UpdateProfilePictureAsync(UserProfilePictureDto dto);
        Task<Response> ToggleUserStatusAsync(UserStatusDto dto);
        Task<Response<UserResponseDto>> LoginAsync(LoginDto dto);

    }
}
