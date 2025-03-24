using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Domin.System.Entities;

namespace Infrastructure.System.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connection = "ConnectionStrings";
                optionsBuilder.UseSqlServer(connection, b => b.MigrationsAssembly("DefaultConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseCollation("Arabic_CI_AI"); // Replace with the appropriate Arabic collation
            modelBuilder.Entity<Order>()
                .HasOne(o => o.applicationUser)
                .WithMany()
                .HasForeignKey(o => o.User_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Branch>().HasData(
            new Branch
            {
                Id_Branch = 1, // Set the Id explicitly, make sure this ID doesn't conflict with future entries
                Name = "Default Branch",
                Address = "123 Default St.",
                City = "Default City",
                Phone = "775128735",
                IsActive = true

            });

            // Configure your entity mappings here
        }
        public DbSet<Branch> branches { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<Department> departments { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetails> orderDetails { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<Unit> units { get; set; }
        public DbSet<Product_Unit> Product_Units { get; set; }
    }
}
