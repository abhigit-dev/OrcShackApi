using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using OrcShackApi.Core.Models;
using OrcShackApi.Data.Helper;
using OrcShackApi.Data.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrcShackApi.Data.UnitTests.Repository
{
    public class UserRepositoryTests
    {
        private UserRepository _userRepository;
        private Mock<OrcShackApiContext> _mockContext;
        private Mock<DbSet<User>> _mockUserSet;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<OrcShackApiContext>();
            _mockUserSet = new Mock<DbSet<User>>();
            _mockContext.Setup(x => x.Users).Returns(_mockUserSet.Object);
            _userRepository = new UserRepository(_mockContext.Object, Mock.Of<ILogger<UserRepository>>());
        }

        [Test]
        public async Task Authenticate_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            var passwordHash = new byte[128]; 
            var passwordSalt = new byte[128];
            CreatePasswordHash("password",out passwordHash,out passwordSalt);

            var user = new User { Email = "test@example.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt };
            var data = new List<User> { user };

            _mockContext.Setup(x => x.Users).ReturnsDbSet(data);

            // Act
            var result = await _userRepository.Authenticate(user.Email, "password"); 

            // Assert
            Assert.AreEqual(user, result);
        }


        [Test]
        public async Task GetById_WithValidId_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1 };
            _mockContext.Setup(x => x.Users.FindAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userRepository.GetById(1);

            // Assert
            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task Create_WithValidUser_CreatesUser()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var data = new List<User>();

            _mockContext.Setup(x => x.Users).ReturnsDbSet(data);

            // Act
            var result = await _userRepository.Create(user, "password");

            // Assert
            _mockContext.Verify(x => x.Users.AddAsync(user,default), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }


        [Test]
        public async Task Update_WithValidUser_UpdatesUser()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            _mockContext.Setup(x => x.Users.FindAsync(1)).ReturnsAsync(user);

            // Act
            await _userRepository.Update(user);

            // Assert
            _mockContext.Verify(x => x.Users.Update(user), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task Delete_WithValidId_DeletesUser()
        {
            // Arrange
            var user = new User { Id = 1 };
            _mockContext.Setup(x => x.Users.FindAsync(1)).ReturnsAsync(user);

            // Act
            await _userRepository.Delete(1);

            // Assert
            _mockContext.Verify(x => x.Users.Remove(user), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task UpdatePassword_WithValidCredentials_UpdatesPassword()
        {
            // Arrange
            var passwordHash = new byte[128];
            var passwordSalt = new byte[128];
            CreatePasswordHash("oldPassword", out passwordHash, out passwordSalt);

            var user = new User { Email = "test@example.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt };
            var data = new List<User> { user };
            _mockContext.Setup(x => x.Users).ReturnsDbSet(data);
             
            // Act
            var result = await _userRepository.UpdatePassword(user.Email, "oldPassword", "newPassword");

            // Assert
            Assert.IsTrue(result);
            _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }
 
        [Test]
        public async Task Authenticate_WrongPassword_IncrementsFailedLoginAttemptCount()
        {
            // Arrange
            var passwordHash = new byte[128];
            var passwordSalt = new byte[128];
            CreatePasswordHash("oldPassword", out passwordHash, out passwordSalt);

            var user = new User { Email = "test@example.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt, FailedLoginAttemptCount=4 };
            var data = new List<User> { user };
            _mockContext.Setup(x => x.Users).ReturnsDbSet(data);

            // Act
            Assert.ThrowsAsync<AccountLockedException>(() =>  _userRepository.Authenticate("test@example.com", "wrongpassword"));

            // Assert
            Assert.AreEqual(5, user.FailedLoginAttemptCount);
        }

        [Test]
        public async Task Authenticate_TooManyFailedAttempts_LocksAccount()
        {
            // Arrange
            var passwordHash = new byte[128];
            var passwordSalt = new byte[128];
            CreatePasswordHash("oldPassword", out passwordHash, out passwordSalt);

            var user = new User { Email = "test@example.com", PasswordHash = passwordHash, PasswordSalt = passwordSalt, FailedLoginAttemptCount = 4 };
            var data = new List<User> { user };
            _mockContext.Setup(x => x.Users).ReturnsDbSet(data);

            // Act
            Assert.ThrowsAsync<AccountLockedException>(() => _userRepository.Authenticate("test@example.com", "wrongpassword"));


            // Assert
            Assert.IsNotNull(user.AccountLockedUntil);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

    }
}