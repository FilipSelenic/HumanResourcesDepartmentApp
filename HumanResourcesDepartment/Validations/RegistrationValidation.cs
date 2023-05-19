using FluentValidation;
using HumanResourcesDepartment.Models.DTO;

namespace HumanResourcesDepartment.Validations
{
    public class RegistrationValidation : AbstractValidator<RegistrationDTO>
    {
        public RegistrationValidation()
        {
            RuleFor(model => model.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(model => model.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(model => model.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
