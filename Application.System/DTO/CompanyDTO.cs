using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class CompanyDTO
    {
        public int Id_Company { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int DiscountRate { get; set; }
    }
}
