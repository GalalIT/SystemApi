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
        [Display(Name = "رقم الفرع")]
        public int Id_Branch { get; set; }
        [Required]
        [MinLength(5)]
        [Display(Name ="اسم الفرع"  )]
        public string Name { get; set; }
        [Required]
        [Display(Name = "عنوان الفرع")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "مدينة الفرع")]
        public string City { get; set; }
        [Required]
        [Display(Name = "هاتف الفرع")]
        public string Phone { get; set; }

        [Display(Name = "حالة الفرع")]
        public bool? IsActive { get; set; }
        
    }

}
