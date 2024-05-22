using OrcShackApi.Core.Models;

namespace OrcShackApi.Web.Services
{
    public interface IDishService
    {
        Task<IEnumerable<Dish>> GetAllDishes();
        Task<Dish> GetDishById(int id);
        Task<Dish> CreateDish(Dish dish);
        Task UpdateDish(Dish dish);
        Task DeleteDish(int id);
        Task<IEnumerable<Dish>> GetDishesByName(string name);
        Task UpdateDishRating(int id, int userid, int rate, string review);
    }
}
