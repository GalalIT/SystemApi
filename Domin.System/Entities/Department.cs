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
        [Display(Name = "رقم القسم")]
        public int Id_Department { get; set; }
        [Required]
        [Display(Name = "اسم القسم")]
        public string Name { get; set; }
        [Display(Name = "وصف القسم")]
        public string? Description { get; set; }=string.Empty;
        [Required]
        [Display(Name = "اسم الفرع")]
        public int Branch_Id { get; set; }
        [ForeignKey(nameof(Branch_Id))]
        public virtual Branch? Branch { get; set; }
    }
}
