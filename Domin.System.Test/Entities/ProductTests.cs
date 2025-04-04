using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domin.System.Entities;
namespace Domin.System.Test.Entities
{
    
    public class ProductTests
    {
        private (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        [Fact]
        public void Product_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var product = new Product
            {
                Name = "منتج تجريبي",
                Department_Id = 1,
                Price = 99.99m,
                IsActive = true
            };

            // Act
            var (isValid, errors) = ValidateModel(product);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null, "اسم المنتج مطلوب | Product name is required")]
        [InlineData("", "اسم المنتج مطلوب | Product name is required")]
        [InlineData("ab", "يجب أن يكون اسم المنتج بين 3 و100 حرف | Product name must be between 3 and 100 characters")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
    "يجب أن يكون اسم المنتج بين 3 و100 حرف | Product name must be between 3 and 100 characters")] // 101 chars
        public void Name_InvalidValues_ShouldFailValidation(string name, string expectedError)
        {
            // Arrange
            var product = new Product
            {
                Name = name,
                Department_Id = 1,
                Price = 10.0m
            };

            // Act & Assert (same as before)
            var (isValid, errors) = ValidateModel(product);
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Name"));
        }

        [Theory]
        [InlineData(0, "معرف القسم غير صالح | Invalid department ID")]
        [InlineData(-1, "معرف القسم غير صالح | Invalid department ID")]
        public void DepartmentId_InvalidValues_ShouldFailValidation(int departmentId, string expectedError)
        {
            // Arrange
            var product = new Product
            {
                Name = "منتج صالح",
                Department_Id = departmentId,
                Price = 10.0m
            };

            // Act
            var (isValid, errors) = ValidateModel(product);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Department_Id"));
        }

        [Theory]
        [InlineData(0, "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero")]
        [InlineData(-5.99, "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero")]
        public void Price_InvalidValues_ShouldFailValidation(decimal price, string expectedError)
        {
            // Arrange
            var product = new Product
            {
                Name = "منتج صالح",
                Department_Id = 1,
                Price = price
            };

            // Act
            var (isValid, errors) = ValidateModel(product);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Price"));
        }

        [Fact]
        public void Product_DefaultConstructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var product = new Product();

            // Assert
            Assert.Equal(string.Empty, product.Name);
            Assert.True(product.IsActive); // Default value
            Assert.NotNull(product.ProductUnits);
            Assert.Empty(product.ProductUnits);
        }

        [Fact]
        public void Product_MissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var product = new Product(); // All required fields missing

            // Act
            var (isValid, errors) = ValidateModel(product);

            // Debug output if test fails
            if (!isValid)
            {
                Console.WriteLine("Validation errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"{string.Join(",", error.MemberNames)}: {error.ErrorMessage}");
                }
            }

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == "اسم المنتج مطلوب | Product name is required");
            Assert.Contains(errors, e =>
                e.ErrorMessage == "معرف القسم غير صالح | Invalid department ID");
            Assert.Contains(errors, e =>
                e.ErrorMessage == "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero");
        }

        [Fact]
        public void Product_NavigationProperties_ShouldWorkCorrectly()
        {
            // Arrange
            var department = new Department { Id_Department = 1 };
            var productUnit = new Product_Unit();

            var product = new Product
            {
                Department = department,
                ProductUnits = new List<Product_Unit> { productUnit }
            };

            // Assert
            Assert.Same(department, product.Department);
            Assert.Contains(productUnit, product.ProductUnits);
        }
    }
}
