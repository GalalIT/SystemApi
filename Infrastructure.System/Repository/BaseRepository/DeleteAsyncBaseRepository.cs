using Domin.System.IRepository.IBaseRepository;
using Infrastructure.System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.BaseRepository
{
    public class DeleteAsyncBaseRepository<T> : IDeleteAsyncBaseRepository<T> where T : class
    {
        private readonly AppDbContext context;
        private readonly GetByIdAsyncBaseRepository<T> getByIdRepository;

        public DeleteAsyncBaseRepository(AppDbContext context, GetByIdAsyncBaseRepository<T> getByIdRepository)
        {
            this.context = context;
            this.getByIdRepository = getByIdRepository;
        }
        public async Task<T> DeleteAsync(int id)
        {
            try
            {
                var entity = await getByIdRepository.GetByIdAsync(id);
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
    }
}
