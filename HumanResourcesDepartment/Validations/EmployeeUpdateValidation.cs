using FluentValidation;
using HumanResourcesDepartment.Models.DTO;

namespace HumanResourcesDepartment.Validations
{
    public class EmployeeUpdateValidation : AbstractValidator<EmployeeUpdateDTO>
    {

        public EmployeeUpdateValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.FullName).NotEmpty();
            RuleFor(model => model.FullName).MaximumLength(70);
            RuleFor(model => model.Role).NotEmpty();
            RuleFor(model => model.Role).MaximumLength(50);
            RuleFor(model => model.BirthYear).NotEmpty();
            RuleFor(model => model.BirthYear).InclusiveBetween(1960, 1999);
            RuleFor(model => model.EmploymentYear).NotEmpty();
            RuleFor(model => model.EmploymentYear).InclusiveBetween(2010, 2020);
            RuleFor(model => model.Salary).NotEmpty();
            RuleFor(model => model.Salary).ExclusiveBetween(250, 10000);
        }
    }
}
