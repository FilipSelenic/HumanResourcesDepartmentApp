using FluentValidation;
using HumanResourcesDepartment.Models.DTO;

namespace HumanResourcesDepartment.Validations
{
    public class EmployeeCreateValidation : AbstractValidator<EmployeeCreateDTO>
    {
        public EmployeeCreateValidation() 
        { 
            RuleFor(model => model.FullName).NotEmpty().MaximumLength(70);
            RuleFor(model => model.Role).NotEmpty().MaximumLength(50);
            RuleFor(model => model.BirthYear).NotEmpty().InclusiveBetween(1960, 1999);
            RuleFor(model => model.EmploymentYear).NotEmpty().InclusiveBetween(2010, 2020);
            RuleFor(model => model.Salary).NotEmpty().ExclusiveBetween(250, 10000);
        }
    }
}
