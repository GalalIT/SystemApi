using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Infrastructure.System.Repository.UnitOfRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
namespace Domin.System.Test.IRepository
{
    

    public class IUnitOfRepositoryTests
    {
        private readonly Mock<IUnitOfRepository> _mockUnitOfWork;

        public IUnitOfRepositoryTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfRepository>();
        }

        [Fact]
        public void Interface_ShouldContainAllExpectedProperties()
        {
            // Arrange
            var properties = typeof(IUnitOfRepository).GetProperties();

            // Assert
            Assert.Equal(9, properties.Length);
            Assert.NotNull(properties.Single(p => p.Name == "_Branch" && p.PropertyType == typeof(IAllBranchRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_Company" && p.PropertyType == typeof(IAllCompanyRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_Department" && p.PropertyType == typeof(IAllDepartmentRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_OrderDetails" && p.PropertyType == typeof(IAllOrderDetailsRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_Order" && p.PropertyType == typeof(IAllOrderRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_Product" && p.PropertyType == typeof(IAllProductRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_Unit" && p.PropertyType == typeof(IAllUnitRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_ProductUnit" && p.PropertyType == typeof(IAllProduct_UnitRepository)));
            Assert.NotNull(properties.Single(p => p.Name == "_User" && p.PropertyType == typeof(IAllUserRepository)));
        }

        [Fact]
        public void Properties_ShouldBeReadOnly()
        {
            // Arrange
            var properties = typeof(IUnitOfRepository).GetProperties();

            // Assert
            foreach (var property in properties)
            {
                Assert.False(property.CanWrite);
                Assert.True(property.CanRead);
            }
        }

        //[Fact]
        //public void Implementation_ShouldSatisfyInterfaceContract()
        //{
        //    // Arrange
        //    var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        //    var unitOfWork = new UnitOfRepository(mockContext.Object);

        //    // Assert
        //    Assert.IsAssignableFrom<IUnitOfRepository>(unitOfWork);
        //}

        //[Fact]
        //public void Properties_ShouldReturnCorrectInterfaceTypes()
        //{
        //    // Arrange
        //    var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        //    var unitOfWork = new UnitOfRepository(mockContext.Object);

        //    // Assert
        //    Assert.IsAssignableFrom<IAllBranchRepository>(unitOfWork._Branch);
        //    Assert.IsAssignableFrom<IAllCompanyRepository>(unitOfWork._Company);
        //    Assert.IsAssignableFrom<IAllDepartmentRepository>(unitOfWork._Department);
        //    Assert.IsAssignableFrom<IAllOrderDetailsRepository>(unitOfWork._OrderDetails);
        //    Assert.IsAssignableFrom<IAllOrderRepository>(unitOfWork._Order);
        //    Assert.IsAssignableFrom<IAllProductRepository>(unitOfWork._Product);
        //    Assert.IsAssignableFrom<IAllUnitRepository>(unitOfWork._Unit);
        //    Assert.IsAssignableFrom<IAllProduct_UnitRepository>(unitOfWork._ProductUnit);
        //    Assert.IsAssignableFrom<IAllUserRepository>(unitOfWork._User);
        //}

        //[Fact]
        //public void Interface_ShouldNotAllowPropertySetters()
        //{
        //    // Arrange
        //    var properties = typeof(IUnitOfRepository).GetProperties();
        //    var mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        //    var unitOfWork = new UnitOfRepository(mockContext.Object);

        //    // Assert
        //    foreach (var property in properties)
        //    {
        //        var setMethod = property.GetSetMethod();
        //        Assert.Null(setMethod); // No setter should exist
        //    }
        //}
    }
}
