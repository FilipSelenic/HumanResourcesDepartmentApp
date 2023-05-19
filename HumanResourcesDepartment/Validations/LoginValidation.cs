using FluentValidation;
using HumanResourcesDepartment.Models.DTO;

namespace HumanResourcesDepartment.Validations
{
    public class LoginValidation : AbstractValidator<LoginDTO>
    {
        public LoginValidation()
        {
            RuleFor(model => model.Username).NotEmpty().WithMessage("Please enter your username!");
            RuleFor(model => model.Password).NotEmpty().WithMessage("Please enter your password!");
        }
    }
}
