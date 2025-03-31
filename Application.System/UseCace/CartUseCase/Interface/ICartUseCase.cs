using Application.System.DTO;
using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.UseCace.CartUseCase.Interface
{
    public interface ICartUseCase
    {
        Task<Response<int>> ProcessCartAsync(CreateCartDTO cartDto);
    }
}
