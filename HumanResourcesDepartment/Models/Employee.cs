using System.ComponentModel.DataAnnotations;

namespace HumanResourcesDepartment.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(70)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [Range(1960, 1999)]
        public int BirthYear { get; set; }

        [Required]
        [Range(2010, 2020)]
        public int EmploymentYear { get; set; }

        [Required]
        [Range(250, 10000)]
        public decimal Salary { get; set; }
        public int UnitId { get; set; }
        public OrganizationalUnit Unit { get; set; }
    }
}
