using Domin.System.IRepository.IBaseRepository.IAllBaseRepository;
using Infrastructure.System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.BaseRepository.AllBaseRepository
{
    public class BaseRepository<T> : IAllBaseRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public BaseRepository(AppDbContext context)
        {
            this.context = context;
        }

        public DbSet<T> Db => context.Set<T>();
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.AnyAsync(predicate);
        }
        public async Task<T> AddAsync(T entity)
        {
            try
            {
                var result = await context.AddAsync(entity);
                await context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<AddAsync>");
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public async Task<T> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                var result = context.Remove(entity);
                await context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<DeleteAsync>");
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                return await context.Set<T>().AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<GetAllAsync>!!!!");
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                var res = await context.Set<T>().FindAsync(id);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<GetByIdAsync>");
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                var result = context.Update(entity);
                await context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<UpdateAsync>");
                Console.WriteLine(ex.Message);
                return default;
            }
        }
    }
}
