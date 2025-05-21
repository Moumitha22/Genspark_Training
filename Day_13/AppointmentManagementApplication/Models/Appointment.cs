using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppointmentManagementApplication.Models
{
    public class Appointment : IComparable<Appointment>, IEquatable<Appointment>
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;

        public Appointment() { }

        public Appointment(int id, string patientName, int patientAge, DateTime appointmentDate, string reason)
        {
            Id = id;
            PatientName = patientName;
            PatientAge = patientAge;
            AppointmentDate = appointmentDate;
            Reason = reason;
        }

        public void GetAppointmentDetailsFromUser()
        {
            Console.Write("Please enter the patient name: ");
            string name = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Patient name cannot be empty. Please enter the patient name: ");
                name = Console.ReadLine();
            }
            PatientName = name;

            Console.Write("Please enter the patient age: ");
            int age;
            while(!int.TryParse(Console.ReadLine(), out age) || age < 0)
            {
                Console.Write("Invalid entry for age. Please enter a valid(non-negative number) patient age: ");
            }
            PatientAge = age;

            DateTime date;
            Console.Write("Please enter the appointment date and time between 10AM to 5PM(e.g., yyyy-mm-dd hh-mm AM/PM): ");
            while (!DateTime.TryParse(Console.ReadLine(), out date) || date < DateTime.Today || date.TimeOfDay < new TimeSpan(10, 0, 0) || date.TimeOfDay > new TimeSpan(17, 0, 0) )
            {
                Console.WriteLine("Invalid date. Please enter a valid future date between 10AM to 5PM(yyyy-mm-dd AM/PM): ");
            }
            AppointmentDate = date;

            Console.Write("Please enter reason for visit: ");
            string reason = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(reason))
            {
                Console.Write("Reason cannot be empty. Please enter a valid reason: ");
                reason = Console.ReadLine();
            }
            Reason = reason;
        }

        public override string ToString()
        {
            return "Appointment ID : " + Id + "\nPatient Name : " + PatientName + "\nPatient Age : " + PatientAge + "\nAppointment Date : " + AppointmentDate;
        }

        public int CompareTo(Appointment? other)
        {
            return this.Id.CompareTo(other?.Id);
        }

        public bool Equals(Appointment? other)
        {
            return this.Id == other?.Id;
        }
    }
}
