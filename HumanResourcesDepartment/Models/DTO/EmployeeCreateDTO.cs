using System.ComponentModel.DataAnnotations;

namespace HumanResourcesDepartment.Models.DTO
{
    public class EmployeeCreateDTO
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public int BirthYear { get; set; }
        public int EmploymentYear { get; set; }
        public decimal Salary { get; set; }
        public int UnitId { get; set; }
    }
}
