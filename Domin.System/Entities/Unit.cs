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
        [Display(Name = "رقم الوحده")]
        public int Id_Unit { get; set; }
        [Required]
        [Display(Name = "اسم الوحده")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "اسم الفرع")]
        public int Branch_Id { get; set; }
        [ForeignKey(nameof(Branch_Id))]
        public virtual Branch? Branch { get; set; }
        public virtual ICollection<Product_Unit>? ProductUnits { get; set; }
    }
    //public List<Product_Unit> Units_Product { get; set; }
    //public ICollection<Product> products { get; set; }
}
