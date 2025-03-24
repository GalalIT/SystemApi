using Domin.System.IRepository.IBaseRepository;
using Infrastructure.System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.BaseRepository
{
    public class GetByIdAsyncBaseRepository<T> : IGetByIdAsyncBaseRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public GetByIdAsyncBaseRepository(AppDbContext context)
        {
            this.context = context;
        }
        public DbSet<T> Db => context.Set<T>();

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                var res = await Db.FindAsync(id);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<GetByIdAsync>");
                Console.WriteLine(ex.Message);
                return default;
            }
        }
    }
}
