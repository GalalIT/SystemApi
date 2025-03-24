using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IBaseRepository
{
    public interface IDeleteAsyncBaseRepository<T> where T : class
    {
        Task<T> DeleteAsync(int id);
    }
}
