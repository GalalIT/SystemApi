using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.System.Entities;
using Domin.System.IRepository.IProduct_UnitRepository;
using global::System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Domin.System.Test.IRepository
{
    

    public class IAllProduct_UnitRepositoryTests
    {
        private readonly Mock<IAllProduct_UnitRepository> _mockRepo;
        private readonly List<Product_Unit> _testProductUnits;
        private readonly List<Product> _testProducts;
        private readonly List<Department> _testDepartments;
        private readonly List<Unit> _testUnits;


        public IAllProduct_UnitRepositoryTests()
        {
            // Initialize Arabic test data
            _testDepartments = new List<Department>
        {
            new Department {
                Id_Department = 1,
                Name = "قسم المأكولات",
                Branch_Id = 1
            }
        };

            _testProducts = new List<Product>
        {
            new Product {
                Id_Product = 1,
                Name = "بيتزا",
                Department_Id = 1,
                Price = 50.99m,
                Department = _testDepartments[0]
            },
            new Product {
                Id_Product = 2,
                Name = "برجر",
                Department_Id = 1,
                Price = 35.50m,
                Department = _testDepartments[0]
            }
        };

            _testUnits = new List<Unit>
        {
            new Unit {
                Id_Unit = 1,
                Name = "حبة",
                Branch_Id = 1
            },
            new Unit {
                Id_Unit = 2,
                Name = "كيلو",
                Branch_Id = 1
            }
        };

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
                ProductId = 1,
                UnitId = 2,
                SpecialPrice = 450.00m,
                Product = _testProducts[0],
                Unit = _testUnits[1]
            },
            new Product_Unit {
                Id = 3,
                ProductId = 2,
                UnitId = 1,
                SpecialPrice = 32.50m,
                Product = _testProducts[1],
                Unit = _testUnits[0]
            }
        };

            // Setup navigation properties
            _testProducts[0].ProductUnits = new List<Product_Unit> { _testProductUnits[0], _testProductUnits[1] };
            _testProducts[1].ProductUnits = new List<Product_Unit> { _testProductUnits[2] };
            _testUnits[0].ProductUnits = new List<Product_Unit> { _testProductUnits[0], _testProductUnits[2] };
            _testUnits[1].ProductUnits = new List<Product_Unit> { _testProductUnits[1] };
            _testDepartments[0].Products = new List<Product> { _testProducts[0], _testProducts[1] };


            _mockRepo = new Mock<IAllProduct_UnitRepository>();

            /* Base Repository Methods */
            var mockDbSet = new Mock<DbSet<Product_Unit>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testProductUnits.FirstOrDefault(pu => pu.Id == id));

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testProductUnits);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product_Unit>()))
                .ReturnsAsync((Product_Unit pu) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(pu, new ValidationContext(pu), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }
                    _testProductUnits.Add(pu);
                    return pu;
                });

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Product_Unit>()))
                .ReturnsAsync((Product_Unit pu) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(pu, new ValidationContext(pu), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    var existing = _testProductUnits.FirstOrDefault(x => x.Id == pu.Id);
                    if (existing != null)
                    {
                        existing.ProductId = pu.ProductId;
                        existing.UnitId = pu.UnitId;
                        existing.SpecialPrice = pu.SpecialPrice;
                    }
                    return existing;
                });

            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testProductUnits.FirstOrDefault(pu => pu.Id == id);
                    if (toDelete != null) _testProductUnits.Remove(toDelete);
                    return toDelete;
                });

            /* Custom Methods */
            _mockRepo.Setup(r => r.GetAllIncludeProdDepAsync())
                .ReturnsAsync(() => _testProductUnits
                    .Select(pu => new Product_Unit
                    {
                        Id = pu.Id,
                        Product = new Product
                        {
                            Id_Product = pu.Product.Id_Product,
                            Department = pu.Product.Department
                        },
                        Unit = pu.Unit
                    })
                    .ToList());

            _mockRepo.Setup(r => r.GetProductUnitsByProductIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int productId) => _testProductUnits
                    .Where(pu => pu.ProductId == productId)
                    .ToList());
        }

        /* Base Repository Tests */
        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsProductUnit()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("بيتزا", result.Product.Name);
            Assert.Equal("حبة", result.Unit.Name);
        }

        /* Custom Method Tests */
        [Fact]
        public async Task GetAllIncludeProdDepAsync_ReturnsFullHierarchy()
        {
            var result = await _mockRepo.Object.GetAllIncludeProdDepAsync();

            Assert.Equal(3, result.Count);
            Assert.All(result, pu => Assert.NotNull(pu.Product.Department));
            Assert.Equal("قسم المأكولات", result[0].Product.Department.Name);
        }

        [Theory]
        [InlineData(1, 2)] // Product 1 has 2 units
        [InlineData(2, 1)] // Product 2 has 1 unit
        [InlineData(3, 0)] // Non-existent product
        public async Task GetProductUnitsByProductIdAsync_ReturnsCorrectCount(int productId, int expectedCount)
        {
            var result = await _mockRepo.Object.GetProductUnitsByProductIdAsync(productId);
            Assert.Equal(expectedCount, result.Count);
        }

        /* Validation Tests */
        [Theory]
        [InlineData(0, 1, 50.99, "معرف المنتج غير صالح")] // Invalid product
        [InlineData(1, 0, 50.99, "معرف الوحدة غير صالح")] // Invalid unit
        [InlineData(1, 1, 0, "يجب أن يكون السعر الخاص أكبر من صفر")] // Invalid price
        public async Task AddAsync_InvalidData_ThrowsValidationException(
            int productId, int unitId, decimal price, string expectedError)
        {
            var invalidPU = new Product_Unit
            {
                ProductId = productId,
                UnitId = unitId,
                SpecialPrice = price
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidPU));

            Assert.Contains(expectedError, ex.Message);
        }

        /* Relationship Tests */
        [Fact]
        public async Task GetAllIncludeProdDepAsync_IncludesDepartment()
        {
            var result = await _mockRepo.Object.GetAllIncludeProdDepAsync();
            Assert.Equal("قسم المأكولات", result[0].Product.Department.Name);
        }

        [Fact]
        public async Task GetProductUnitsByProductIdAsync_ReturnsCorrectUnits()
        {
            var result = await _mockRepo.Object.GetProductUnitsByProductIdAsync(1);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, pu => pu.Unit.Name == "حبة");
            Assert.Contains(result, pu => pu.Unit.Name == "كيلو");
        }

        /* Edge Cases */
        [Fact]
        public async Task GetAllIncludeProdDepAsync_HandlesNullDepartment()
        {
            // Arrange - Set department to null
            _testProducts[0].Department = null;

            // Act
            var result = await _mockRepo.Object.GetAllIncludeProdDepAsync();

            // Assert
            Assert.Null(result[0].Product.Department);
        }
    }
}
