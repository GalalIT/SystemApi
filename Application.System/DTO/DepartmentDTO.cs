using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class DepartmentDTO
    {
        public int Id_Department { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Branch_Id { get; set; }
    }
}
