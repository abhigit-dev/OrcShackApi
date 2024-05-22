using OrcShackApi.Core.Models;
using OrcShackApi.Data.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrcShackApi.Web.Services
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepository;
        private readonly ILogger<DishService> _logger;

        public DishService(IDishRepository dishRepository, ILogger<DishService> logger)
        {
            _dishRepository = dishRepository ?? throw new ArgumentNullException(nameof(dishRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Dish>> GetAllDishes()
        {
            _logger.LogInformation("Getting all dishes");
            var dishes = await _dishRepository.GetAllDishes();
            _logger.LogInformation("Retrieved {Count} dishes", dishes.Count());
            return dishes;
        }

        public async Task<Dish> GetDishById(int id)
        {
            _logger.LogInformation("Getting dish by id: {Id}", id);
            var dish = await _dishRepository.GetDishById(id);
            _logger.LogInformation(dish != null ? "Dish retrieved successfully" : "Failed to retrieve dish");
            return dish;
        }

        public async Task<Dish> CreateDish(Dish dish)
        {
            _logger.LogInformation("Creating new dish");
            var newDish = await _dishRepository.CreateDish(dish);
            _logger.LogInformation("Dish created successfully");
            return newDish;
        }

        public async Task UpdateDish(Dish dish)
        {
            _logger.LogInformation("Updating dish with id: {Id}", dish.Id);
            await _dishRepository.UpdateDish(dish);
            _logger.LogInformation("Dish updated successfully");
        }

        public async Task DeleteDish(int id)
        {
            _logger.LogInformation("Deleting dish with id: {Id}", id);
            await _dishRepository.DeleteDish(id);
            _logger.LogInformation("Dish deleted successfully");
        }

        public async Task<IEnumerable<Dish>> GetDishesByName(string name)
        {
            _logger.LogInformation("Getting dish by name: {name}", name);
            var dishes = await _dishRepository.GetDishesByName(name);
            _logger.LogInformation("Dish deleted successfully");
            return dishes;
        }

        public async Task UpdateDishRating(int id, int userid, int rate, string review)
        {
            _logger.LogInformation("Deleting dish with id: {Id}", id);
            await _dishRepository.UpdateDishRating(id, userid, rate, review);
            _logger.LogInformation("Dish deleted successfully");
        }
    }
}
