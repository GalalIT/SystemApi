using Domin.System.Entities;
using Domin.System.IRepository.IOrderRepository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Test.IRepository
{
    public class IAllOrderRepositoryTests
    {
        private readonly Mock<IAllOrderRepository> _mockRepo;
        private readonly List<Order> _testOrders;
        private readonly List<OrderDetails> _testOrderDetails;
        private readonly List<Branch> _testBranches;
        private readonly List<Company> _testCompanies;
        private readonly List<ApplicationUser> _testUsers;
        private readonly List<Product_Unit> _testProductUnits;

        public IAllOrderRepositoryTests()
        {
            // Initialize Arabic test data
            _testBranches = new List<Branch>
        {
            new Branch {
                Id_Branch = 1,
                Name = "الفرع الرئيسي",
                Address = "شارع الملك عبدالعزيز",
                City = "الرياض",
                Phone = "0501234567",
                IsActive = true
            }
        };

            _testCompanies = new List<Company>
        {
            new Company {
                Id_Company = 1,
                Name = "شركة التقنية",
                FromDate = DateTime.Now.AddYears(-1),
                ToDate = DateTime.Now.AddYears(1),
                DiscountRate = 10
            }
        };

            _testUsers = new List<ApplicationUser>
        {
            new ApplicationUser {
                Id = "user1",
                Name = "محمد أحمد",
                Branch_Id = 1,
                Branch = _testBranches[0]
            }
        };

            _testProductUnits = new List<Product_Unit>
        {
            new Product_Unit {
                Id = 1,
                ProductId = 1,
                UnitId = 1,
                SpecialPrice = 50.99m
            }
        };

            _testOrderDetails = new List<OrderDetails>
        {
            new OrderDetails {
                Id_OrderDetail = 1,
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                product_Unit = _testProductUnits[0]
            }
        };

            _testOrders = new List<Order>
        {
            new Order {
                Id_Order = 1,
                Total_Amount = 200.50m,
                Branch_Id = 1,
                Company_id = 1,
                User_id = "user1",
                OrderType = 1,
                Branch = _testBranches[0],
                Company = _testCompanies[0],
                applicationUser = _testUsers[0],
                OrderDetails = new List<OrderDetails> { _testOrderDetails[0] }
            }
        };

            _mockRepo = new Mock<IAllOrderRepository>();

            /* Base Repository Methods */
            var mockDbSet = new Mock<DbSet<Order>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testOrders.FirstOrDefault(o => o.Id_Order == id));

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testOrders);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(o, new ValidationContext(o), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }
                    _testOrders.Add(o);
                    return o;
                });

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
                .ReturnsAsync((Order o) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(o, new ValidationContext(o), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    var existing = _testOrders.FirstOrDefault(x => x.Id_Order == o.Id_Order);
                    if (existing != null)
                    {
                        existing.Total_Amount = o.Total_Amount;
                        existing.OrderType = o.OrderType;
                        // Update other properties as needed
                    }
                    return existing;
                });

            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testOrders.FirstOrDefault(o => o.Id_Order == id);
                    if (toDelete != null) _testOrders.Remove(toDelete);
                    return toDelete;
                });

            /* Custom Methods */
            _mockRepo.Setup(r => r.GetOrderWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testOrders
                    .Where(o => o.Id_Order == id)
                    .Select(o => new Order
                    {
                        Id_Order = o.Id_Order,
                        OrderDetails = o.OrderDetails.Select(od => new OrderDetails
                        {
                            Id_OrderDetail = od.Id_OrderDetail,
                            product_Unit = od.product_Unit
                        }).ToList(),
                        Branch = o.Branch,
                        Company = o.Company,
                        applicationUser = o.applicationUser
                    })
                    .FirstOrDefault());

            _mockRepo.Setup(r => r.AnyOrderDetailsWithProductAsync(It.IsAny<int>()))
                .ReturnsAsync((int productId) => _testOrderDetails
                    .Any(od => od.product_Unit.ProductId == productId));
        }

        /* Base Repository Tests */
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsOrder()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("الفرع الرئيسي", result.Branch.Name);
        }

        /* Custom Method Tests */
        [Fact]
        public async Task GetOrderWithDetailsAsync_ReturnsFullOrderGraph()
        {
            var result = await _mockRepo.Object.GetOrderWithDetailsAsync(1);

            Assert.NotNull(result);
            Assert.Single(result.OrderDetails);
            Assert.Equal("محمد أحمد", result.applicationUser.Name);
            Assert.Equal("شركة التقنية", result.Company.Name);
            Assert.NotNull(result.OrderDetails.First().product_Unit);
        }

        [Theory]
        [InlineData(1, true)]  // Product exists in orders
        [InlineData(99, false)] // Product doesn't exist
        public async Task AnyOrderDetailsWithProductAsync_ReturnsCorrectResult(int productId, bool expected)
        {
            var result = await _mockRepo.Object.AnyOrderDetailsWithProductAsync(productId);
            Assert.Equal(expected, result);
        }

        /* Validation Tests */
        [Theory]
        [InlineData(0, 1, "user1", 1, "يجب أن يكون السعر أكبر من 50")] // Invalid amount
        [InlineData(100, 0, "user1", 1, "يجب أن يكون نوع الطلب 1 (محلي) أو 2 (سفري)")] // Invalid type
        [InlineData(100, 1, "", 1, "معرف المستخدم مطلوب")] // Invalid user
        [InlineData(100, 1, "user1", 0, "معرف الفرع غير صالح")] // Invalid branch
        public async Task AddAsync_InvalidData_ThrowsValidationException(
            decimal amount, int type, string userId, int branchId, string expectedError)
        {
            var invalidOrder = new Order
            {
                Total_Amount = amount,
                OrderType = type,
                User_id = userId,
                Branch_Id = branchId,
                Company_id = 1
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidOrder));

            Assert.Contains(expectedError, ex.Message);
        }

        /* Relationship Tests */
        [Fact]
        public async Task GetOrderWithDetailsAsync_IncludesProductUnitProduct()
        {
            // Arrange - Add product to product unit
            _testProductUnits[0].Product = new Product { Id_Product = 1, Name = "منتج اختبار" };

            // Act
            var result = await _mockRepo.Object.GetOrderWithDetailsAsync(1);

            // Assert
            Assert.Equal("منتج اختبار", result.OrderDetails.First().product_Unit.Product.Name);
        }

        /* Edge Cases */
        [Fact]
        public async Task GetOrderWithDetailsAsync_NonExistingId_ReturnsNull()
        {
            var result = await _mockRepo.Object.GetOrderWithDetailsAsync(99);
            Assert.Null(result);
        }
    }
}
