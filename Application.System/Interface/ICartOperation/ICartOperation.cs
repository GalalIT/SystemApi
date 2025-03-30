using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.ICartOperation
{
    public interface ICartOperation
    {
        Task<Response<int>> ProcessCartAsync(CreateCartDTO cartDto);

    }
}
