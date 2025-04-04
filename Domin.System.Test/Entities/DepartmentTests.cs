using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Domin.System.Test.Entities
{
    
    public class DepartmentTests
    {
        private static (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        [Fact]
        public void Department_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var department = new Department
            {
                Name = "قسم المبيعات",
                Branch_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(department);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null, "اسم القسم مطلوب | Department name is required")]
        [InlineData("", "اسم القسم مطلوب | Department name is required")]
        [InlineData("ق", "يجب أن يكون اسم القسم بين 2 و100 حرف | Department name must be between 2 and 100 characters")]
        public void Name_ValidationTests(string name, string expectedError)
        {
            // Arrange
            var department = new Department
            {
                Name = name,
                Branch_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(department);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }

        [Fact]
        public void Description_ExceedsMaxLength_ShouldFailValidation()
        {
            // Arrange
            var longDescription = new string('a', 501);
            var department = new Department
            {
                Name = "قسم صالح",
                Branch_Id = 1,
                Description = longDescription
            };

            // Act
            var (isValid, errors) = ValidateModel(department);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage.Contains("يجب ألا يتجاوز الوصف 500 حرف"));
        }

        [Theory]
        [InlineData(0, "معرف الفرع غير صالح | Invalid branch ID")]
        [InlineData(-1, "معرف الفرع غير صالح | Invalid branch ID")]
        public void BranchId_ValidationTests(int branchId, string expectedError)
        {
            // Arrange
            var department = new Department
            {
                Name = "قسم صالح",
                Branch_Id = branchId
            };

            // Act
            var (isValid, errors) = ValidateModel(department);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e => e.ErrorMessage == expectedError);
        }
    }
}
