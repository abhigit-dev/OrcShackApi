namespace OrcShackApi.Core.Models
{
    public class UpdateDishRatingDto
    {
        public int DishId { get; set; }
        public int Rate { get; set; }
        public string Review { get; set; }
    }
}
