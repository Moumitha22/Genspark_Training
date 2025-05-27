using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagementSystem.Models
{
    public class Appointment : IEquatable<Appointment>
    {
        [Key]
        public string AppointmentNumber { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonToVisit { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        public bool Equals(Appointment? other)
        {
            return this.AppointmentNumber == other?.AppointmentNumber;
        }
    }
}
