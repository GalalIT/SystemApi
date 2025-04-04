using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Entities
{
    public class Order
    {
        [Key]
        [Display(Name = "رقم الطلب", Description = "Order ID")]
        public int Id_Order { get; set; }

        [Required(ErrorMessage = "إجمالي سعر الطلب مطلوب | Total amount is required")]
        [Range(50, int.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من 50 | Amount must be greater than 50")]
        [Display(Name = "إجمالي السعر", Description = "Total Amount")]
        public decimal Total_Amount { get; set; }

        [Display(Name = "الإجمالي بعد الخصم", Description = "Total Amount After Discount")]
        public decimal Total_AmountAfterDiscount { get; set; }

        [Display(Name = "الخصم", Description = "Discount")]
        public decimal Discount { get; set; }

        [Display(Name = "تاريخ الإنشاء", Description = "Creation Date")]
        public DateTime DateTime_Created { get; set; } = DateTime.Now;

        [Display(Name = "رقم فاتورة الشركة", Description = "Company Invoice Number")]
        public string? OrderNumber { get; set; }

        [Range(1, 2, ErrorMessage = "يجب أن يكون نوع الطلب 1 (محلي) أو 2 (سفري) | Order type must be 1 (Local) or 2 (Travel)")]
        [Display(Name = "نوع الطلب", Description = "Order Type (1=Local, 2=Travel)")]
        public int OrderType { get; set; }

        [Required(ErrorMessage = "معرف الفرع مطلوب | Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الفرع غير صالح | Invalid branch ID")]
        [Display(Name = "الفرع", Description = "Branch")]
        public int Branch_Id { get; set; }

        [ForeignKey(nameof(Branch_Id))]
        [Display(Name = "تفاصيل الفرع", Description = "Branch Details")]
        public virtual Branch? Branch { get; set; }

        [Required(ErrorMessage = "معرف الشركة مطلوب | Company ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "معرف الشركة غير صالح | Invalid company ID")]
        [Display(Name = "الشركة", Description = "Company")]
        public int? Company_id { get; set; }

        [ForeignKey(nameof(Company_id))]
        [Display(Name = "تفاصيل الشركة", Description = "Company Details")]
        public virtual Company? Company { get; set; }

        [Required(ErrorMessage = "معرف المستخدم مطلوب | User ID is required")]
        [MinLength(1, ErrorMessage = "معرف المستخدم غير صالح | Invalid user ID")]
        [Display(Name = "المستخدم", Description = "User")]
        public string User_id { get; set; } = null!;

        [ForeignKey(nameof(User_id))]
        [Display(Name = "تفاصيل المستخدم", Description = "User Details")]
        public virtual ApplicationUser? applicationUser { get; set; }

        [Display(Name = "تفاصيل الطلب", Description = "Order Details")]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new HashSet<OrderDetails>();
    }
}
