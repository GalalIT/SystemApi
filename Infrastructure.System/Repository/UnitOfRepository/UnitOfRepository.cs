using Domin.System.Entities;
using Domin.System.IRepository.IBranchRepository;
using Domin.System.IRepository.ICompanyRepository;
using Domin.System.IRepository.IDepartmentRepository;
using Domin.System.IRepository.IOrderDetailsRepository;
using Domin.System.IRepository.IOrderRepository;
using Domin.System.IRepository.IProduct_UnitRepository;
using Domin.System.IRepository.IProductRepository;
using Domin.System.IRepository.IUnitOfRepository;
using Domin.System.IRepository.IUnitRepository;
using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BranchRepository;
using Infrastructure.System.Repository.CompanyRepository;
using Infrastructure.System.Repository.DepartmentRepository;
using Infrastructure.System.Repository.OrderDetailsRepository;
using Infrastructure.System.Repository.OrderRepository;
using Infrastructure.System.Repository.Product_UnitRepository;
using Infrastructure.System.Repository.ProductRepository;
using Infrastructure.System.Repository.UnitRepository;
using Infrastructure.System.Repository.UserRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Repository.UnitOfRepository
{
    public class UnitOfRepository : IUnitOfRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILoggerFactory _loggerFactory;

        // Repository properties
        public IAllBranchRepository _Branch { get; private set; }
        public IAllCompanyRepository _Company { get; private set; }
        public IAllDepartmentRepository _Department { get; private set; }
        public IAllOrderDetailsRepository _OrderDetails { get; private set; }
        public IAllOrderRepository _Order { get; private set; }
        public IAllProductRepository _Product { get; private set; }
        public IAllUnitRepository _Unit { get; private set; }
        public IAllProduct_UnitRepository _ProductUnit { get; private set; }
        public IAllUserRepository _User { get; private set; }

        public UnitOfRepository(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _loggerFactory = loggerFactory;

            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            _Branch = new AllBranchRepository(_context);
            _Company = new AllCompanyRepository(_context);
            _Department = new AllDepartmentRepository(_context);
            _OrderDetails = new AllOrderDetailsRepository(_context);
            _Order = new AllOrderRepository(_context);
            _Product = new AllProductRepository(_context);
            _Unit = new AllUnitRepository(_context);
            _ProductUnit = new AllProduct_UnitRepository(_context);
            _User = new AllUserRepository(_roleManager, _userManager, _context, _loggerFactory.CreateLogger<AllUserRepository>());
        }
    }
}
