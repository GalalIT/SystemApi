using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Unit
    {
        [Key]
        [Display(Name = "رقم الوحدة", Description = "Unit ID")]
        public int Id_Unit { get; set; }

        [Required(ErrorMessage = "اسم الوحدة مطلوب | Unit name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "يجب أن يكون اسم الوحدة بين 2 و100 حرف | Unit name must be between 2 and 100 characters")]
        [Display(Name = "اسم الوحدة", Description = "Unit Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "معرف الفرع مطلوب | Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفرع غير صالح | Invalid branch ID")]
        [Display(Name = "الفرع", Description = "Branch")]
        public int Branch_Id { get; set; }

        [ForeignKey(nameof(Branch_Id))]
        [Display(Name = "تفاصيل الفرع", Description = "Branch Details")]
        public virtual Branch? Branch { get; set; }

        [Display(Name = "وحدات المنتج", Description = "Product Units")]
        public virtual ICollection<Product_Unit> ProductUnits { get; set; } = new HashSet<Product_Unit>();
    }
}
