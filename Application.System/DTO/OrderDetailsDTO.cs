using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class OrderDetailsDTO
    {
        public int Id_OrderDetail { get; set; }
        public string? Description_product { get; set; }
        public int Quantity { get; set; }
        public decimal Total_Price { get; set; }
        public int Product_Unit_id { get; set; }
        public int? Order_Id { get; set; }
    }
}
