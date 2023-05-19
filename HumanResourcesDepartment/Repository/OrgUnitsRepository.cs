using HumanResourcesDepartment.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourcesDepartment.Repository
{
    public class OrgUnitsRepository : IOrgUnitsRepository
    {
        private readonly AppDbContext _context;
        public OrgUnitsRepository(AppDbContext context)
        {
            this._context = context;
        }
        public IQueryable<OrganizationalUnit> GetAll()
        {
            return _context.OrganizationalUnits;
        }
        public OrganizationalUnit GetById(int id)
        {
            return _context.OrganizationalUnits.FirstOrDefault(p => p.Id == id);
        }

        public void Add(OrganizationalUnit unit)
        {
            _context.OrganizationalUnits.Add(unit);
            _context.SaveChanges();
        }

        public void Update(OrganizationalUnit unit)
        {
            _context.Entry(unit).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public void Delete(OrganizationalUnit unit)
        {
            _context.OrganizationalUnits.Remove(unit);
            _context.SaveChanges();
        }
    }
}
