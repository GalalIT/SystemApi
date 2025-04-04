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
        public int OrderType { get; set; }
        public int Branch_Id { get; set; }
        public int? Company_id { get; set; }
        public string User_id { get; set; }
    }
    public class OrderDetailResponse
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItemDetail> Items { get; set; }
    }

    public class OrderItemDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Description { get; set; }
    }
}
