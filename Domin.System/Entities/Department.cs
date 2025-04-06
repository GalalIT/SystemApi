using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Department
    {
        [Key]
        [Display(Name = "رقم القسم", Description = "Department ID")]
        public int Id_Department { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب | Department name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "يجب أن يكون اسم القسم بين 2 و100 حرف | Department name must be between 2 and 100 characters")]
        [Display(Name = "اسم القسم", Description = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "يجب ألا يتجاوز الوصف 500 حرف | Description must not exceed 500 characters")]
        [Display(Name = "وصف القسم", Description = "Department Description")]
        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "معرف الفرع مطلوب | Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفرع غير صالح | Invalid branch ID")]
        [Display(Name = "الفرع", Description = "Branch")]
        public int Branch_Id { get; set; }

        [ForeignKey(nameof(Branch_Id))]
        [Display(Name = "تفاصيل الفرع", Description = "Branch Details")]
        public virtual Branch? Branch { get; set; }
        [Display(Name = "منتجات القسم", Description = "Department Products")]
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
