using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Misc
{

    public class AppointmentMapper
    {
        public Appointment MapAppointmentAddRequestDtoToAppointment(AppointmentAddRequestDto addRequestDto)
        {
            Appointment Appointment = new();
            Appointment.AppointmentNumber = $"APT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 4)}";
            Appointment.PatientId = addRequestDto.PatientId;
            Appointment.DoctorId = addRequestDto.DoctorId;
            Appointment.AppointmentDateTime = DateTime.SpecifyKind(addRequestDto.AppointmentDateTime, DateTimeKind.Utc);
            Appointment.ReasonToVisit = addRequestDto.ReasonToVisit;
            Appointment.Status = "Scheduled";
            return Appointment;
        }

    }
}
