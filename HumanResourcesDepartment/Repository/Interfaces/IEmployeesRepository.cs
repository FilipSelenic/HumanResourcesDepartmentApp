using HumanResourcesDepartment.Models;

namespace HumanResourcesDepartment.Repository.Interfaces
{
    public interface IEmployeesRepository
    {
        void Add(Employee employee);
        void Delete(Employee employee);
        IQueryable<Employee> GetAll();
        Employee GetById(int id);
        void Update(Employee employee);
    }
}