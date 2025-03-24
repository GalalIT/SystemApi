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
        [Display(Name = "رقم الطلب")]
        public int Id_Order { get; set; }
        [Required]
        [Display(Name = "اجمالي سعر الطلب")]
        [Range(5000, int.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من 50.")]
        public decimal Total_Amount { get; set; }
        public decimal Total_AmountAfterDiscount { get; set; }
        public decimal Discount { get; set; }
        public DateTime DateTime_Created { get; set; }= DateTime.Now;
        public string? OrderNumber {  get; set; }//رقم فاتورة الشركه
        public int? OrderType { get; set; }//this for محلي و سفري 

        [Required]
        [Display(Name = "اسم الفرع")]
        public int Branch_Id { get; set; }
        [ForeignKey("Branch_Id")]
        public virtual Branch? Branch { get; set; }

        [Display(Name = "اسم الشركه")]
        public int? Company_id { get; set; }
        [ForeignKey("Company_id")]
        public virtual Company? Company { get; set; }

        public string User_id { get; set; }
        [ForeignKey("User_id")]
        public virtual ApplicationUser? applicationUser { get; set; }


        //[Required]
        //public string SellerId { get; set; } //this for now how is do or seller this order

    }

}
