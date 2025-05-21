using AppointmentManagementApplication.Interfaces;
using AppointmentManagementApplication.Models;
using AppointmentManagementApplication.Repositories;
using AppointmentManagementApplication.Services;

namespace AppointmentManagementApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            IRepository<int, Appointment> appointmentRepository = new AppointmentRepository();
            IAppointmentService appointmentService = new AppointmentService(appointmentRepository);
            ManageAppointment manageAppointment = new ManageAppointment(appointmentService);
            manageAppointment.Start();
        }
    }
}
