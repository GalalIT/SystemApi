
using Application.System.Interface.IBranchOperation;
using Application.System.Interface.ICompanyOperation;
using Application.System.Interface.IDepartmentOperation;
using Application.System.Interface.IOrderDetailsOperation;
using Application.System.Interface.IOrderOperation;
using Application.System.Interface.IProduct_UnitOperation;
using Application.System.Interface.IProductOperation;
using Application.System.Interface.IUnitOperation;
using Application.System.Interface.IUserOperation;
using Application.System.Services.BranchServices;
using Application.System.Services.CompanyServices;
using Application.System.Services.DepartmentServices;
using Application.System.Services.OrderDetailsServices;
using Application.System.Services.OrderServices;
using Application.System.Services.Product_UnitServices;
using Application.System.Services.ProductServices;
using Application.System.Services.UnitServices;
using Application.System.Services.UserServices;
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
using Application.System.UseCace.UserUseCases.Implement;
using Application.System.UseCace.UserUseCases.Interface;
using Domin.System.Entities;
using Domin.System.IRepository.IProduct_UnitRepository;
using Domin.System.IRepository.IUnitOfRepository;
using Domin.System.IRepository.IUserRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.Product_UnitRepository;
using Infrastructure.System.Repository.UnitOfRepository;
using Infrastructure.System.Repository.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;


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
            //services.AddLogging();
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

            //services.AddRateLimiter(options =>
            //{
            //    options.AddFixedWindowLimiter("LoginLimit", opt =>
            //    {
            //        opt.PermitLimit = 2; // 5 محاولات تسجيل دخول فقط
            //        opt.Window = TimeSpan.FromMinutes(2); // كل 5 دقائق
            //        opt.QueueLimit = 0; // لا نسمح بأي طلبات في قائمة الانتظار
            //    });

            //    options.OnRejected = (context, _) =>
            //    {
            //        Console.WriteLine($"Rate limit exceeded for IP: {context.HttpContext.Connection.RemoteIpAddress}");
            //        context.HttpContext.Response.StatusCode = 429;
            //        context.HttpContext.Response.WriteAsync("Too many login attempts. Please try again later.");
            //        return new ValueTask();
            //    };
            //});
            //services.AddRateLimiter(options =>
            //{
            //    options.AddFixedWindowLimiter("LoginLimit", opt =>
            //    {
            //        opt.PermitLimit = 2;
            //        opt.Window = TimeSpan.FromMinutes(2);
            //        opt.QueueLimit = 0;
            //    });

            //    options.OnRejected = async (context, cancellationToken) =>
            //    {
            //        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            //        await context.HttpContext.Response.WriteAsync(
            //            "Too many login attempts. Please try again later.",
            //            cancellationToken);
            //    };
            //});
            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            //////////////////////////////////////////Difine the UseCase ////////////////////


            services.AddScoped<IBranchUseCase, BranchUseCase>();
            services.AddScoped<ICompanyUseCase, CompanyUseCase>();
            services.AddScoped<IProductUseCase, ProductUseCase>();
            services.AddScoped<IUnitUseCase, UnitUseCase>();
            services.AddScoped<IDepartmentUseCase, DepartmentUseCase>();
            services.AddScoped<IOrderUseCase, OrderUseCase>();
            services.AddScoped<IUserUseCases, UserUseCases>();

            //////////////////////////////////////////Difine the Services ////////////////////

            services.AddScoped<IAllBranchOperation, AllBranchServices>();
            services.AddScoped<IAllCompanyOperation, AllCompanyServices>();
            services.AddScoped<IAllDepartmentOperation, AllDepartmentServices>();
            services.AddScoped<IAllOrderDetailsOperation, AllOrderDetailsServices>();
            services.AddScoped<IAllOrderOperation, AllOrderServices>();
            services.AddScoped<IAllProductOperation, AllProductServices>();
            services.AddScoped<IAllUnitOperation, AllUnitServices>();
            services.AddScoped<IAllProduct_UnitOperation, AllProduct_UnitServices>();
            services.AddScoped<IAllUserOperation, AllUserServices>();
            services.AddScoped<ITokenGenerationOperation, JwtTokenService>();
            //services.AddScoped<IAllUserOperation, AllUserService>();

            //////////////////////////////////////////Difine the Repository ////////////////////
            ///

            services.AddScoped<IUnitOfRepository, UnitOfRepository>();
            services.AddScoped<IAllRoleRepository, AllUserRepository>();

            ///
            /////////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////// JWTSetting  ////////////////////
            var JWTSetting = configuration.GetSection("JWTSetting");
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = JWTSetting["ValidAudience"],
                    ValidIssuer = JWTSetting["ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("SecretKey").Value))
                };
            });
            return services;
        }
    }
}
