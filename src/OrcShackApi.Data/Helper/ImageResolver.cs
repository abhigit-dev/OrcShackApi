using AutoMapper;
using OrcShackApi.Core.Models;

namespace OrcShackApi.Data.Helper
{
    public class ImageResolver : IValueResolver<DishDto, Dish, string>
    {
        public string Resolve(DishDto source, Dish destination, string destMember, ResolutionContext context)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(source.Image.FileName);
            var path = Path.Combine($"Images", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            source.Image.CopyTo(stream);

            return path;

        }
    }

}
