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
        public string DepartmentName { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
    }
    public class ProductBranchResponse
    {
        public List<ProductDTO> Products { get; set; }
        public List<DepartmentDTO> Departments { get; set; }
        public int? SelectedDepartmentId { get; set; }
    }
    public class CreateProductWithUnitsDTO
    {
        public string Name { get; set; }
        public int Department_Id { get; set; }
        public decimal Price { get; set; }
        public bool? IsActive { get; set; }
        public List<int> UnitIds { get; set; }
        public List<decimal> SpecialPrices { get; set; }
    }
}
