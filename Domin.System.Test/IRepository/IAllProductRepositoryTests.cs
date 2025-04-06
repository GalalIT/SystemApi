using Domin.System.Entities;
using Domin.System.IRepository.IProductRepository;
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
    public class IAllProductRepositoryTests
    {
        private readonly Mock<IAllProductRepository> _mockRepo;
        private readonly List<Product> _testProducts;
        private readonly List<Department> _testDepartments;
        private readonly List<Product_Unit> _testProductUnits;
        private readonly List<Unit> _testUnits;
        private readonly List<OrderDetails> _testOrderDetails;

        public IAllProductRepositoryTests()
        {
            // Initialize Arabic test data
            _testDepartments = new List<Department>
        {
            new Department {
                Id_Department = 1,
                Name = "قسم المأكولات",
                Branch_Id = 1,
                Products = new List<Product>()
            },
            new Department {
                Id_Department = 2,
                Name = "قسم المشروبات",
                Branch_Id = 2,
                Products = new List<Product>()
            }
        };

            _testUnits = new List<Unit>
        {
            new Unit {
                Id_Unit = 1,
                Name = "حبة",
                Branch_Id = 1,
                ProductUnits = new List<Product_Unit>()
            },
            new Unit {
                Id_Unit = 2,
                Name = "لتر",
                Branch_Id = 2,
                ProductUnits = new List<Product_Unit>()
            }
        };

            _testProducts = new List<Product>
        {
            new Product {
                Id_Product = 1,
                Name = "بيتزا",
                Price = 50.99m,
                Department_Id = 1,
                Department = _testDepartments[0],
                ProductUnits = new List<Product_Unit>()
            },
            new Product {
                Id_Product = 2,
                Name = "عصير",
                Price = 15.50m,
                Department_Id = 2,
                Department = _testDepartments[1],
                ProductUnits = new List<Product_Unit>()
            }
        };

            // Add products to departments
            _testDepartments[0].Products.Add(_testProducts[0]);
            _testDepartments[1].Products.Add(_testProducts[1]);

            _testProductUnits = new List<Product_Unit>
        {
            new Product_Unit {
                Id = 1,
                ProductId = 1,
                UnitId = 1,
                SpecialPrice = 45.99m,
                Product = _testProducts[0],
                Unit = _testUnits[0]
            },
            new Product_Unit {
                Id = 2,
                ProductId = 2,
                UnitId = 2,
                SpecialPrice = 14.00m,
                Product = _testProducts[1],
                Unit = _testUnits[1]
            }
        };

            // Add product units to products and units
            _testProducts[0].ProductUnits.Add(_testProductUnits[0]);
            _testProducts[1].ProductUnits.Add(_testProductUnits[1]);
            _testUnits[0].ProductUnits.Add(_testProductUnits[0]);
            _testUnits[1].ProductUnits.Add(_testProductUnits[1]);

            _testOrderDetails = new List<OrderDetails>
        {
            new OrderDetails {
                Id_OrderDetail = 1,
                product_Unit = _testProductUnits[0]
            }
        };

            _mockRepo = new Mock<IAllProductRepository>();

            /* Base Repository Methods */
            var mockDbSet = new Mock<DbSet<Product>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testProducts.FirstOrDefault(p => p.Id_Product == id));

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testProducts);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(p, new ValidationContext(p), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }
                    _testProducts.Add(p);
                    return p;
                });

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(p, new ValidationContext(p), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    var existing = _testProducts.FirstOrDefault(x => x.Id_Product == p.Id_Product);
                    if (existing != null)
                    {
                        existing.Name = p.Name;
                        existing.Price = p.Price;
                        existing.Department_Id = p.Department_Id;
                        existing.Department = p.Department;
                    }
                    return existing;
                });

            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testProducts.FirstOrDefault(p => p.Id_Product == id);
                    if (toDelete != null) _testProducts.Remove(toDelete);
                    return toDelete;
                });

            /* Custom Methods */
            _mockRepo.Setup(r => r.GetAllIncludeToDepartmentAsync())
                .ReturnsAsync(() => _testProducts
                    .Select(p => new Product
                    {
                        Id_Product = p.Id_Product,
                        Department = p.Department
                    })
                    .ToList());

            _mockRepo.Setup(r => r.GetAllIncludeToUnitAsync())
                .ReturnsAsync(() => _testProducts
                    .Select(p => new Product
                    {
                        Id_Product = p.Id_Product,
                        ProductUnits = p.ProductUnits
                            .Select(pu => new Product_Unit
                            {
                                Unit = pu.Unit
                            })
                            .ToList()
                    })
                    .ToList());

            _mockRepo.Setup(r => r.GetAllWithIncludesAsync())
                .ReturnsAsync(() => _testProducts
                    .Select(p => new Product
                    {
                        Id_Product = p.Id_Product,
                        Department = new Department
                        {
                            Id_Department = p.Department.Id_Department,
                            Branch_Id = p.Department.Branch_Id
                        },
                        ProductUnits = p.ProductUnits
                            .Select(pu => new Product_Unit
                            {
                                Unit = pu.Unit
                            })
                            .ToList()
                    })
                    .ToList());

            _mockRepo.Setup(r => r.GetAllProductsByUserBranchAsync(It.IsAny<int>()))
                .ReturnsAsync((int branchId) => _testProducts
                    .Where(p => p.Department.Branch_Id == branchId)
                    .ToList());

            _mockRepo.Setup(r => r.HasRelatedRecords(It.IsAny<int>()))
                .ReturnsAsync((int productId) => _testOrderDetails
                    .Any(od => od.product_Unit.ProductId == productId));
        }

        /* Base Repository Tests */
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsProduct()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("بيتزا", result.Name);
        }

        /* Custom Method Tests */
        [Fact]
        public async Task GetAllIncludeToDepartmentAsync_ReturnsProductsWithDepartments()
        {
            var result = await _mockRepo.Object.GetAllIncludeToDepartmentAsync();
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.NotNull(p.Department));
            Assert.Equal("قسم المأكولات", result[0].Department.Name);
        }

        [Fact]
        public async Task GetAllIncludeToUnitAsync_ReturnsProductsWithUnits()
        {
            var result = await _mockRepo.Object.GetAllIncludeToUnitAsync();
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.NotEmpty(p.ProductUnits));
            Assert.Equal("حبة", result[0].ProductUnits.First().Unit.Name);
        }

        [Fact]
        public async Task GetAllWithIncludesAsync_ReturnsFullHierarchy()
        {
            var result = await _mockRepo.Object.GetAllWithIncludesAsync();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result[0].Department);
            Assert.NotEmpty(result[0].ProductUnits);
            Assert.Equal(1, result[0].Department.Branch_Id);
        }

        [Theory]
        [InlineData(1, 1)] // Branch 1 has 1 product
        [InlineData(2, 1)] // Branch 2 has 1 product
        [InlineData(3, 0)] // Non-existent branch
        public async Task GetAllProductsByUserBranchAsync_ReturnsCorrectCount(int branchId, int expectedCount)
        {
            var result = await _mockRepo.Object.GetAllProductsByUserBranchAsync(branchId);
            Assert.Equal(expectedCount, result.Count);
        }

        [Theory]
        [InlineData(1, true)]  // Product 1 has order details
        [InlineData(2, false)] // Product 2 has no order details
        public async Task HasRelatedRecords_ReturnsCorrectResult(int productId, bool expected)
        {
            var result = await _mockRepo.Object.HasRelatedRecords(productId);
            Assert.Equal(expected, result);
        }

        /* Validation Tests */
        [Theory]
        [InlineData("بي", 1, 50.99, "يجب أن يكون اسم المنتج بين 3 و100 حرف")] // Name too short
        [InlineData("بيتزا", 0, 50.99, "معرف القسم غير صالح")] // Invalid department
        [InlineData("بيتزا", 1, 0, "يجب أن يكون السعر أكبر من صفر")] // Invalid price
        public async Task AddAsync_InvalidData_ThrowsValidationException(
            string name, int deptId, decimal price, string expectedError)
        {
            var invalidProduct = new Product
            {
                Name = name,
                Department_Id = deptId,
                Price = price
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidProduct));

            Assert.Contains(expectedError, ex.Message);
        }

        /* Edge Cases */
        [Fact]
        public async Task GetAllProductsByUserBranchAsync_InvalidBranch_ReturnsEmpty()
        {
            var result = await _mockRepo.Object.GetAllProductsByUserBranchAsync(0);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HasRelatedRecords_NonExistingProduct_ReturnsFalse()
        {
            var result = await _mockRepo.Object.HasRelatedRecords(99);
            Assert.False(result);
        }
    }
}
