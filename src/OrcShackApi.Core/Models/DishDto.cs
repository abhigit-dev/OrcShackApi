using Microsoft.AspNetCore.Http;

namespace OrcShackApi.Core.Models
{
    public class DishDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public IFormFile Image { get; set; }
    }
}
