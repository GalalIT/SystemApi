using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.UserUseCase.Interface
{
    public interface IUserManagementUseCases
    {
        Task<Response<IEnumerable<UserResponseDto>>> GetAllUsers();
        Task<Response<UserResponseDto>> GetUserById(string id);
        Task<Response<UserResponseDto>> GetUserByUsername(string username);
        Task<Response<IEnumerable<UserResponseDto>>> GetUsersByBranch(int branchId);
        Task<Response<UserResponseDto>> CreateUser(CreateUserDto dto);
        Task<Response> UpdateUser(UpdateUserDto dto);
        Task<Response> DeleteUser(string id);
        Task<Response> ChangeUserPassword(ChangePasswordDto dto);
        Task<Response> UpdateUserProfilePicture(UserProfilePictureDto dto);
        Task<Response> ToggleUserStatus(UserStatusDto dto);
        Task<Response<UserResponseDto>> UserLogin(LoginDto dto);
    }
}
