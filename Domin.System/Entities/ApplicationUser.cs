using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "اسم المستخدم مطلوب | User name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "يجب أن يكون اسم المستخدم بين 2 و100 حرف | User name must be between 2 and 100 characters")]
        [Display(Name = "اسم المستخدم", Description = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "الصورة الشخصية", Description = "Profile Picture")]
        [DataType(DataType.Upload)]
        public byte[]? ProfilePicture { get; set; }

        [Required(ErrorMessage = "حالة التفعيل مطلوبة | Activation status is required")]
        [Display(Name = "حالة التفعيل", Description = "Activation Status")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "معرف الفرع مطلوب | Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفرع غير صالح | Invalid branch ID")]
        [Display(Name = "الفرع", Description = "Branch")]
        public int Branch_Id { get; set; }

        [ForeignKey(nameof(Branch_Id))]
        [Display(Name = "تفاصيل الفرع", Description = "Branch Details")]
        public virtual Branch? Branch { get; set; }
    }

}
