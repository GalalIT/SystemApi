using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Product_Unit
    {
        [Key]
        [Display(Name = "رقم الوحدة المنتج", Description = "Product Unit ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "معرف المنتج مطلوب | Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف المنتج غير صالح | Invalid product ID")]
        [Display(Name = "المنتج", Description = "Product")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [Display(Name = "تفاصيل المنتج", Description = "Product Details")]
        public virtual Product? Product { get; set; }

        [Required(ErrorMessage = "معرف الوحدة مطلوب | Unit ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الوحدة غير صالح | Invalid unit ID")]
        [Display(Name = "الوحدة", Description = "Unit")]
        public int UnitId { get; set; }

        [ForeignKey("UnitId")]
        [Display(Name = "تفاصيل الوحدة", Description = "Unit Details")]
        public virtual Unit? Unit { get; set; }

        [Required(ErrorMessage = "السعر الخاص مطلوب | Special price is required")]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "يجب أن يكون السعر الخاص أكبر من صفر | Special price must be greater than zero")]
        [Display(Name = "السعر الخاص", Description = "Special Price")]
        [DataType(DataType.Currency)]
        public decimal SpecialPrice { get; set; }

    }

}
