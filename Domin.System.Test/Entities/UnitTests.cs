using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.System.Entities;
using global::System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
namespace Domin.System.Test.Entities
{
    

    public class UnitTests
    {
        private (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        [Fact]
        public void Unit_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var unit = new Unit
            {
                Name = "كيلوغرام",
                Branch_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(unit);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }
        [Theory]
        [InlineData(null, "اسم الوحدة مطلوب | Unit name is required")]
        [InlineData("", "اسم الوحدة مطلوب | Unit name is required")]
        [InlineData("ل", "يجب أن يكون اسم الوحدة بين 2 و100 حرف | Unit name must be between 2 and 100 characters")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "يجب أن يكون اسم الوحدة بين 2 و100 حرف | Unit name must be between 2 and 100 characters")] // 101 chars
        public void Name_InvalidValues_ShouldFailValidation(string name, string expectedError)
        {
            // Test implementation remains the same
            var unit = new Unit
            {
                Name = name,
                Branch_Id = 1
            };

            var (isValid, errors) = ValidateModel(unit);
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Name"));
        }

        [Theory]
        [InlineData(0, "معرف الفرع غير صالح | Invalid branch ID")]
        [InlineData(-1, "معرف الفرع غير صالح | Invalid branch ID")]
        public void BranchId_InvalidValues_ShouldFailValidation(int branchId, string expectedError)
        {
            // Arrange
            var unit = new Unit
            {
                Name = "وحدة صالحة",
                Branch_Id = branchId
            };

            // Act
            var (isValid, errors) = ValidateModel(unit);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Branch_Id"));
        }

        [Fact]
        public void Unit_DefaultConstructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var unit = new Unit();

            // Assert
            Assert.Equal(string.Empty, unit.Name);
            Assert.NotNull(unit.ProductUnits);
            Assert.Empty(unit.ProductUnits);
        }

        [Fact]
        public void Unit_MissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var unit = new Unit(); // All required fields missing/default

            // Act
            var (isValid, errors) = ValidateModel(unit);

            // Debug output
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
                e.ErrorMessage == "اسم الوحدة مطلوب | Unit name is required");
            Assert.Contains(errors, e =>
                e.ErrorMessage == "معرف الفرع غير صالح | Invalid branch ID");
        }

        [Fact]
        public void Unit_NavigationProperties_ShouldWorkCorrectly()
        {
            // Arrange
            var branch = new Branch { Id_Branch = 1 };
            var productUnit = new Product_Unit();

            // Act
            var unit = new Unit
            {
                // Set both the navigation property AND foreign key
                Branch = branch,
                Branch_Id = branch.Id_Branch, // Explicitly set the foreign key
                ProductUnits = new List<Product_Unit> { productUnit }
            };

            // Assert
            Assert.Same(branch, unit.Branch);
            Assert.Contains(productUnit, unit.ProductUnits);
            Assert.Equal(1, unit.Branch_Id); // Now this will pass
        }
    }
}
