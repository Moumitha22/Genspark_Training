using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Repositories
{

    public class SpecialityRepository : Repository<int, Speciality>
    {
        public SpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Speciality> Get(int key)
        {
            var Speciality = await _clinicContext.Specialities.SingleOrDefaultAsync(d => d.Id == key);
            return Speciality ?? throw new Exception($"No speciality with given ID {key}");
        }

        public override async Task<IEnumerable<Speciality>> GetAll()
        {
            var specialities = _clinicContext.Specialities;
            if (specialities.Count() == 0)
            {
                throw new Exception("No specialities in the database");
            }
            return await specialities.ToListAsync();
        }

    }
}
