using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IBaseOperation
{
    public interface IBaseGetByIdAsync<T> where T : class
    {
        Task<Response<T>> GetByIdAsync(int id);

    }
}
