
using Application.System.Interface.IBranchOperation;
using Application.System.Interface.ICompanyOperation;
using Application.System.Interface.IDepartmentOperation;
using Application.System.Interface.IOrderDetailsOperation;
using Application.System.Interface.IOrderOperation;
using Application.System.Interface.IProduct_UnitOperation;
using Application.System.Interface.IProductOperation;
using Application.System.Interface.IUnitOperation;
using Application.System.Services.BranchServices;
using Application.System.Services.CompanyServices;
using Application.System.Services.DepartmentServices;
using Application.System.Services.OrderDetailsServices;
using Application.System.Services.OrderServices;
using Application.System.Services.Product_UnitServices;
using Application.System.Services.ProductServices;
using Application.System.Services.UnitServices;
using Application.System.UseCace.BranchUseCase.Implement;
using Application.System.UseCace.BranchUseCase.Interface;
using Application.System.UseCace.CompanyUseCase.Implement;
using Application.System.UseCace.CompanyUseCase.Interface;
using Application.System.UseCace.DepartmentUseCase.Implement;
using Application.System.UseCace.DepartmentUseCase.Interface;
using Application.System.UseCace.OrderUseCase.Implement;
using Application.System.UseCace.OrderUseCase.Interface;
using Application.System.UseCace.ProductUseCase.Implement;
using Application.System.UseCace.ProductUseCase.Interface;
using Application.System.UseCace.UnitUseCase.Implement;
using Application.System.UseCace.UnitUseCase.Interface;
using Domin.System.Entities;
using Domin.System.IRepository.IProduct_UnitRepository;
using Domin.System.IRepository.IUnitOfRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.Product_UnitRepository;
using Infrastructure.System.Repository.UnitOfRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Infrastructure.System.Extention
{
    public static class AddPresistenceExtenstion
    {
        public static IServiceCollection AddPresistence(this IServiceCollection services, IConfiguration configuration)
        {
            
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is missing or empty");
            }

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Customize password requirements
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            //////////////////////////////////////////Difine the UseCase ////////////////////


            services.AddScoped<IBranchUseCase, BranchUseCase>();
            services.AddScoped<ICompanyUseCase, CompanyUseCase>();
            services.AddScoped<IProductUseCase, ProductUseCase>();
            services.AddScoped<IUnitUseCase, UnitUseCase>();
            services.AddScoped<IDepartmentUseCase, DepartmentUseCase>();
            services.AddScoped<IOrderUseCase, OrderUseCase>();

            //////////////////////////////////////////Difine the Services ////////////////////

            services.AddScoped<IAllBranchOperation, AllBranchServices>();
            services.AddScoped<IAllCompanyOperation, AllCompanyServices>();
            services.AddScoped<IAllDepartmentOperation, AllDepartmentServices>();
            services.AddScoped<IAllOrderDetailsOperation, AllOrderDetailsServices>();
            services.AddScoped<IAllOrderOperation, AllOrderServices>();
            services.AddScoped<IAllProductOperation, AllProductServices>();
            services.AddScoped<IAllUnitOperation, AllUnitServices>();
            services.AddScoped<IAllProduct_UnitOperation, AllProduct_UnitServices>();
            //services.AddScoped<IAllUserOperation, AllUserService>();

            //////////////////////////////////////////Difine the Repository ////////////////////
            ///

            services.AddScoped<IUnitOfRepository, UnitOfRepository>();

            ///
            /////////////////////////////////////////////////////////////////////////////////////
            return services;
        }
    }
}
