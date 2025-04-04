using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Domin.System.Test.Entities
{
    
    public class BranchTests
    {
        private static (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        [Fact]
        public void Branch_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var branch = new Branch
            {
                Name = "الفرع الرئيسي بالرياض",
                Address = "شارع الملك عبدالعزيز، الرياض 12345",
                City = "الرياض",
                Phone = "0555555555",
                IsActive = true
            };

            // Act
            var (isValid, errors) = ValidateModel(branch);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null, "اسم الفرع مطلوب | Branch name is required")]
        [InlineData("", "اسم الفرع مطلوب | Branch name is required")]
        [InlineData("فرع", "يجب أن يكون اسم الفرع بين 5 و100 حرف | Branch name must be between 5 and 100 characters")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "يجب أن يكون اسم الفرع بين 5 و100 حرف | Branch name must be between 5 and 100 characters")] // 101 chars
        public void Name_ValidationTests(string name, string expectedError)
        {
            // Arrange
            var branch = new Branch
            {
                Name = name,
                Address = "عنوان صالح",
                City = "مدينة صالحة",
                Phone = "0512345678"
            };

            // Act
            var (isValid, errors) = ValidateModel(branch);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "عنوان الفرع مطلوب | Branch address is required")]
        [InlineData("", "عنوان الفرع مطلوب | Branch address is required")]
        [InlineData("   ", "عنوان الفرع مطلوب | Branch address is required")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "يجب ألا يتجاوز العنوان 200 حرف | Address must not exceed 200 characters")] // 201 chars
        public void Address_ValidationTests(string address, string expectedError)
        {
            // Arrange
            var branch = new Branch
            {
                Name = "فرع صالح",
                Address = address,
                City = "مدينة صالحة",
                Phone = "0512345678"
            };

            // Act
            var (isValid, errors) = ValidateModel(branch);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "مدينة الفرع مطلوبة | Branch city is required")]
        [InlineData("", "مدينة الفرع مطلوبة | Branch city is required")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "يجب ألا تتجاوز المدينة 50 حرف | City must not exceed 50 characters")] // 51 chars
        public void City_ValidationTests(string city, string expectedError)
        {
            // Arrange
            var branch = new Branch
            {
                Name = "فرع صالح",
                Address = "عنوان صالح",
                City = city,
                Phone = "0512345678"
            };

            // Act
            var (isValid, errors) = ValidateModel(branch);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "هاتف الفرع مطلوب | Branch phone is required")]
        [InlineData("", "هاتف الفرع مطلوب | Branch phone is required")]
        [InlineData("123", "يجب أن يتكون الهاتف من أرقام فقط وبين 10-15 رقم | Phone must contain 10-15 digits only")]
        [InlineData("0512345678901234", "يجب أن يتكون الهاتف من أرقام فقط وبين 10-15 رقم | Phone must contain 10-15 digits only")] // 16 digits
        [InlineData("05xxxxxxx", "يجب أن يتكون الهاتف من أرقام فقط وبين 10-15 رقم | Phone must contain 10-15 digits only")]
        public void Phone_ValidationTests(string phone, string expectedError)
        {
            // Arrange
            var branch = new Branch
            {
                Name = "فرع صالح",
                Address = "عنوان صالح",
                City = "مدينة صالحة",
                Phone = phone
            };

            // Act
            var (isValid, errors) = ValidateModel(branch);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }

        [Fact]
        public void Branch_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var branch = new Branch();

            // Assert
            Assert.Equal(string.Empty, branch.Name);
            Assert.Equal(string.Empty, branch.Address);
            Assert.Equal(string.Empty, branch.City);
            Assert.Equal(string.Empty, branch.Phone);
            Assert.True(branch.IsActive); // Default value
        }

        [Fact]
        public void Branch_MissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var branch = new Branch(); // All required fields missing

            // Act
            var (isValid, errors) = ValidateModel(branch);

            // Debug output if test fails
            if (!isValid)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine($"{string.Join(",", error.MemberNames)}: {error.ErrorMessage}");
                }
            }

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.MemberNames.Contains("Name"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Address"));
            Assert.Contains(errors, e => e.MemberNames.Contains("City"));
            Assert.Contains(errors, e => e.MemberNames.Contains("Phone"));
        }
    }
}
