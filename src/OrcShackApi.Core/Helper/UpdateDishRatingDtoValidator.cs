using FluentValidation;
using OrcShackApi.Core.Models;

namespace OrcShackApi.Core.Helper
{
    public class UpdateDishRatingDtoValidator : AbstractValidator<UpdateDishRatingDto>
    {
        public UpdateDishRatingDtoValidator()
        {
            RuleFor(x => x.DishId).NotEmpty().WithMessage("DishId must not be empty."); ;
            RuleFor(x => x.Rate).InclusiveBetween(0, 10).WithMessage("Rating must be between 0 and 10."); 
            RuleFor(x => x.Review).NotEmpty().WithMessage("Review must not be empty."); 
        }
    }
}
