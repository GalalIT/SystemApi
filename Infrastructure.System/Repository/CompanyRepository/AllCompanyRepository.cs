using Domin.System.Entities;
using Domin.System.IRepository.ICompanyRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.CompanyRepository
{
    public class AllCompanyRepository : BaseRepository<Company>, IAllCompanyRepository
    {
        public AllCompanyRepository(AppDbContext context) : base(context)
        {
        }
    }
}
