using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Interfaces
{
    public interface IAppointmentService
    {
        public Task<ICollection<Appointment>> GetAllAppointments();
        public Task<Appointment> GetAppointmentById(string id);
        public Task<Appointment> AddAppointment(AppointmentAddRequestDto appointmentAddRequestDto);
        public Task<bool> CancelAppointment(string appointmentNumber, string doctorEmail);
    }
}
