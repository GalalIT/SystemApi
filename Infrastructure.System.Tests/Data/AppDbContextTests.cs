using Domin.System.Entities;
using Infrastructure.System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.System.Tests.Data
{
    public class AppDbContextTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public AppDbContextTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }


        [Fact]
        public void OnConfiguring_ConfiguresSqlServer_WhenNotConfigured()
        {
            // Arrange
            var logger = new ListLogger();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new ListLoggerProvider(logger));
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Create options with JUST the logger factory
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseLoggerFactory(loggerFactory)
                .Options;

            // Act
            using (var context = new AppDbContext(options))
            {
                var _ = context.Model;
            }

            // Assert
            var hasSqlServerLog = logger.Logs.Any(log =>
                log.Contains("Microsoft.EntityFrameworkCore.SqlServer") ||
                log.Contains("Using SQL Server"));

            Assert.True(hasSqlServerLog,
                $"Expected SQL Server configuration log. Actual logs: {string.Join(Environment.NewLine, logger.Logs)}");
        }
        [Fact]
        public void OnModelCreating_SetsArabicCollation()
        {
            // Arrange & Act
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureCreated();

                // Get the model through the design-time service
                var designTimeModel = context.GetService<IDesignTimeModel>().Model;

                // Assert
                Assert.Equal("Arabic_CI_AI", designTimeModel.GetCollation());
            }
        }

        [Fact]
        public void OnModelCreating_ConfiguresOrderUserRelationship()
        {
            // Arrange & Act
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureCreated();
                var entityType = context.Model.FindEntityType(typeof(Order));
                var foreignKey = entityType.GetForeignKeys()
                    .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(ApplicationUser));

                // Assert
                Assert.NotNull(foreignKey);
                Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
            }
        }

        [Fact]
        public void OnModelCreating_SeedsDefaultBranch()
        {
            // Arrange & Act
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureCreated();

                // Assert
                var branch = context.branches.FirstOrDefault(b => b.Id_Branch == 1);
                Assert.NotNull(branch);
                Assert.Equal("Default Branch", branch.Name);
                Assert.Equal("123 Default St.", branch.Address);
                Assert.True(branch.IsActive);
            }
        }

        [Fact]
        public void DbSets_AreProperlyConfigured()
        {
            // Arrange & Act
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureCreated();

                // Assert
                Assert.NotNull(context.branches);
                Assert.NotNull(context.companies);
                Assert.NotNull(context.departments);
                Assert.NotNull(context.orders);
                Assert.NotNull(context.orderDetails);
                Assert.NotNull(context.products);
                Assert.NotNull(context.units);
                Assert.NotNull(context.Product_Units);
            }
        }

        [Fact]
        public async Task CanAddAndRetrieveEntity()
        {
            // Arrange
            var product = new Product
            {
                Id_Product = 1,
                Name = "Test Product",
                Price = 9.99m,
                Department_Id = 1,
                IsActive = true
            };

            // Act
            using (var context = new AppDbContext(_options))
            {
                context.Database.EnsureCreated();
                context.products.Add(product);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new AppDbContext(_options))
            {
                var result = await context.products.FindAsync(1);
                Assert.NotNull(result);
                Assert.Equal("Test Product", result.Name);
            }
        }

        #region Test Helpers

        private class TestDbContext : AppDbContext
        {
            public TestDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlServer("TestConnection");
                }
            }
        }

        private class ListLogger : ILogger
        {
            public List<string> Logs { get; } = new List<string>();

            public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = $"[{logLevel}] {formatter(state, exception)}";
                Logs.Add(message);
            }
        }

        private class ListLoggerProvider : ILoggerProvider
        {
            private readonly ListLogger _logger;

            public ListLoggerProvider(ListLogger logger)
            {
                _logger = logger;
            }

            public ILogger CreateLogger(string categoryName) => _logger;

            public void Dispose() { }
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }

        #endregion
    }

}
