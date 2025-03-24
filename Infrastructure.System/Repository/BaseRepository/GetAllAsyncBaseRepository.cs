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
    public class GetAllAsyncBaseRepository<T> : IGetAllAsyncBaseRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public GetAllAsyncBaseRepository(AppDbContext context)
        {
            this.context = context;
        }
        public DbSet<T> Db => context.Set<T>();

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                var res = await Db.ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The code in BaseRepository<GetAllAsync>!!!!");
                Console.WriteLine(ex.Message);
                return default;

            }
        }
    }
}
