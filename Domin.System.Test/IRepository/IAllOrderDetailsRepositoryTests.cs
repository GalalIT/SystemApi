using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.System.Entities;
using Domin.System.IRepository.IOrderDetailsRepository;
using global::System.ComponentModel.DataAnnotations;
using global::System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Domin.System.Test.IRepository
{
    public class IAllOrderDetailsRepositoryTests
    {
        private readonly Mock<IAllOrderDetailsRepository> _mockRepo;
        private readonly List<OrderDetails> _testOrderDetails;
        private readonly List<Product_Unit> _testProductUnits;
        private readonly List<Order> _testOrders;

        public IAllOrderDetailsRepositoryTests()
        {
            // Initialize test data with Arabic values
            _testProductUnits = new List<Product_Unit>
        {
            new Product_Unit {
                Id = 1,
                SpecialPrice = 50.99m,
                Product = new Product { Id_Product = 1, Name = "منتج 1" },
                Unit = new Unit { Id_Unit = 1, Name = "كيلو" }
            },
            new Product_Unit {
                Id = 2,
                SpecialPrice = 75.50m,
                Product = new Product { Id_Product = 2, Name = "منتج 2" },
                Unit = new Unit { Id_Unit = 2, Name = "علبة" }
            }
        };

            _testOrders = new List<Order>
        {
            new Order {
                Id_Order = 1,
                Total_Amount = 200.50m,
                Branch = new Branch { Id_Branch = 1, Name = "الفرع الرئيسي" }
            },
            new Order {
                Id_Order = 2,
                Total_Amount = 350.75m,
                Branch = new Branch { Id_Branch = 2, Name = "فرع جدة" }
            }
        };

            _testOrderDetails = new List<OrderDetails>
        {
            new OrderDetails {
                Id_OrderDetail = 1,
                Description_product = "بدون ملح",
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                Order_Id = 1,
                product_Unit = _testProductUnits[0],
                Order = _testOrders[0]
            },
            new OrderDetails {
                Id_OrderDetail = 2,
                Description_product = "إضافة صوص",
                Quantity = 1,
                Total_Price = 75.25m,
                Product_Unit_id = 2,
                Order_Id = 2,
                product_Unit = _testProductUnits[1],
                Order = _testOrders[1]
            }
        };

            _mockRepo = new Mock<IAllOrderDetailsRepository>();

            /* Base Repository Methods */
            var mockDbSet = new Mock<DbSet<OrderDetails>>();
            _mockRepo.Setup(r => r.Db).Returns(mockDbSet.Object);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _testOrderDetails.FirstOrDefault(o => o.Id_OrderDetail == id));

            _mockRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(_testOrderDetails);

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<OrderDetails>()))
                .ReturnsAsync((OrderDetails od) =>
                {
                    // Validate DataAnnotations
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(od, new ValidationContext(od), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    // Validate relationships exist
                    if (!_testProductUnits.Any(pu => pu.Id == od.Product_Unit_id))
                        throw new ValidationException("معرف وحدة المنتج غير صالح");

                    if (!_testOrders.Any(o => o.Id_Order == od.Order_Id))
                        throw new ValidationException("معرف الطلب غير صالح");

                    _testOrderDetails.Add(od);
                    return od;
                });

            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<OrderDetails>()))
                .ReturnsAsync((OrderDetails od) =>
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(od, new ValidationContext(od), validationResults, true))
                    {
                        throw new ValidationException(validationResults.First().ErrorMessage);
                    }

                    var existing = _testOrderDetails.FirstOrDefault(x => x.Id_OrderDetail == od.Id_OrderDetail);
                    if (existing != null)
                    {
                        existing.Description_product = od.Description_product;
                        existing.Quantity = od.Quantity;
                        existing.Total_Price = od.Total_Price;

                        // Update both the ID and the navigation property
                        existing.Product_Unit_id = od.Product_Unit_id;
                        existing.product_Unit = _testProductUnits.FirstOrDefault(pu => pu.Id == od.Product_Unit_id);

                        existing.Order_Id = od.Order_Id;
                        existing.Order = _testOrders.FirstOrDefault(o => o.Id_Order == od.Order_Id);
                    }
                    return existing;
                });

            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var toDelete = _testOrderDetails.FirstOrDefault(o => o.Id_OrderDetail == id);
                    if (toDelete != null) _testOrderDetails.Remove(toDelete);
                    return toDelete;
                });

            _mockRepo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<OrderDetails, bool>>>()))
                .ReturnsAsync((Expression<Func<OrderDetails, bool>> predicate) =>
                    _testOrderDetails.Any(predicate.Compile()));
        }

        /* Validation Tests */
        [Theory]
        [InlineData(0, 1, 1, 10.50, "يجب أن تكون الكمية أكبر من صفر")] // Quantity = 0
        [InlineData(1, 0, 1, 10.50, "معرف وحدة المنتج غير صالح")] // Invalid Product_Unit
        [InlineData(1, 1, 0, 10.50, "معرف الطلب غير صالح")] // Invalid Order
        [InlineData(1, 1, 1, 0, "يجب أن يكون السعر أكبر من صفر")] // Price = 0
        public async Task AddAsync_InvalidData_ThrowsValidationException(
            int quantity, int productUnitId, int orderId, decimal price, string expectedError)
        {
            var invalidDetail = new OrderDetails
            {
                Quantity = quantity,
                Product_Unit_id = productUnitId,
                Order_Id = orderId,
                Total_Price = price
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidDetail));

            Assert.Contains(expectedError, ex.Message);
        }

        [Fact]
        public async Task AddAsync_ValidOrderDetail_Success()
        {
            var validDetail = new OrderDetails
            {
                Description_product = "إضافة جبن",
                Quantity = 3,
                Total_Price = 150.75m,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            var result = await _mockRepo.Object.AddAsync(validDetail);

            Assert.Equal("إضافة جبن", result.Description_product);
            Assert.Equal(3, result.Quantity);
            Assert.Equal(150.75m, result.Total_Price);
            Assert.Contains(validDetail, _testOrderDetails);
        }

        /* Relationship Tests */
        [Fact]
        public async Task GetByIdAsync_IncludesNavigationProperties()
        {
            var result = await _mockRepo.Object.GetByIdAsync(1);

            Assert.NotNull(result?.product_Unit);
            Assert.NotNull(result?.Order);
            Assert.Equal("كيلو", result.product_Unit.Unit?.Name);
            Assert.Equal("الفرع الرئيسي", result.Order.Branch?.Name);
        }

        [Fact]
        public async Task UpdateAsync_ChangeOrder_UpdatesRelationship()
        {
            var update = new OrderDetails
            {
                Id_OrderDetail = 1,
                Product_Unit_id = 1,
                Order_Id = 2, // Change to second order
                Quantity = 2,
                Total_Price = 100.50m
            };

            var result = await _mockRepo.Object.UpdateAsync(update);
            Assert.Equal(2, result?.Order_Id);
            Assert.Equal("فرع جدة", result?.Order?.Branch?.Name);
        }
        [Fact]
        public async Task UpdateAsync_ChangeProductUnit_UpdatesRelationship()
        {
            // Arrange
            var update = new OrderDetails
            {
                Id_OrderDetail = 1,
                Product_Unit_id = 2, // Change to second product unit ("علبة")
                Order_Id = 1,
                Quantity = 2,
                Total_Price = 100.50m
            };

            // Act
            var result = await _mockRepo.Object.UpdateAsync(update);

            // Assert
            Assert.Equal(2, result?.Product_Unit_id);
            Assert.Equal("علبة", result?.product_Unit?.Unit?.Name); // Now passes
            Assert.Equal("منتج 2", result?.product_Unit?.Product?.Name); // Verify product name too
        }
        /* Edge Cases */
        [Fact]
        public async Task AddAsync_MaxLengthDescription_Success()
        {
            var longDescription = new string('a', 500); // Max length
            var validDetail = new OrderDetails
            {
                Description_product = longDescription,
                Quantity = 1,
                Total_Price = 50.99m,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            var result = await _mockRepo.Object.AddAsync(validDetail);
            Assert.Equal(500, result.Description_product?.Length);
        }

        [Fact]
        public async Task AddAsync_ExceedsMaxLengthDescription_ThrowsException()
        {
            var longDescription = new string('a', 501); // Exceeds max length
            var invalidDetail = new OrderDetails
            {
                Description_product = longDescription,
                Quantity = 1,
                Total_Price = 50.99m,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                _mockRepo.Object.AddAsync(invalidDetail));

            Assert.Contains("يجب ألا تتجاوز الملاحظة 500 حرف", ex.Message);
        }

        /* Bulk Operations */
        [Fact]
        public async Task GetAllAsync_ReturnsCompleteDataSet()
        {
            var result = await _mockRepo.Object.GetAllAsync();
            Assert.Equal(2, result.Count);
            Assert.All(result, item => Assert.NotNull(item.product_Unit));
            Assert.All(result, item => Assert.NotNull(item.Order));
        }
    }
}
