namespace ClinicManagementSystem.Models
{
    public class Doctor : IEquatable<Doctor>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float YearsOfExperience { get; set; }
        public string Status { get; set; } = string.Empty;
        public ICollection<DoctorSpeciality> DoctorSpecialities { get; set; } = new List<DoctorSpeciality>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        // public ICollection<DoctorSpeciality>? DoctorSpecialities { get; set; }
        // public ICollection<Appointment>? Appointments { get; set; }

        public bool Equals(Doctor? other)
        {
            return this.Id == other?.Id;
        }
    }
}