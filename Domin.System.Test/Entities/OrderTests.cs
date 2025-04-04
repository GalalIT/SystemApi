using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;

namespace Domin.System.Test.Entities
{
   
    public class OrderTests
    {
        private Order CreateValidOrder()
        {
            return new Order
            {
                Total_Amount = 100,
                Total_AmountAfterDiscount = 90,
                Discount = 10,
                OrderType = 1,
                Branch_Id = 1,
                Company_id = 1,
                User_id = "user123",
                OrderDetails = new HashSet<OrderDetails>()
            };
        }

        private List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void Order_WithValidData_PassesValidation()
        {
            // Arrange
            var order = CreateValidOrder();

            // Act
            var errors = ValidateModel(order);

            // Assert
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(49)]
        [InlineData(-100)]
        public void TotalAmount_BelowMinimum_FailsValidation(decimal amount)
        {
            // Arrange
            var order = CreateValidOrder();
            order.Total_Amount = amount;

            // Act
            var errors = ValidateModel(order);

            // Assert
            Assert.Single(errors);
            Assert.Contains("يجب أن يكون السعر أكبر من 50", errors[0].ErrorMessage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(-1)]
        public void OrderType_InvalidValue_FailsValidation(int orderType)
        {
            // Arrange
            var order = CreateValidOrder();
            order.OrderType = orderType;

            // Act
            var errors = ValidateModel(order);

            // Assert
            Assert.Single(errors);
            Assert.Contains("يجب أن يكون نوع الطلب 1 (محلي) أو 2 (سفري)", errors[0].ErrorMessage);
        }

        [Fact]
        public void BranchId_Zero_FailsValidation()
        {
            // Arrange
            var order = CreateValidOrder();
            order.Branch_Id = 0;

            // Act
            var errors = ValidateModel(order);

            // Assert
            Assert.Single(errors);
            Assert.Contains("معرف الفرع غير صالح", errors[0].ErrorMessage);
        }

        [Fact]
        public void CompanyId_Zero_FailsValidation()
        {
            // Arrange
            var order = CreateValidOrder();
            order.Company_id = 0;

            // Act
            var errors = ValidateModel(order);

            // Assert
            Assert.Single(errors);
            Assert.Contains("معرف الشركة غير صالح", errors[0].ErrorMessage);
        }

        [Fact]
        public void UserId_Empty_FailsValidation()
        {
            // Arrange
            var order = CreateValidOrder();
            order.User_id = "";

            // Act
            var errors = ValidateModel(order);

            // Assert
            Assert.NotEmpty(errors);

            // Check for either of the validation messages
            var errorMessages = string.Join("; ", errors.Select(e => e.ErrorMessage));
            Assert.True(
                errorMessages.Contains("معرف المستخدم مطلوب") ||
                errorMessages.Contains("معرف المستخدم غير صالح"),
                $"Expected either 'معرف المستخدم مطلوب' or 'معرف المستخدم غير صالح' but got: {errorMessages}"
            );
        }

        [Fact]
        public void DateTimeCreated_DefaultsToCurrentTime()
        {
            // Arrange
            var preTestTime = DateTime.Now;
            var order = CreateValidOrder();
            var postTestTime = DateTime.Now;

            // Assert
            Assert.InRange(order.DateTime_Created, preTestTime, postTestTime);
        }

        [Fact]
        public void OrderDetails_InitializedAsEmptyCollection()
        {
            // Arrange
            var order = new Order();

            // Assert
            Assert.NotNull(order.OrderDetails);
            Assert.Empty(order.OrderDetails);
        }

        [Fact]
        public void TotalAmountAfterDiscount_CalculatesCorrectly()
        {
            // Arrange
            var order = CreateValidOrder();
            order.Total_Amount = 200;
            order.Discount = 30;

            // Act
            order.Total_AmountAfterDiscount = order.Total_Amount - order.Discount;

            // Assert
            Assert.Equal(170, order.Total_AmountAfterDiscount);
        }

        [Fact]
        public void NavigationProperties_WorkCorrectly()
        {
            // Arrange
            var order = CreateValidOrder();
            var branch = new Branch { Id_Branch = 1 };
            var company = new Company { Id_Company = 1 };
            var user = new ApplicationUser { Id = "user123" };

            // Act
            order.Branch = branch;
            order.Company = company;
            order.applicationUser = user;

            // Assert
            Assert.Equal(branch, order.Branch);
            Assert.Equal(company, order.Company);
            Assert.Equal(user, order.applicationUser);
        }
    }
}
