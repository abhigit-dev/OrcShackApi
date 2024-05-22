using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Web.Controllers;
using OrcShackApi.Web.Services;

namespace OrcShackApi.Web.UnitTests.Controllers
{
    [TestFixture]
    public class DishesControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IDishService> _mockDishService;
        private Mock<ILogger<DishesController>> _mockLogger;
        private Mock<IMapper> _mockMapper;
        private DishesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDishService = new Mock<IDishService>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<DishesController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new DishesController(_mockDishService.Object, _mockUserService.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult()
        {
            // Arrange
            var dishes = new List<Dish> { new Dish(), new Dish() };
            _mockDishService.Setup(service => service.GetAllDishes()).ReturnsAsync(dishes);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<List<Dish>>(okResult.Value);
            Assert.AreEqual(2, ((List<Dish>)okResult.Value).Count);
        }

        [Test]
        public async Task Get_ReturnsOkResult()
        {
            // Arrange
            var dish = new Dish();
            _mockDishService.Setup(service => service.GetDishById(It.IsAny<int>())).ReturnsAsync(dish);

            // Act
            var result = await _controller.Get(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOf<Dish>(okResult.Value);
        }

        [Test]
        public async Task CreateDish_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var dishDto = new DishDto();
            var dish = new Dish();
            _mockMapper.Setup(mapper => mapper.Map<Dish>(It.IsAny<DishDto>())).Returns(dish);
            _mockDishService.Setup(service => service.CreateDish(It.IsAny<Dish>())).ReturnsAsync(dish);

            // Act
            var result = await _controller.CreateDish(dishDto);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task Update_ReturnsNoContentResult()
        {
            // Arrange
            var dishDto = new DishDto() { Description = "Dish Description", Name = "Dish 2", Price = 12 };
            var dish = new Dish() { Id = 1, Description = "Dish 1", Name = "Dish 1", Price = 10 };
            _mockDishService.Setup(service => service.GetDishById(It.IsAny<int>())).ReturnsAsync(dish);
            _mockDishService.Setup(service => service.UpdateDish(It.IsAny<Dish>())).Returns(Task.CompletedTask);
            // Act
            var result = await _controller.Update(1, dishDto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_ValidId_DeletesDishAndReturnsNoContent()
        {
            // Arrange
            var dish = new Dish { Id = 1, Image = "path/to/image.jpg" };
            _mockDishService.Setup(s => s.GetDishById(1)).ReturnsAsync(dish);
            _mockDishService.Setup(s => s.DeleteDish(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            _mockDishService.Verify(s => s.DeleteDish(1), Times.Once);
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsInternalServerError()
        {
            // Arrange
            _mockDishService.Setup(s => s.GetDishById(1)).Throws<Exception>();

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.That(result, Is.TypeOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
