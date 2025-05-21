using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentManagementApplication.Interfaces;
using AppointmentManagementApplication.Models;

namespace AppointmentManagementApplication.Services
{
    public class AppointmentService : IAppointmentService
    {
        IRepository<int, Appointment> _appointmentrepository;
        public AppointmentService(IRepository<int, Appointment> appointmentRepository)
        {
            _appointmentrepository = appointmentRepository;
        }

        public int AddAppointment(Appointment appointment)
        {
            try
            {
                var result = _appointmentrepository.Add(appointment);
                if (result != null)
                {
                    return result.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;
        }

        public List<Appointment>? SearchAppointments(AppointmentSearchModel searchModel)
        {
            try
            {
                var appointments = _appointmentrepository.GetAll();
                appointments = SearchByPatientName(appointments, searchModel.PatientName);
                appointments = SearchByAppointmentDate(appointments, searchModel.AppointmentDate);
                appointments = SearchByPatientAge(appointments, searchModel.AgeRange);

                if (appointments != null && appointments.Count > 0)
                {
                    return appointments.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        private ICollection<Appointment> SearchByPatientName(ICollection<Appointment> appointments, string? patientName)
        {
            if (string.IsNullOrEmpty(patientName) || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            return appointments.Where(e => e.PatientName.ToLower().Contains(patientName.ToLower())).ToList();
        }

        private ICollection<Appointment> SearchByAppointmentDate(ICollection<Appointment> appointments, DateTime? appointmentDate)
        {
            if (appointmentDate == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            return appointments.Where(e => e.AppointmentDate == appointmentDate).ToList();
        }

        private ICollection<Appointment> SearchByPatientAge(ICollection<Appointment> appointments, Range<int>? patientAge)
        {
            if (patientAge == null || appointments == null || appointments.Count == 0)
            {
                return appointments;
            }
            return appointments.Where(e => e.PatientAge >= patientAge.MinVal && e.PatientAge <= patientAge.MaxVal).ToList();
        }
    }
}
