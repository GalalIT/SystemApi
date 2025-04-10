using Application.System.DTO;
using Application.System.Interface.IUserOperation;
using Application.System.UseCace.UserUseCase.Interface;
using Application.System.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.UserUseCase.Implement
{
    public class UserManagementUseCases : IUserManagementUseCases
    {
        private readonly IAllUserOperation _userService;
        private readonly ILogger<UserManagementUseCases> _logger;

        public UserManagementUseCases(IAllUserOperation userService, ILogger<UserManagementUseCases> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<Response<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            try
            {
                return await _userService.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return Response<IEnumerable<UserResponseDto>>.Failure("An error occurred while retrieving users");
            }
        }

        public async Task<Response<UserResponseDto>> GetUserById(string id)
        {
            try
            {
                return await _userService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by ID: {UserId}", id);
                return Response<UserResponseDto>.Failure("An error occurred while retrieving the user");
            }
        }

        public async Task<Response<UserResponseDto>> GetUserByUsername(string username)
        {
            try
            {
                return await _userService.GetByUserNameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by username: {Username}", username);
                return Response<UserResponseDto>.Failure("An error occurred while retrieving the user");
            }
        }

        public async Task<Response<IEnumerable<UserResponseDto>>> GetUsersByBranch(int branchId)
        {
            try
            {
                return await _userService.GetUsersByBranchAsync(branchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users by branch ID: {BranchId}", branchId);
                return Response<IEnumerable<UserResponseDto>>.Failure("An error occurred while retrieving users by branch");
            }
        }

        public async Task<Response<UserResponseDto>> CreateUser(CreateUserDto dto)
        {
            try
            {
                return await _userService.CreateAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new user: {Username}", dto.UserName);
                return Response<UserResponseDto>.Failure("An error occurred while creating the user");
            }
        }

        public async Task<Response> UpdateUser(UpdateUserDto dto)
        {
            try
            {
                return await _userService.UpdateAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user: {UserId}", dto.Id);
                return Response.Failure("An error occurred while updating the user");
            }
        }

        public async Task<Response> DeleteUser(string id)
        {
            try
            {
                return await _userService.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user: {UserId}", id);
                return Response.Failure("An error occurred while deleting the user");
            }
        }

        public async Task<Response> ChangeUserPassword(ChangePasswordDto dto)
        {
            try
            {
                return await _userService.ChangePasswordAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user: {UserId}", dto.UserId);
                return Response.Failure("An error occurred while changing the password");
            }
        }

        public async Task<Response> UpdateUserProfilePicture(UserProfilePictureDto dto)
        {
            try
            {
                return await _userService.UpdateProfilePictureAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile picture for user: {UserId}", dto.UserId);
                return Response.Failure("An error occurred while updating the profile picture");
            }
        }

        public async Task<Response> ToggleUserStatus(UserStatusDto dto)
        {
            try
            {
                return await _userService.ToggleUserStatusAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling status for user: {UserId}", dto.UserId);
                return Response.Failure("An error occurred while updating user status");
            }
        }

        public async Task<Response<UserResponseDto>> UserLogin(LoginDto dto)
        {
            try
            {
                return await _userService.LoginAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for: {UsernameOrEmail}", dto.UsernameOrEmail);
                return Response<UserResponseDto>.Failure("An error occurred during login");
            }
        }
    }
}
