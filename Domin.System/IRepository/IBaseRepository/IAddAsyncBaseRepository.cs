using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IBaseRepository
{
    public interface IAddAsyncBaseRepository<T> where T : class
    {
        //IRepository
        Task<T> AddAsync(T entity);
    }
}
