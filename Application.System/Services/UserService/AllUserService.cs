using Application.System.DTO;
using Application.System.Interface.IUserOperation;
using Application.System.Utility;
using Domin.System.Entities;
using Domin.System.IRepository.IUserRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Services.UserService
{
    public class AllUserService : IAllUserOperation
    {
        private readonly IAllUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AllUserService(IAllUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Response<IEnumerable<UserResponseDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(MapToDto);
            return Response<IEnumerable<UserResponseDto>>.Success(dtos);
        }

        public async Task<Response<UserResponseDto>> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetUserWithBranchAsync(id);
            if (user == null)
                return Response<UserResponseDto>.Failure("User not found");

            return Response<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<Response<UserResponseDto>> GetByUserNameAsync(string username)
        {
            var user = await _userRepository.GetByUserNameAsync(username);
            if (user == null)
                return Response<UserResponseDto>.Failure("User not found");

            return Response<UserResponseDto>.Success(MapToDto(user));
        }

        public async Task<Response<IEnumerable<UserResponseDto>>> GetUsersByBranchAsync(int branchId)
        {
            var users = await _userRepository.GetUsersByBranchAsync(branchId);
            var dtos = users.Select(MapToDto);
            return Response<IEnumerable<UserResponseDto>>.Success(dtos);
        }

        public async Task<Response<UserResponseDto>> CreateAsync(CreateUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Name = dto.Name,
                Branch_Id = dto.Branch_Id,
                IsActive = true
            };

            var result = await _userRepository.AddAsync(user, dto.Password);

            if (!result.Succeeded)
                return Response<UserResponseDto>.Failure("Failed to create user");

            return Response<UserResponseDto>.Success(MapToDto(user), "User created successfully");
        }

        public async Task<Response> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
                return Response.Failure("User not found");

            user.Name = dto.Name;
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.Branch_Id = dto.Branch_Id;

            var result = await _userRepository.UpdateAsync(user);
            return result.Succeeded
                ? Response.Success("User updated successfully")
                : Response.Failure("Failed to update user");
        }

        public async Task<Response> DeleteAsync(string id)
        {
            var result = await _userRepository.DeleteAsync(id);
            return result.Succeeded
                ? Response.Success("User deleted successfully")
                : Response.Failure("Failed to delete user");
        }

        public async Task<Response> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return Response.Failure("User not found");

            var result = await _userRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded
                ? Response.Success("Password changed successfully")
                : Response.Failure("Failed to change password");
        }

        public async Task<Response> UpdateProfilePictureAsync(UserProfilePictureDto dto)
        {
            var result = await _userRepository.UpdateProfilePictureAsync(dto.UserId, dto.ProfilePicture);
            return result.Succeeded
                ? Response.Success("Profile picture updated")
                : Response.Failure("Failed to update profile picture");
        }

        public async Task<Response> ToggleUserStatusAsync(UserStatusDto dto)
        {
            var result = await _userRepository.ToggleUserStatusAsync(dto.UserId, dto.IsActive);
            return result.Succeeded
                ? Response.Success("User status updated")
                : Response.Failure("Failed to update user status");
        }
        public async Task<Response<UserResponseDto>> LoginAsync(LoginDto dto)
        {
            ApplicationUser? user = null;

            // Try to find by username or email
            if (dto.UsernameOrEmail.Contains("@"))
                user = await _userRepository.GetByUserNameAsync(dto.UsernameOrEmail); // Assuming you search by email here
            else
                user = await _userRepository.GetByUserNameAsync(dto.UsernameOrEmail);

            if (user == null)
                return await Response<UserResponseDto>.FailureAsync("المستخدم غير موجود | User not found", "404");

            if (!user.IsActive)
                return await Response<UserResponseDto>.FailureAsync("تم تعطيل هذا الحساب | This account is disabled", "403");

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!passwordValid.Succeeded)
                return await Response<UserResponseDto>.FailureAsync("كلمة المرور غير صحيحة | Incorrect password", "401");

            // Map user to response DTO
            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                IsActive = user.IsActive,
                Branch_Id = user.Branch_Id,
                Branch = user.Branch != null ? new BranchUserDto
                {
                    Id = user.Branch.Id_Branch,
                    Name = user.Branch.Name
                } : null
            };

            return await Response<UserResponseDto>.SuccessAsync(response, "تم تسجيل الدخول بنجاح | Login successful");
        }

        private UserResponseDto MapToDto(ApplicationUser user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Branch_Id = user.Branch_Id,
                IsActive = user.IsActive ,
                ProfilePicture = user.ProfilePicture,
                Branch = user.Branch != null ? new BranchUserDto
                {
                    Id = user.Branch.Id_Branch,
                    Name = user.Branch.Name
                } : null
            };
        }


    }
}
