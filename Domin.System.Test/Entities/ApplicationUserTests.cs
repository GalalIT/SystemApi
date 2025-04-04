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
    
    public class ApplicationUserTests
    {
        private (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        [Fact]
        public void ApplicationUser_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "test.user",
                Email = "test@example.com",
                Name = "محمد أحمد",
                Branch_Id = 1,
                IsActive = true
            };

            // Act
            var (isValid, errors) = ValidateModel(user);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(null, "اسم المستخدم مطلوب | User name is required")]
        [InlineData("", "اسم المستخدم مطلوب | User name is required")]
        [InlineData("م", "يجب أن يكون اسم المستخدم بين 2 و100 حرف | User name must be between 2 and 100 characters")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            "يجب أن يكون اسم المستخدم بين 2 و100 حرف | User name must be between 2 and 100 characters")] // 101 chars
        public void Name_ValidationTests(string name, string expectedError)
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "test.user",
                Email = "test@example.com",
                Name = name,
                Branch_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(user);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Name"));
        }

        [Theory]
        [InlineData(0, "معرف الفرع غير صالح | Invalid branch ID")]
        [InlineData(-5, "معرف الفرع غير صالح | Invalid branch ID")]
        public void BranchId_ValidationTests(int branchId, string expectedError)
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "test.user",
                Email = "test@example.com",
                Name = "اسم صالح",
                Branch_Id = branchId
            };

            // Act
            var (isValid, errors) = ValidateModel(user);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Branch_Id"));
        }

        [Fact]
        public void ApplicationUser_DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var user = new ApplicationUser();

            // Assert
            Assert.Equal(string.Empty, user.Name);
            Assert.True(user.IsActive); // Default value
            Assert.Null(user.ProfilePicture);
        }

        [Fact]
        public void ApplicationUser_NavigationProperty_ShouldWork()
        {
            // Arrange
            var branch = new Branch { Id_Branch = 1 };

            // Act
            var user = new ApplicationUser
            {
                Branch = branch,
                Branch_Id = branch.Id_Branch // Must set both in unit tests
            };

            // Assert
            Assert.Same(branch, user.Branch);
            Assert.Equal(branch.Id_Branch, user.Branch_Id);
        }

        
    }
}
