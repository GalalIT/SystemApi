using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.System.Utility;
using System.Threading.Tasks;
using Xunit;

namespace Application.System.Tests.Utility
{
    public class ResponseTests
    {
        #region Response (Non-Generic) Tests

        [Fact]
        public void Response_Success_ShouldCreateSuccessfulResponse()
        {
            // Act
            var response = Response.Success("Custom success message", "201");

            // Assert
            Assert.True(response.Succeeded);
            Assert.Equal("Custom success message", response.Message);
            Assert.Equal("201", response.Status);
        }

        [Fact]
        public void Response_Success_ShouldUseDefaultValues()
        {
            // Act
            var response = Response.Success();

            // Assert
            Assert.True(response.Succeeded);
            Assert.Equal("Operation succeeded", response.Message);
            Assert.Equal("200", response.Status);
        }

        [Fact]
        public void Response_Failure_ShouldCreateFailedResponse()
        {
            // Act
            var response = Response.Failure("Custom error message", "500");

            // Assert
            Assert.False(response.Succeeded);
            Assert.Equal("Custom error message", response.Message);
            Assert.Equal("500", response.Status);
        }

        [Fact]
        public void Response_Failure_ShouldUseDefaultValues()
        {
            // Act
            var response = Response.Failure();

            // Assert
            Assert.False(response.Succeeded);
            Assert.Equal("Operation failed", response.Message);
            Assert.Equal("400", response.Status);
        }

        #endregion

        #region Response<T> (Generic) Tests

        [Fact]
        public void GenericResponse_Success_ShouldCreateSuccessfulResponseWithData()
        {
            // Arrange
            var testData = new { Id = 1, Name = "Test" };

            // Act
            var response = Response<object>.Success(testData, "Data retrieved", "200");

            // Assert
            Assert.True(response.Succeeded);
            Assert.Equal(testData, response.Data);
            Assert.Equal("Data retrieved", response.Message);
            Assert.Equal("200", response.Status);
        }

        [Fact]
        public void GenericResponse_Success_ShouldUseDefaultValues()
        {
            // Arrange
            var testData = 42;

            // Act
            var response = Response<int>.Success(testData);

            // Assert
            Assert.True(response.Succeeded);
            Assert.Equal(42, response.Data);
            Assert.Equal("Operation succeeded", response.Message);
            Assert.Equal("200", response.Status);
        }

        [Fact]
        public async Task GenericResponse_SuccessAsync_ShouldCreateSuccessfulAsyncResponse()
        {
            // Arrange
            var testData = "Async result";

            // Act
            var response = await Response<string>.SuccessAsync(testData, "Async success", "202");

            // Assert
            Assert.True(response.Succeeded);
            Assert.Equal("Async result", response.Data);
            Assert.Equal("Async success", response.Message);
            Assert.Equal("202", response.Status);
        }

        [Fact]
        public void GenericResponse_Failure_ShouldCreateFailedResponseWithoutData()
        {
            // Act
            var response = Response<string>.Failure("Not found", "404");

            // Assert
            Assert.False(response.Succeeded);
            Assert.Null(response.Data);
            Assert.Equal("Not found", response.Message);
            Assert.Equal("404", response.Status);
        }

        [Fact]
        public void GenericResponse_Failure_ShouldUseDefaultValues()
        {
            // Act
            var response = Response<double>.Failure();

            // Assert
            Assert.False(response.Succeeded);
            Assert.Equal(0, response.Data); // default for double
            Assert.Equal("Operation failed", response.Message);
            Assert.Equal("400", response.Status);
        }

        [Fact]
        public async Task GenericResponse_FailureAsync_ShouldCreateFailedAsyncResponse()
        {
            // Act
            var response = await Response<bool>.FailureAsync("Async failure", "500");

            // Assert
            Assert.False(response.Succeeded);
            Assert.False(response.Data); // default for bool
            Assert.Equal("Async failure", response.Message);
            Assert.Equal("500", response.Status);
        }

        #endregion

        #region Interface Implementation Tests

        [Fact]
        public void Response_ShouldImplementIResponse()
        {
            // Arrange
            var response = Response.Success();

            // Assert
            Assert.IsAssignableFrom<IResponse>(response);
        }

        [Fact]
        public void GenericResponse_ShouldImplementIResponseT()
        {
            // Arrange
            var response = Response<int>.Success(123);

            // Assert
            Assert.IsAssignableFrom<IResponse<int>>(response);
        }

        [Fact]
        public void GenericResponse_ShouldAlsoImplementIResponse()
        {
            // Arrange
            var response = Response<string>.Success("test");

            // Assert
            Assert.IsAssignableFrom<IResponse>(response);
        }

        #endregion
    }
}
