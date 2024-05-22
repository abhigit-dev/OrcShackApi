namespace OrcShackApi.Core.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int DishId { get; set; }
        public int UserId { get; set; }
        public int Rate { get; set; }
        public required string Review { get; set; }

        // Navigation properties
        public Dish Dish { get; set; }
        public User User { get; set; }
    }

}
