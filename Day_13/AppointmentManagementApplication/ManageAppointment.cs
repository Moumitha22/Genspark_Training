using System;
using System.Collections.Generic;
using AppointmentManagementApplication.Interfaces;
using AppointmentManagementApplication.Models;

namespace AppointmentManagementApplication
{
    public class ManageAppointment
    {
        private readonly IAppointmentService _appointmentService;

        public ManageAppointment(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                PrintMenu();

                Console.Write("\nChoose an option: ");
                int option = 0;
                while (!int.TryParse(Console.ReadLine(), out option) || (option < 1 && option > 2))
                {
                    Console.WriteLine("Invalid entry. Please enter a valid option");
                }
                switch (option)
                {
                    case 1:
                        AddAppoinment();
                        break;

                    case 2:
                        SearchAppointments();
                        break;

                    case 3:
                        exit = true;
                        Console.WriteLine("Exiting...");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void PrintMenu()
        {
            Console.WriteLine("\n==== Appointment Management System ====");
            Console.WriteLine("1. Add Patient");
            Console.WriteLine("2. Search Patient");
            Console.WriteLine("3. Exit");
        }

        private void AddAppoinment()
        {
            Appointment appointment = new Appointment();
            appointment.GetAppointmentDetailsFromUser();
            int result = _appointmentService.AddAppointment(appointment);
            Console.WriteLine((result != -1) ? $"\nAppointment is added with id : {appointment.Id}" : "\nError Creating appointment. Try again!");
        }
        private void SearchAppointments()
        {
            var searchMenu = SearchMenu();
            var appointments = _appointmentService.SearchAppointments(searchMenu);

            Console.WriteLine("\nThe search options you have selected");
            Console.WriteLine(searchMenu); 
            if (appointments != null && appointments.Count > 0)
            {
                
                PrintAppointments(appointments);
            }
            else
            {
                Console.WriteLine("No Patient found matching the criteria.");
            }
        }
        private AppointmentSearchModel SearchMenu()
        {
            AppointmentSearchModel searchModel = new AppointmentSearchModel();

            Console.Write("Enter Name (leave blank to skip): ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
                searchModel.PatientName = name;

            Console.Write("Enter Min Age (leave blank to skip): ");
            string? minAgeStr = Console.ReadLine();
            Console.Write("Enter Max Age (leave blank to skip): ");
            string? maxAgeStr = Console.ReadLine();
            if (int.TryParse(minAgeStr, out int minAge) && int.TryParse(maxAgeStr, out int maxAge))
                searchModel.AgeRange = new Range<int> { MinVal = minAge, MaxVal = maxAge };

            Console.Write("Enter Appointment Date (leave blank to skip): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                searchModel.AppointmentDate = date;

            return searchModel;
        }
        private void PrintAppointments(List<Appointment>? appointments)
        {
            foreach (var appointment in appointments)
            {
                Console.WriteLine("\n----------------------------------");
                Console.WriteLine(appointment);
                Console.WriteLine("\n----------------------------------");
            }
        }
    }
}
