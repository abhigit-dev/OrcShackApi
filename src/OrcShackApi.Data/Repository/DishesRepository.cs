using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrcShackApi.Core.Models;
using OrcShackApi.Data.Helper;

namespace OrcShackApi.Data.Repository
{
    public class DishRepository : IDishRepository
    {
        private readonly OrcShackApiContext _context;
        private readonly ILogger<DishRepository> _logger;

        public DishRepository(OrcShackApiContext context, ILogger<DishRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Dish>> GetAllDishes()
        {
            _logger.LogInformation("Getting all dishes");
            var dishes = await _context.Dishes.ToListAsync();
            _logger.LogInformation("Retrieved {Count} dishes", dishes.Count());
            return dishes;
        }

        public async Task<Dish> GetDishById(int id)
        {
            _logger.LogInformation("Getting dish by id: {Id}", id);
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                _logger.LogError("Dish not found");
                throw new NotFoundException("Dish not found");
            }
            _logger.LogInformation("Dish retrieved successfully");
            return dish;
        }

        public async Task<Dish> CreateDish(Dish dish)
        {
            _logger.LogInformation("Creating new dish");
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Dish created successfully");
            return dish;
        }

        public async Task UpdateDish(Dish dish)
        {
            _logger.LogInformation("Updating dish with id: {Id}", dish.Id);
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Dish updated successfully");
        }

        public async Task DeleteDish(int id)
        {
            _logger.LogInformation("Deleting dish with id: {Id}", id);
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Dish deleted successfully");
            }
            else
            {
                _logger.LogError("Dish not found");
            }
        }

        public async Task<IEnumerable<Dish>> GetDishesByName(string name)
        {
            _logger.LogInformation("Getting dish by name: {name}", name);

            var dish = await _context.Dishes.Where(d => d.Name.Contains(name)).ToListAsync();

            _logger.LogInformation("Dish retrieved successfully");
            return dish;
        }

        public async Task UpdateDishRating(int id,int userid ,int rate, string review)
        {
            _logger.LogInformation("Rating dish with id: {Id}", id);

            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                // Add the new rating
                var newRating = new Rating() { Review = review, DishId = dish.Id, UserId = userid, Rate = rate};
                _context.Ratings.Add(newRating);
                await _context.SaveChangesAsync();

                // Calculate the average rating
                var ratings = await _context.Ratings.Where(r => r.DishId == dish.Id).ToListAsync();
                dish.Rating = ratings.Average(r => r.Rate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Dish rated successfully");
            }
        }
    }
}
