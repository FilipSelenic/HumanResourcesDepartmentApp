using AutoMapper;
using HumanResourcesDepartment.Models;
using HumanResourcesDepartment.Models.DTO;

namespace HumanResourcesDepartment
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Employee, EmployeeCreateDTO>().ReverseMap();
            CreateMap<Employee, EmployeeDTO>().ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));
            CreateMap<Employee, EmployeeUpdateDTO>().ReverseMap();

        }
    }
}
