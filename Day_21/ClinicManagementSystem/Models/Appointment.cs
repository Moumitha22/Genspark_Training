using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class Appointment
    {
        public string AppointmentNumber { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonToVisit { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }

    }
}
