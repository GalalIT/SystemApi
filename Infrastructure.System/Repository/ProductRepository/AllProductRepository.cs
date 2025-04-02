using Domin.System.Entities;
using Domin.System.IRepository.IProductRepository;
using Infrastructure.System.Data;
using Infrastructure.System.Repository.BaseRepository.AllBaseRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.System.Repository.ProductRepository
{
    public class AllProductRepository : BaseRepository<Product>, IAllProductRepository
    {
        private readonly AppDbContext context;

        public AllProductRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Product>> GetAllIncludeToDepartmentAsync()
        {
            var res = await context.Set<Product>().Include(pu => pu.Department).ToListAsync();
            return res;
        }

        public async Task<List<Product>> GetAllIncludeToUnitAsync()
        {
        var res = await context.Set<Product>().Include(pu => pu.ProductUnits).ThenInclude(p => p.Unit).ToListAsync();
            return res;
        }
        public async Task<List<Product>> GetAllWithIncludesAsync()
        {
            return await context.Set<Product>()
                .Include(p => p.Department)          // Include the Department
                .Include(p => p.ProductUnits)        // Include the ProductUnits
                    .ThenInclude(pu => pu.Unit)      // Then include the Unit for each ProductUnit
                .ToListAsync();
        }
        public async Task<List<Product>> GetAllProductsByUserBranchAsync(int userBranchId)
        {
           

            if (userBranchId == 0)
            {
                Console.WriteLine("User's branch is not found.");
                return new List<Product>(); // No branch found
            }

            Console.WriteLine($"User Branch ID: {userBranchId}");

            // Fetch products that belong to the user's branch
            var res = await context.Set<Product>()
                                   .Where(p => p.Department.Branch_Id == userBranchId)
                                   .Include(p => p.Department)
                                   .Include(p => p.ProductUnits).ThenInclude(pu => pu.Unit)
                                   .ToListAsync();

            Console.WriteLine($"Products found: {res.Count}");

            return res;
        }
        public async Task<bool> HasRelatedRecords(int productId)
        {
            // Check if there are any order details or other entities linked to this product
            var isInUse = await context.orderDetails
                                          .AnyAsync(od => od.product_Unit.ProductId == productId);

            // Add additional checks for other related entities if needed
            return isInUse;
        }

    }
}
