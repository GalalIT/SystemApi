using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class CreateCartDTO
    {
        public List<int> Quantities { get; set; }
        public List<int> ProductUnitIds { get; set; }
        public List<decimal> Prices { get; set; }
        public List<string> Descriptions { get; set; }
        public int? CompanyId { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public string OrderNumber { get; set; }
        public int OrderType { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string UserId { get; set; }
        public int BranchId { get; set; }
    }
}
