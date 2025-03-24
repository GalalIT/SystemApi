using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class OrderDetails
    {
        [Key]
        [Display(Name ="الرقم")]
        public int Id_OrderDetail { get; set; }

        [Display(Name = "ملاحظه الوجبه")]
        public string? Description_product { get; set; }
        [Required]
        [Display(Name = "الكميه")]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "اجمالي الصنف")]
        public decimal Total_Price { get; set; }

        [Required]
        public int Product_Unit_id { get; set; }
        

        [Required]
        [Display(Name = " رقم الطلب")]
        public int? Order_Id { get; set; }

        [ForeignKey("Product_Unit_id")]
        public virtual Product_Unit product_Unit { get; set; }
        [ForeignKey("Order_Id")]
        public virtual Order? Order { get; set; }
    }


}
