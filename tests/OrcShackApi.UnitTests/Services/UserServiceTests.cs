using Microsoft.Extensions.Logging;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Data.Repository;
using OrcShackApi.Web.Services;

namespace OrcShackApi.Web.UnitTests.Services
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<UserService>> _mockLogger;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAll_WhenCalled_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User> { new User(), new User() };
            _mockUserRepository.Setup(r => r.GetAll()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
            _mockUserRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [Test]
        public async Task GetById_WithExistingId_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1 };
            _mockUserRepository.Setup(r => r.GetById(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetById(user.Id);

            // Assert
            Assert.AreEqual(user, result);
            _mockUserRepository.Verify(r => r.GetById(user.Id), Times.Once);
        }

        [Test]
        public async Task Create_WithValidUser_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1 };
            var password = "password";
            _mockUserRepository.Setup(r => r.Create(user, password)).ReturnsAsync(user);

            // Act
            var result = await _userService.Create(user, password);

            // Assert
            Assert.AreEqual(user, result);
            _mockUserRepository.Verify(r => r.Create(user, password), Times.Once);
        }

        [Test]
        public async Task Update_WithValidUser_CallsUpdateOnRepository()
        {
            // Arrange
            var user = new User { Id = 1 };
            var password = "password";
            _mockUserRepository.Setup(r => r.Update(user, password)).Returns(Task.CompletedTask);

            // Act
            await _userService.Update(user, password);

            // Assert
            _mockUserRepository.Verify(r => r.Update(user, password), Times.Once);
        }

        [Test]
        public async Task Delete_WithExistingId_CallsDeleteOnRepository()
        {
            // Arrange
            var id = 1;
            _mockUserRepository.Setup(r => r.Delete(id)).Returns(Task.CompletedTask);

            // Act
            await _userService.Delete(id);

            // Assert
            _mockUserRepository.Verify(r => r.Delete(id), Times.Once);
        }
    }
}
