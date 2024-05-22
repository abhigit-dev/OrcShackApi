using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Web.Jwt;

namespace OrcShackApi.Web.UnitTests.Jwt
{
    public class TokenServiceTests
    {
        private Mock<IOptions<JwtSettings>> _mockJwtSettings;
        private Mock<ILogger<TokenService>> _mockLogger;
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var jwtSettings = new JwtSettings
            {
                SecretKey = "ThisIsASecretKey1234567890####@!@!@!@!@",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };
            _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
            _mockJwtSettings.Setup(s => s.Value).Returns(jwtSettings);
            _mockLogger = new Mock<ILogger<TokenService>>();
            _tokenService = new TokenService(_mockJwtSettings.Object, _mockLogger.Object);
        }

        [Test]
        public void GenerateToken_WithValidUser_ReturnsToken()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Test User", Email = "test@gmamil.com"};

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsInstanceOf<string>(token);
        }
    }
}
