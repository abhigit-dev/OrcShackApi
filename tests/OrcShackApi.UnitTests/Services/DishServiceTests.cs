using Microsoft.Extensions.Logging;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Data.Repository;
using OrcShackApi.Web.Services;

namespace OrcShackApi.Web.UnitTests.Services
{
    public class DishServiceTests
    {
        private Mock<IDishRepository> _mockDishRepository;
        private Mock<ILogger<DishService>> _mockLogger;
        private DishService _dishService;

        [SetUp]
        public void Setup()
        {
            _mockDishRepository = new Mock<IDishRepository>();
            _mockLogger = new Mock<ILogger<DishService>>();

            _dishService = new DishService(_mockDishRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllDishes_WhenCalled_ReturnsAllDishes()
        {
            // Arrange
            var dishes = new List<Dish> { new Dish(), new Dish() };
            _mockDishRepository.Setup(r => r.GetAllDishes()).ReturnsAsync(dishes);

            // Act
            var result = await _dishService.GetAllDishes();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetDishById_WithExistingId_ReturnsDish()
        {
            // Arrange
            var dish = new Dish { Id = 1 };
            _mockDishRepository.Setup(r => r.GetDishById(dish.Id)).ReturnsAsync(dish);

            // Act
            var result = await _dishService.GetDishById(dish.Id);

            // Assert
            Assert.AreEqual(dish, result);
        }

        [Test]
        public async Task CreateDish_WithValidDish_ReturnsDish()
        {
            // Arrange
            var dish = new Dish { Id = 1 };
            _mockDishRepository.Setup(r => r.CreateDish(dish)).ReturnsAsync(dish);

            // Act
            var result = await _dishService.CreateDish(dish);

            // Assert
            Assert.AreEqual(dish, result);
        }

        [Test]
        public async Task UpdateDish_WithValidDish_ReturnsNoException()
        {
            // Arrange
            var dish = new Dish { Id = 1 };
            _mockDishRepository.Setup(r => r.UpdateDish(dish)).Returns(Task.CompletedTask);

            // Act
            await _dishService.UpdateDish(dish);

            // Assert
            _mockDishRepository.Verify(r => r.UpdateDish(dish), Times.Once);
        }

        [Test]
        public async Task DeleteDish_WithExistingId_ReturnsNoException()
        {
            // Arrange
            var id = 1;
            _mockDishRepository.Setup(r => r.DeleteDish(id)).Returns(Task.CompletedTask);

            // Act
            await _dishService.DeleteDish(id);

            // Assert
            _mockDishRepository.Verify(r => r.DeleteDish(id), Times.Once);
        }
    }
}

