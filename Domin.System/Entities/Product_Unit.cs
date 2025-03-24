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
        public int Id { get; set; }
        /// ////////////////////////////////////
        [Display(Name = "اسم المنتج")]
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        /// ////////////////////////////////////
        [Required]
        [Display(Name = "اسم الوحده")]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public virtual Unit? Unit { get; set; }
        /// ////////////////////////////////////
        [Required]
        [Display(Name = "Price")]
        public decimal SpecialPrice { get; set; }
    }

}
