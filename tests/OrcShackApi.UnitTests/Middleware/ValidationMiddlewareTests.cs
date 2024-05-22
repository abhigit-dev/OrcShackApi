using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Web.Middleware;

namespace OrcShackApi.Web.UnitTests.Middleware
{
    public class ValidationMiddlewareTests
    {
        private Mock<RequestDelegate> _mockNext;
        private ValidationMiddleware _middleware;

        [SetUp]
        public void Setup()
        {
            _mockNext = new Mock<RequestDelegate>();
            _middleware = new ValidationMiddleware(_mockNext.Object);
        }

        [Test]
        public async Task InvokeAsync_WithInvalidUserDto_ReturnsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/users/register";
            context.Request.Method = "POST";
            var userDto = new UserDto { Email = "invalid", Password = "password" }; // Invalid email
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userDto)));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.AreEqual(400, context.Response.StatusCode);
        }

        [Test]
        public async Task InvokeAsync_WithValidUserDto_CallsNextDelegate()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/users/register";
            context.Request.Method = "POST";
            var userDto = new UserDto { Email = "valid@test.com", Password = "password", Name = "Name" };
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userDto)));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }

        [Test]
        public async Task InvokeAsync_WithInvalidPasswordUpdateDto_ReturnsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/users/updatePassword";
            context.Request.Method = "PUT";
            var passwordUpdateDto = new PasswordUpdateDto { OldPassword = "oldpassword", NewPassword = "new" }; // Invalid new password
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(passwordUpdateDto)));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.AreEqual(400, context.Response.StatusCode);
        }

        [Test]
        public async Task InvokeAsync_WithValidPasswordUpdateDto_CallsNextDelegate()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/users/updatePassword";
            context.Request.Method = "PUT";
            var passwordUpdateDto = new PasswordUpdateDto { OldPassword = "oldpassword", NewPassword = "newpassword" };
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(passwordUpdateDto)));

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(next => next(context), Times.Once);
        }
    }
}