using Microsoft.EntityFrameworkCore;
using System;

namespace HumanResourcesDepartment.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<OrganizationalUnit> OrganizationalUnits { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public AppDbContext(DbContextOptions <AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationalUnit>().HasData(
              new OrganizationalUnit { Id = 1, Name = "Administracija", EstablishmentYear = 2010 },
              new OrganizationalUnit { Id = 2, Name = "Računovodstvo", EstablishmentYear = 2012 },
              new OrganizationalUnit { Id = 3, Name = "Razvoj", EstablishmentYear = 2013 }
             );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Pera Peric", Role = "Direktor", BirthYear = 1980, EmploymentYear = 2010, Salary = 3000, UnitId = 1 },
                new Employee { Id = 2, FullName = "Mika Mikic", Role = "Sekretar", BirthYear = 1985, EmploymentYear = 2011, Salary = 1000, UnitId = 1 },
                new Employee { Id = 3, FullName = "Iva Ivic", Role = "Računovođa", BirthYear = 1981, EmploymentYear = 2012, Salary = 2000, UnitId = 2 },
                new Employee { Id = 4, FullName = "Zika Zikic", Role = "Inženjer", BirthYear = 1982, EmploymentYear = 2013, Salary = 2500, UnitId = 3 },
                new Employee { Id = 5, FullName = "Ana Anic", Role = "Inženjer", BirthYear = 1984, EmploymentYear = 2014, Salary = 2500, UnitId = 3 }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
