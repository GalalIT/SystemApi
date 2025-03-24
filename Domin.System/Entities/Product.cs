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
        [Display(Name = "رقم المنتج")]
        public int Id_Product { get; set; }
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; }
        
        [Display(Name = "اسم القسم")]
        public int Department_Id { get; set; }
        [ForeignKey(nameof(Department_Id))]
        public virtual Department? Department { get; set; }
        [Required]
        [Display(Name = " ألسعر")]
        public decimal Price {  get; set; }

        [Required]
        [Display(Name = "حالة المنتج")]
        public bool? IsActive { get; set; }
        public virtual ICollection<Product_Unit>? ProductUnits { get; set; }
    }
    
}
