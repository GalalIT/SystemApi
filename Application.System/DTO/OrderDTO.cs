using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class OrderDTO
    {
        public int Id_Order { get; set; }
        public decimal Total_Amount { get; set; }
        public decimal Total_AmountAfterDiscount { get; set; }
        public decimal Discount { get; set; }
        public DateTime DateTime_Created { get; set; }
        public string? OrderNumber { get; set; }
        public int? OrderType { get; set; }
        public int Branch_Id { get; set; }
        public int? Company_id { get; set; }
        public string User_id { get; set; }
    }
}
