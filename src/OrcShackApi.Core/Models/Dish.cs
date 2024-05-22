using System.Text.Json.Serialization;

namespace OrcShackApi.Core.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public double Rating { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<Rating> Ratings { get; set; }
    }
}
