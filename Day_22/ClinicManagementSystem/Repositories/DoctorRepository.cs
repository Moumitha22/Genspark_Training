using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Repositories
{
    public class DoctorRepository : Repository<int, Doctor>
    {
        public DoctorRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Doctor> Get(int key)
        {
            var doctor = await _clinicContext.Doctors
                .Include(d => d.DoctorSpecialities)
                    .ThenInclude(ds => ds.Speciality)
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == key);

            return doctor ?? throw new Exception($"No doctor with given ID {key}");
        }

        public override async Task<IEnumerable<Doctor>> GetAll()
        {
            var doctors = await _clinicContext.Doctors
                .Include(d => d.DoctorSpecialities)
                    .ThenInclude(ds => ds.Speciality)
                .Include(d => d.Appointments)
                .ToListAsync();

            if (doctors.Count == 0)
            {
                throw new Exception("No doctors in the database");
            }

            return doctors;
        }
    }
}
