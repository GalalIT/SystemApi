
using Domin.System.IRepository.IProduct_UnitRepository;
using Domin.System.IRepository.IUnitOfRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.Product_UnitRepository;
using Infrastructure.System.Repository.UnitOfRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure.System.Extention
{
    public static class AddPresistenceExtenstion
    {
        public static IServiceCollection AddPresistence(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            //////////////////////////////////////////Difine the UseCase ////////////////////


            //services.AddScoped<AllBranchUseCase>();
            //services.AddScoped<AllCompanyUseCase>();
            //services.AddScoped<AllOrderDetailsUseCase>();
            //services.AddScoped<AllProductUseCase>();
            //services.AddScoped<AllUnitUseCase>();
            //services.AddScoped<AllDepartmentUseCase>();
            //services.AddScoped<AllOrderUseCase>();
            //services.AddScoped<AllProduct_UnitUseCase>();

            //////////////////////////////////////////Difine the Services ////////////////////
            
            //services.AddScoped<IAllBranchOparation, AllBranchServices>();
            //services.AddScoped<IAllCompanyOparation, AllCompanyServices>();
            //services.AddScoped<IAllDepartmentOparation, AllDepartmentServices>();
            //services.AddScoped<IAllOrderDetailsOparation, AllOrderDetailsServices>();
            //services.AddScoped<IAllOrderOparation, AllOrderServices>();
            //services.AddScoped<IAllProductOparation, AllProductServices>();
            //services.AddScoped<IAllUnitOparation, AllUnitServices>();
            //services.AddScoped<IAllProduct_UnitOparation, AllProduct_UnitServices>();

            //////////////////////////////////////////Difine the Repository ////////////////////
            ///

            //services.AddScoped<IAllProduct_UnitRepository, AllProduct_UnitRepository>();
            //services.AddScoped<IUnitOfRepository, UnitOfRepository>();

            ///
           /////////////////////////////////////////////////////////////////////////////////////
            return services;
        }
    }
}
