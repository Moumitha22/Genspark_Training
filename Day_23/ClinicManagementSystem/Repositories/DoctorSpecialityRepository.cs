using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Repositories
{

    public class DoctorSpecialityRepository : Repository<int, DoctorSpeciality>
    {
        public DoctorSpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<DoctorSpeciality> Get(int key)
        {
            var doctorSpeciality = await _clinicContext.DoctorSpecialities.SingleOrDefaultAsync(d => d.SerialNumber == key);
            return doctorSpeciality ?? throw new Exception($"No doctor speciality with given serial Number {key}");
        }

        public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
        {
            var doctorSpecialities = _clinicContext.DoctorSpecialities;
            if (doctorSpecialities.Count() == 0)
            {
                throw new Exception("No doctor specialities in the database");
            }
            return await doctorSpecialities.ToListAsync();
        }

    }
}
