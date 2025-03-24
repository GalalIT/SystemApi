using Domin.System.IRepository.IBaseRepository;
using Infrastructure.System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.BaseRepository
{
    public class UpdateAsyncBaseRepository<T> : IUpdateAsyncBaseRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public UpdateAsyncBaseRepository(AppDbContext context)
        {
            this.context = context;
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
