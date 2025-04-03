using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.IRepository.IBaseRepository.IAllBaseRepository
{
    public interface IAllBaseRepository<T>  where T : class
    {
        DbSet<T> Db { get; }
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
