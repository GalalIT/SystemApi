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
using Domin.System.IRepository.IUnitOfRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Extention;
using Infrastructure.System.Repository.UnitOfRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.System.Tests.Extention
{
    public class AddPresistenceExtensionTests
    {
        [Fact]
        public void AddPresistence_WithValidConfiguration_RegistersAllDependencies()
        {
            // Arrange
            var services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string> {
        {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;"}
    };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // 🔧 Add logging for ILogger<T> dependencies
            services.AddLogging();

            // Act
            services.AddPresistence(configuration);

            // Assert
            var provider = services.BuildServiceProvider();

            // Check DbContext
            Assert.NotNull(provider.GetService<AppDbContext>());

            // UseCases
            Assert.NotNull(provider.GetService<IBranchUseCase>());
            Assert.NotNull(provider.GetService<ICompanyUseCase>());
            Assert.NotNull(provider.GetService<IProductUseCase>());
            Assert.NotNull(provider.GetService<IUnitUseCase>());
            Assert.NotNull(provider.GetService<IDepartmentUseCase>());
            Assert.NotNull(provider.GetService<IOrderUseCase>());

            // Services
            Assert.NotNull(provider.GetService<IAllBranchOperation>());
            Assert.NotNull(provider.GetService<IAllCompanyOperation>());
            Assert.NotNull(provider.GetService<IAllDepartmentOperation>());
            Assert.NotNull(provider.GetService<IAllOrderDetailsOperation>());
            Assert.NotNull(provider.GetService<IAllOrderOperation>());
            Assert.NotNull(provider.GetService<IAllProductOperation>());
            Assert.NotNull(provider.GetService<IAllUnitOperation>());
            Assert.NotNull(provider.GetService<IAllProduct_UnitOperation>());

            // Repository
            Assert.NotNull(provider.GetService<IUnitOfRepository>());
        }

        [Fact]
        public void AddPresistence_ThrowsInvalidOperationException_IfConnectionStringIsMissing()
        {
            // Arrange
            var services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string>(); // No connection string

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => services.AddPresistence(configuration));
        }

        
        [Fact]
        public void AddPresistence_ThrowsArgumentNullException_IfConfigurationIsNull()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => services.AddPresistence(null));
        }

        //[Fact]
        //public void AddPresistence_ThrowsInvalidOperationException_IfConnectionStringIsMissing()
        //{
        //    // Arrange
        //    var services = new ServiceCollection();
        //    var configurationMock = new Mock<IConfiguration>();
        //    configurationMock.Setup(c => c.GetConnectionString("DefaultConnection")).Returns(string.Empty);

        //    // Act & Assert
        //    Assert.Throws<InvalidOperationException>(() => services.AddPresistence(configurationMock.Object));
        //}
    }
}
