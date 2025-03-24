using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Company
    {
        [Key]
        [Display(Name = "رقم الشركه")]
        public int Id_Company { get; set; }
        [Required]
        [Display(Name = "اسم الشركه")]
        public string Name { get; set; }

        [Display(Name = "ملاحظة")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "من تاريخ")]
        public DateTime? FromDate { get; set; }
        [Required]
        [Display(Name = "الى تاريخ")]
        public DateTime? ToDate { get; set; }

        [Required]
        [Display(Name = "نسبة الخصم")]
        [Range(1, 50, ErrorMessage = "Discount rate must be between 1 To 50.")]
        public int DiscountRate { get; set; }
    }

}
