using Application.System.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Interface.IBaseOperation
{
    public interface IBaseUpdateAsync<T> where T : class
    {
        Task<Response<T>> UpdateAsync(T entity);

    }
}
