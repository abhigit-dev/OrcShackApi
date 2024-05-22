using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrcShackApi.Core.Models;
using OrcShackApi.Data.Repository;

namespace OrcShackApi.Data.UnitTests.Repository
{
    public class DishRepositoryTests
    {
        private Mock<DbSet<Dish>> _mockDishSet;
        private Mock<OrcShackApiContext> _mockContext;
        private Mock<ILogger<DishRepository>> _mockLogger;
        private DishRepository _dishRepository;

        [SetUp]
        public void Setup()
        {
            _mockDishSet = new Mock<DbSet<Dish>>();
            _mockContext = new Mock<OrcShackApiContext>();
            _mockLogger = new Mock<ILogger<DishRepository>>();
            _mockContext.Setup(c => c.Dishes).Returns(_mockDishSet.Object);

            _dishRepository = new DishRepository(_mockContext.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllDishes_WhenCalled_ReturnsAllDishes()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrcShackApiContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory database
                .Options;

            var dishes = new List<Dish> { new Dish() { Description = "dish description 3", Name = "dish1", Image = "images/1234jpg"},  new Dish() { Description = "dish description 3", Name = "dish3", Image = "images/12345jpg" } };

            // Seed the database
            using (var context = new OrcShackApiContext(options))
            {
                await context.Dishes.AddRangeAsync(dishes);
                await context.SaveChangesAsync();
            }

            // Act
            List<Dish> result;
            using (var context = new OrcShackApiContext(options))
            {
                var dishRepository = new DishRepository(context,_mockLogger.Object);
                result = (await dishRepository.GetAllDishes()).ToList();
            }

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetDishById_WithExistingId_ReturnsDish()
        {
            // Arrange
            var dish = new Dish { Id = 1 };
            _mockDishSet.Setup(s => s.FindAsync(dish.Id)).ReturnsAsync(dish);

            // Act
            var result = await _dishRepository.GetDishById(dish.Id);

            // Assert
            Assert.AreEqual(dish, result);
            _mockDishSet.Verify(s => s.FindAsync(dish.Id), Times.Once);
        }

        [Test]
        public async Task CreateDish_WithValidDish_ReturnsDish()
        {
            // Arrange
            var dish = new Dish { Id = 1 };
            _mockDishSet.Setup(s => s.AddAsync(dish, default)).Returns(null);

            // Act
            var result = await _dishRepository.CreateDish(dish);

            // Assert
            Assert.AreEqual(dish, result);
            _mockDishSet.Verify(s => s.AddAsync(dish, default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task UpdateDish_WithValidDish_CallsUpdateOnContext()
        {
            // Arrange
            var dish = new Dish { Id = 1 };
            _mockDishSet.Setup(s => s.Update(dish));

            // Act
            await _dishRepository.UpdateDish(dish);

            // Assert
            _mockDishSet.Verify(s => s.Update(dish), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task DeleteDish_WithExistingId_CallsRemoveOnContext()
        {
            // Arrange
            var id = 1;
            var dish = new Dish { Id = id };
            _mockDishSet.Setup(s => s.FindAsync(id)).ReturnsAsync(dish);

            // Act
            await _dishRepository.DeleteDish(id);

            // Assert
            _mockDishSet.Verify(s => s.FindAsync(id), Times.Once);
            _mockDishSet.Verify(s => s.Remove(dish), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}
