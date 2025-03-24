using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.DTO
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; } // Inherited from IdentityUser
        public string Name { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public bool? IsActive { get; set; }
        public int Branch_Id { get; set; }
    }
}
