using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Branch
    {
        [Key]
        [Display(Name = "رقم الفرع", Description = "Branch ID")]
        public int Id_Branch { get; set; }

        [Required(ErrorMessage = "اسم الفرع مطلوب | Branch name is required")]
        [StringLength(100, MinimumLength = 5,
            ErrorMessage = "يجب أن يكون اسم الفرع بين 5 و100 حرف | Branch name must be between 5 and 100 characters")]
        [Display(Name = "اسم الفرع", Description = "Branch Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "عنوان الفرع مطلوب | Branch address is required")]
        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز العنوان 200 حرف | Address must not exceed 200 characters")]
        [Display(Name = "عنوان الفرع", Description = "Branch Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "مدينة الفرع مطلوبة | Branch city is required")]
        [StringLength(50, ErrorMessage = "يجب ألا تتجاوز المدينة 50 حرف | City must not exceed 50 characters")]
        [Display(Name = "مدينة الفرع", Description = "Branch City")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "هاتف الفرع مطلوب | Branch phone is required")]
        [RegularExpression(@"^[0-9]{10,15}$",
            ErrorMessage = "يجب أن يتكون الهاتف من أرقام فقط وبين 10-15 رقم | Phone must contain 10-15 digits only")]
        [Display(Name = "هاتف الفرع", Description = "Branch Phone")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "حالة الفرع مطلوبة | Branch status is required")]
        [Display(Name = "حالة الفرع", Description = "Branch Status")]
        public bool IsActive { get; set; } = true;
    }

}
