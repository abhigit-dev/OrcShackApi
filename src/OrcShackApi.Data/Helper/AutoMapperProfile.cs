using AutoMapper;
using OrcShackApi.Core.Models;

namespace OrcShackApi.Data.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
                
            CreateMap<DishDto, Dish>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom<ImageResolver>());
        }
    }
}
