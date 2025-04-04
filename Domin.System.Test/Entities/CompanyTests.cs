using Domin.System.Entities;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Domin.System.Test.Entities
{
    
    public class CompanyTests
    {
        // Fixed ValidateModel helper with explicit return type
        private static (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // For IValidatableObject implementations
            if (model is IValidatableObject validatableModel)
            {
                results.AddRange(validatableModel.Validate(context));
                isValid = results.Count == 0;
            }

            return (IsValid: isValid, Errors: results);
        }

        [Fact]
        public void Company_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var company = new Company
            {
                Name = "شركة تجارية",
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddYears(1),
                DiscountRate = 10
            };

            // Act - Explicit tuple deconstruction
            (bool isValid, List<ValidationResult> errors) = ValidateModel(company);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null, "اسم الشركة مطلوب | Company name is required")]
        [InlineData("", "اسم الشركة مطلوب | Company name is required")]
        [InlineData("ش", "يجب أن يكون اسم الشركة بين 2 و100 حرف | Company name must be between 2 and 100 characters")]
        public void Name_ValidationTests(string name, string expectedError)
        {
            // Arrange
            var company = new Company
            {
                Name = name,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddYears(1),
                DiscountRate = 10
            };

            // Act
            var (isValid, errors) = ValidateModel(company);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }

        [Fact]
        public void DateRange_Validation_ShouldFail_When_ToDateBeforeFromDate()
        {
            // Arrange
            var company = new Company
            {
                Name = "شركة صالحة",
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddDays(-1), // Invalid
                DiscountRate = 10
            };

            // Act
            var (isValid, errors) = ValidateModel(company);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage.Contains("يجب أن يكون تاريخ الانتهاء بعد تاريخ البداية") &&
                e.MemberNames.Contains(nameof(Company.ToDate)));
        }

        [Theory]
        [InlineData(0, "يجب أن تكون نسبة الخصم بين 1 و50 بالمئة | Discount rate must be between 1 and 50 percent")]
        [InlineData(51, "يجب أن تكون نسبة الخصم بين 1 و50 بالمئة | Discount rate must be between 1 and 50 percent")]
        public void DiscountRate_ValidationTests(int rate, string expectedError)
        {
            // Arrange
            var company = new Company
            {
                Name = "شركة صالحة",
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddYears(1),
                DiscountRate = rate
            };

            // Act
            var (isValid, errors) = ValidateModel(company);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }
    }
}