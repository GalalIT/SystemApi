using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class ProductDTO
    {
        public int Id_Product { get; set; }
        public string Name { get; set; }
        public int Department_Id { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
    }
}
