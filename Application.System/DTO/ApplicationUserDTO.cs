using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class ApplicationUserDTO
    {
        //public string Id { get; set; } // Inherited from IdentityUser
        //public string Name { get; set; }
        //public byte[]? ProfilePicture { get; set; }
        //public bool? IsActive { get; set; }
        //public int Branch_Id { get; set; }
    }
    public class CreateUserDto
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب | User name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "يجب أن يكون اسم المستخدم بين 2 و100 حرف | User name must be between 2 and 100 characters")]
        [Display(Name = "اسم المستخدم", Description = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب | Email is required")]
        [EmailAddress(ErrorMessage = "بريد إلكتروني غير صالح | Invalid email address")]
        [Display(Name = "البريد الإلكتروني", Description = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم المستخدم مطلوب | Username is required")]
        [Display(Name = "اسم الدخول", Description = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة | Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "يجب أن تكون كلمة المرور بين 6 و100 حرف | Password must be between 6 and 100 characters")]
        [Display(Name = "كلمة المرور", Description = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "معرف الفرع مطلوب | Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفرع غير صالح | Invalid branch ID")]
        [Display(Name = "الفرع", Description = "Branch")]
        public int Branch_Id { get; set; }


        public List<string>? Roles { get; set; }
    }
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب | User ID is required")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم المستخدم مطلوب | User name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "يجب أن يكون اسم المستخدم بين 2 و100 حرف | User name must be between 2 and 100 characters")]
        [Display(Name = "اسم المستخدم", Description = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب | Email is required")]
        [EmailAddress(ErrorMessage = "بريد إلكتروني غير صالح | Invalid email address")]
        [Display(Name = "البريد الإلكتروني", Description = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم المستخدم مطلوب | Username is required")]
        [Display(Name = "اسم الدخول", Description = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "معرف الفرع مطلوب | Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفرع غير صالح | Invalid branch ID")]
        [Display(Name = "الفرع", Description = "Branch")]
        public int Branch_Id { get; set; }
    }
    public class UserProfilePictureDto
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب | User ID is required")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "الصورة الشخصية مطلوبة | Profile picture is required")]
        [Display(Name = "الصورة الشخصية", Description = "Profile Picture")]
        public byte[] ProfilePicture { get; set; } = Array.Empty<byte>();
    }
    public class UserStatusDto
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب | User ID is required")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "حالة التفعيل مطلوبة | Activation status is required")]
        [Display(Name = "حالة التفعيل", Description = "Activation Status")]
        public bool IsActive { get; set; }
    }
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب | User ID is required")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة | Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الحالية", Description = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة | New password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "يجب أن تكون كلمة المرور بين 6 و100 حرف | Password must be between 6 and 100 characters")]
        [Display(Name = "كلمة المرور الجديدة", Description = "New Password")]
        public string NewPassword { get; set; } = string.Empty;
    }
    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public byte[]? ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public string? PhonNumber { get; set; }
        public bool TowFactorEnable { get; set; }
        public bool PhonNumberConfirm { get; set; }
        public bool AccessFailedCount { get; set; }
        public string[]? Roles { get; set; }
        public int Branch_Id { get; set; }
        public BranchUserDto? Branch { get; set; }
    }

    public class BranchUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class LoginDto
    {
        [Required(ErrorMessage = "اسم المستخدم أو البريد الإلكتروني مطلوب | Username or email is required")]
        [Display(Name = "اسم الدخول/البريد الإلكتروني", Description = "Username/Email")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة | Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور", Description = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "تذكرني", Description = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
