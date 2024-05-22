using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Web.Controllers;
using OrcShackApi.Web.Jwt;
using OrcShackApi.Web.Services;

namespace OrcShackApi.Web.UnitTests.Controllers
{
    public class UsersControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<ITokenService> _mockTokenService;
        private Mock<IValidator<UserDto>> _mockUserDtoValidator;
        private Mock<IValidator<PasswordUpdateDto>> _mockPasswordUpdateDtoValidator;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<UsersController>> _mockLogger;
        private UsersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockUserDtoValidator = new Mock<IValidator<UserDto>>();
            _mockPasswordUpdateDtoValidator = new Mock<IValidator<PasswordUpdateDto>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UsersController>>();

            _controller = new UsersController(
                _mockUserService.Object,
                _mockTokenService.Object,
                _mockMapper.Object,
                _mockUserDtoValidator.Object,
                _mockPasswordUpdateDtoValidator.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task Register_WithValidUserDto_ReturnsOk()
        {
            // Arrange
            var userDto = new UserDto { Email = "test@test.com", Password = "password" };
            _mockUserDtoValidator.Setup(v => v.ValidateAsync(userDto, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task Authenticate_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var authenticateDto = new AuthenticateDto { Email = "test@test.com", Password = "password" };
            _mockUserService.Setup(s => s.Authenticate(authenticateDto.Email, authenticateDto.Password)).ReturnsAsync(new User());

            // Act
            var result = await _controller.Authenticate(authenticateDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAll_WhenCalled_ReturnsOk()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetAll()).ReturnsAsync(new List<User>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Get_WithExistingId_ReturnsOk()
        {
            // Arrange
            var id = 1;
            _mockUserService.Setup(s => s.GetById(id)).ReturnsAsync(new User());

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Update_WithValidUser_ReturnsNoContent()
        {
            // Arrange
            var user = new User { Id = 1 };
            _mockUserService.Setup(s => s.Update(user, "password1")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(user.Id, user);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdatePassword_WithValidCredentials_ReturnsNoContent()
        {
            // Arrange
            var email = "test@test.com";
            var passwordUpdateDto = new PasswordUpdateDto { OldPassword = "oldpassword", NewPassword = "newpassword" };
            _mockUserService.Setup(s => s.UpdatePassword(email, passwordUpdateDto.OldPassword, passwordUpdateDto.NewPassword)).ReturnsAsync(true);
            _mockPasswordUpdateDtoValidator.Setup(v => v.ValidateAsync(It.IsAny<PasswordUpdateDto>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());


            // Act
            var result = await _controller.UpdatePassword(email, passwordUpdateDto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_WithExistingId_ReturnsNoContent()
        {
            // Arrange
            var id = 1;
            _mockUserService.Setup(s => s.Delete(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
