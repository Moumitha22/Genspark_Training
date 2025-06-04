using System;

namespace ClinicManagementSystem.Models.DTOs
{
    public class AppointmentAddRequestDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ReasonToVisit { get; set; } = string.Empty;
    }
}
