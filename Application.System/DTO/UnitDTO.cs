using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class UnitDTO
    {
        public int Id_Unit { get; set; }
        public string Name { get; set; }
        public int Branch_Id { get; set; }
        public string BranchName { get; set; }

    }

}
