using HumanResourcesDepartment.Models;

namespace HumanResourcesDepartment.Repository
{
    public interface IOrgUnitsRepository
    {
        void Add(OrganizationalUnit unit);
        void Delete(OrganizationalUnit unit);
        IQueryable<OrganizationalUnit> GetAll();
        OrganizationalUnit GetById(int id);
        void Update(OrganizationalUnit unit);
    }
}