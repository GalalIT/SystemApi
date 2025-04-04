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
        [Display(Name = "الرقم", Description = "Order Detail ID")]
        public int Id_OrderDetail { get; set; }

        [Display(Name = "ملاحظه الوجبه", Description = "Product Notes")]
        [StringLength(500, ErrorMessage = "يجب ألا تتجاوز الملاحظة 500 حرف | Notes must not exceed 500 characters")]
        public string? Description_product { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة | Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر | Quantity must be greater than zero")]
        [Display(Name = "الكميه", Description = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "السعر الإجمالي مطلوب | Total price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero")]
        [Display(Name = "اجمالي الصنف", Description = "Total Price")]
        public decimal Total_Price { get; set; }

        [Required(ErrorMessage = "معرف وحدة المنتج مطلوب | Product unit ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف وحدة المنتج غير صالح | Invalid product unit ID")]
        [Display(Name = "وحدة المنتج", Description = "Product Unit")]
        public int Product_Unit_id { get; set; }

        [Required(ErrorMessage = "معرف الطلب مطلوب | Order ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الطلب غير صالح | Invalid order ID")]
        [Display(Name = "رقم الطلب", Description = "Order ID")]
        public int Order_Id { get; set; }

        [ForeignKey("Product_Unit_id")]
        [Display(Name = "وحدة المنتج", Description = "Product Unit")]
        public virtual Product_Unit? product_Unit { get; set; }

        [ForeignKey("Order_Id")]
        [Display(Name = "الطلب", Description = "Order")]
        public virtual Order? Order { get; set; }
    }

}
