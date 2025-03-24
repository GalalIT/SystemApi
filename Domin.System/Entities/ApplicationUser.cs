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
        [Required]
        [Display(Name = "اسم المستخدم")]
        public string Name { get; set; }

        [Display(Name = " الصورة الشخصية")]
        public byte[]? ProfilePicture { get; set; }

        [Display(Name = "تفعيل")]
        public bool? IsActive { get; set; }

        [Required]
        [Display(Name = "اسم الفرع")]
        public int Branch_Id { get; set; }
        [ForeignKey(nameof(Branch_Id))]
        public virtual Branch? Branch { get; set; }
    }

}
