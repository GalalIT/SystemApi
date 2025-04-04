using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Product
    {
        [Key]
        [Display(Name = "رقم المنتج", Description = "Product ID")]
        public int Id_Product { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب | Product name is required")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "يجب أن يكون اسم المنتج بين 3 و100 حرف | Product name must be between 3 and 100 characters")]
        [Display(Name = "اسم المنتج", Description = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى اختيار القسم | Department is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف القسم غير صالح | Invalid department ID")]
        [Display(Name = "القسم", Description = "Department")]
        public int Department_Id { get; set; }

        [ForeignKey(nameof(Department_Id))]
        [Display(Name = "تفاصيل القسم", Description = "Department Details")]
        public virtual Department? Department { get; set; }

        [Required(ErrorMessage = "السعر مطلوب | Price is required")]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero")]
        [Display(Name = "السعر", Description = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "حالة المنتج مطلوبة | Product status is required")]
        [Display(Name = "الحالة", Description = "Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "الوحدات المتاحة", Description = "Available Units")]
        public virtual ICollection<Product_Unit> ProductUnits { get; set; } = new HashSet<Product_Unit>();
    }

}
