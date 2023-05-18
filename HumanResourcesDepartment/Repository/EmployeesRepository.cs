using HumanResourcesDepartment.Models;
using HumanResourcesDepartment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResourcesDepartment.Repository
{
    public class EmployeesRepository : IEmployeesRepository
    {
        private readonly AppDbContext _context;
        public EmployeesRepository(AppDbContext context)
        {
            this._context = context;
        }
        public IQueryable<Employee> GetAll()
        {
            return _context.Employees.Include(p => p.Unit);
        }
        public Employee GetById(int id)
        {
            return _context.Employees.Include(p => p.Unit).FirstOrDefault(p => p.Id == id);
        }

        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public void Delete(Employee employee)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }
    }
}
