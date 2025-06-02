using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace ClinicManagementSystem.Repositories
{

    public class AppointmentRepository : Repository<string, Appointment>
    {
        public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Appointment> Get(string key)
        {
            var loweredKey = key.ToLower();
            var appointment = await _clinicContext.Appointments.SingleOrDefaultAsync(a => a.AppointmentNumber != null && a.AppointmentNumber == loweredKey);
            return appointment ?? throw new Exception($"No appointment with given appointment number {key}");
        }

        public override async Task<IEnumerable<Appointment>> GetAll()
        {
            var appointments = _clinicContext.Appointments;
            if (appointments.Count() == 0)
            {
                throw new Exception("No appointments in the database");
            }
            return await appointments.ToListAsync();
        }

    }
}
