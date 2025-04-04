using Domin.System.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.System.Test.Entities
{
    public class OrderDetailsTests
    {
        private (bool IsValid, List<ValidationResult> Errors) ValidateModel(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return (isValid, results);
        }

        [Fact]
        public void OrderDetails_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                Order_Id = 1,
                Description_product = "Special instructions"
            };

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errors);
        }

        [Theory]
        [InlineData(0, "يجب أن تكون الكمية أكبر من صفر | Quantity must be greater than zero")]
        [InlineData(-5, "يجب أن تكون الكمية أكبر من صفر | Quantity must be greater than zero")]
        public void Quantity_InvalidValues_ShouldFailValidation(int quantity, string expectedError)
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = quantity,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Quantity"));
        }

        [Theory]
        [InlineData(0, "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero")]
        [InlineData(-10.50, "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero")]
        public void TotalPrice_InvalidValues_ShouldFailValidation(decimal price, string expectedError)
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = 2,
                Total_Price = price,
                Product_Unit_id = 1,
                Order_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Total_Price"));
        }

        [Fact]
        public void Description_ExceedsMaxLength_ShouldFailValidation()
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                Order_Id = 1,
                Description_product = new string('a', 501) // 501 characters
            };

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == "يجب ألا تتجاوز الملاحظة 500 حرف | Notes must not exceed 500 characters" &&
                e.MemberNames.Contains("Description_product"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Valid description")]
        public void Description_ValidValues_ShouldPassValidation(string description)
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                Order_Id = 1,
                Description_product = description
            };

            // Act
            var (isValid, _) = ValidateModel(orderDetail);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData(0, "معرف وحدة المنتج غير صالح | Invalid product unit ID")]
        [InlineData(-1, "معرف وحدة المنتج غير صالح | Invalid product unit ID")]
        public void ProductUnitId_InvalidValues_ShouldFailValidation(int productUnitId, string expectedError)
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = productUnitId,
                Order_Id = 1
            };

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Product_Unit_id"));
        }

        [Theory]
        [InlineData(0, "معرف الطلب غير صالح | Invalid order ID")]
        [InlineData(-5, "معرف الطلب غير صالح | Invalid order ID")]
        public void OrderId_InvalidValues_ShouldFailValidation(int orderId, string expectedError)
        {
            // Arrange
            var orderDetail = new OrderDetails
            {
                Quantity = 2,
                Total_Price = 100.50m,
                Product_Unit_id = 1,
                Order_Id = orderId
            };

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == expectedError &&
                e.MemberNames.Contains("Order_Id"));
        }

        [Fact]
        public void OrderDetails_MissingRequiredFields_ShouldFailValidation()
        {
            // Arrange
            var orderDetail = new OrderDetails();

            // Act
            var (isValid, errors) = ValidateModel(orderDetail);

            // Assert
            Assert.False(isValid);
            Assert.Contains(errors, e =>
                e.ErrorMessage == "يجب أن تكون الكمية أكبر من صفر | Quantity must be greater than zero");
            Assert.Contains(errors, e =>
                e.ErrorMessage == "يجب أن يكون السعر أكبر من صفر | Price must be greater than zero");
            Assert.Contains(errors, e =>
                e.ErrorMessage == "معرف وحدة المنتج غير صالح | Invalid product unit ID");
            Assert.Contains(errors, e =>
                e.ErrorMessage == "معرف الطلب غير صالح | Invalid order ID");
        }

        [Fact]
        public void OrderDetails_NavigationProperties_ShouldBeNullWhenNotSet()
        {
            // Arrange & Act
            var orderDetail = new OrderDetails();

            // Assert
            Assert.Null(orderDetail.product_Unit);
            Assert.Null(orderDetail.Order);
        }
    }
}
