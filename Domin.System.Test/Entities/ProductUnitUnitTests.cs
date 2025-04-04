using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.System.Entities;
using global::System.ComponentModel.DataAnnotations;

namespace Domin.System.Test.Entities
{
    

    public class ProductUnitUnitTests
    {
        private (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        // Validation Tests
        [Fact]
        public void Should_Pass_When_Valid()
        {
            var pu = new Product_Unit
            {
                ProductId = 1,
                UnitId = 1,
                SpecialPrice = 10.99m
            };

            var (isValid, errors) = ValidateModel(pu);
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_InvalidProductId(int productId)
        {
            var pu = new Product_Unit
            {
                ProductId = productId,
                UnitId = 1,
                SpecialPrice = 10.99m
            };

            var (isValid, errors) = ValidateModel(pu);
            Assert.False(isValid);
            Assert.Contains(errors, e => e.MemberNames.Contains("ProductId"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Fail_When_InvalidUnitId(int unitId)
        {
            var pu = new Product_Unit
            {
                ProductId = 1,
                UnitId = unitId,
                SpecialPrice = 10.99m
            };

            var (isValid, errors) = ValidateModel(pu);
            Assert.False(isValid);
            Assert.Contains(errors, e => e.MemberNames.Contains("UnitId"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5.99)]
        public void Should_Fail_When_InvalidPrice(decimal price)
        {
            var pu = new Product_Unit
            {
                ProductId = 1,
                UnitId = 1,
                SpecialPrice = price
            };

            var (isValid, errors) = ValidateModel(pu);
            Assert.False(isValid);
            Assert.Contains(errors, e => e.MemberNames.Contains("SpecialPrice"));
        }

        [Fact]
        public void Should_Fail_When_MissingRequiredFields()
        {
            var pu = new Product_Unit();
            var (isValid, errors) = ValidateModel(pu);

            Assert.False(isValid);
            Assert.Contains(errors, e => e.MemberNames.Contains("ProductId"));
            Assert.Contains(errors, e => e.MemberNames.Contains("UnitId"));
            Assert.Contains(errors, e => e.MemberNames.Contains("SpecialPrice"));
        }

        [Fact]
        public void Should_Set_NavigationProperties()
        {
            // Arrange
            var product = new Product { Id_Product = 1 };
            var unit = new Unit { Id_Unit = 1 };

            // Act
            var pu = new Product_Unit
            {
                // Set both navigation properties and foreign keys
                ProductId = product.Id_Product,
                UnitId = unit.Id_Unit,
                Product = product,
                Unit = unit
            };

            // Assert
            Assert.Equal(1, pu.ProductId);
            Assert.Equal(1, pu.UnitId);
            Assert.Same(product, pu.Product);
            Assert.Same(unit, pu.Unit);
        }
    }
}
