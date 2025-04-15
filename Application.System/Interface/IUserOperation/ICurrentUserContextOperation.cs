using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IUserOperation
{
    public interface ICurrentUserContextOperation
    {
        string? UserId { get; }
    }
}
