using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Company : IValidatableObject
    {
        [Key]
        [Display(Name = "رقم الشركة", Description = "Company ID")]
        public int Id_Company { get; set; }

        [Required(ErrorMessage = "اسم الشركة مطلوب | Company name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "يجب أن يكون اسم الشركة بين 2 و100 حرف | Company name must be between 2 and 100 characters")]
        [Display(Name = "اسم الشركة", Description = "Company Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "يجب ألا تتجاوز الملاحظة 500 حرف | Description must not exceed 500 characters")]
        [Display(Name = "ملاحظات", Description = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "من التاريخ مطلوب | From date is required")]
        [Display(Name = "من تاريخ", Description = "From Date")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "إلى التاريخ مطلوب | To date is required")]
        [Display(Name = "إلى تاريخ", Description = "To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "نسبة الخصم مطلوبة | Discount rate is required")]
        [Range(1, 50, ErrorMessage = "يجب أن تكون نسبة الخصم بين 1 و50 بالمئة | Discount rate must be between 1 and 50 percent")]
        [Display(Name = "نسبة الخصم", Description = "Discount Rate")]
        public int DiscountRate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromDate == default)
            {
                yield return new ValidationResult(
                    "من التاريخ مطلوب | From date is required",
                    new[] { nameof(FromDate) });
            }

            if (ToDate == default)
            {
                yield return new ValidationResult(
                    "إلى التاريخ مطلوب | To date is required",
                    new[] { nameof(ToDate) });
            }

            // Updated condition to include equal dates
            if (ToDate <= FromDate)  // Changed from < to <=
            {
                yield return new ValidationResult(
                    "يجب أن يكون تاريخ الانتهاء بعد تاريخ البداية | To date must be after from date",
                    new[] { nameof(ToDate) });
            }
        }
    }

}

