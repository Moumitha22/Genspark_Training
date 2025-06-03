using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class Doctor 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public float YearsOfExperience { get; set; }
        public string Status { get; set; } = string.Empty;
        public ICollection<DoctorSpeciality> DoctorSpecialities { get; set; } = new List<DoctorSpeciality>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public User? User { get; set; }
    }
}