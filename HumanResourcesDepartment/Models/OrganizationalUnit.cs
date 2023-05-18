using System.ComponentModel.DataAnnotations;

namespace HumanResourcesDepartment.Models
{
    public class OrganizationalUnit
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Range(2010, 2020)]
        public int EstablishmentYear { get; set; }
    }
}
