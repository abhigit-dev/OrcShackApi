using FluentValidation;
using OrcShackApi.Core.Models;

namespace OrcShackApi.Core.Helper
{
    public class PasswordUpdateDtoValidator : AbstractValidator<PasswordUpdateDto>
    {
        public PasswordUpdateDtoValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.")
                .Length(6, 100).WithMessage("Old password must be between 6 and 100 characters.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .Length(6, 100).WithMessage("New password must be between 6 and 100 characters.");
        }
    }
}
