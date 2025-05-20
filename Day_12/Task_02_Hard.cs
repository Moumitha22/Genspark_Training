using System;

namespace Day_12
{
    public class Task_02_Hard
    {
        // Create an instance of EmployeeManagement to interact with the logic
        private static EmployeeManagement empManagement = new EmployeeManagement();

        public static void Run()
        {
            int choice;

            do
            {
                Console.WriteLine("\n------ Employee Management System -----");
                Console.WriteLine("1. Display all employee details");
                Console.WriteLine("2. Add a new employee");
                Console.WriteLine("3. Modify employee details (except ID)");
                Console.WriteLine("4. Print employee details by ID");
                Console.WriteLine("5. Delete employee by ID");
                Console.WriteLine("6. Exit");

                ConsoleHelper.PrintGreen("\nEnter your choice (1-6): ");
                if(!int.TryParse(Console.ReadLine(), out choice))
                {
                    ConsoleHelper.PrintRed("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        empManagement.DisplayAllEmployees();
                        break;
                    case 2:
                        empManagement.AddEmployee();
                        break;
                    case 3:
                        empManagement.ModifyEmployeeDetails();
                        break;
                    case 4:
                        empManagement.PrintEmployeeById();
                        break;
                    case 5:
                        empManagement.DeleteEmployeeById();
                        break;
                    case 6:
                        ConsoleHelper.PrintBlue("Exiting program....");
                        break;
                    default:
                        ConsoleHelper.PrintRed("Invalid choice. Please try again.");
                        break;
                }

            } while (choice != 6);
        }
    }
}
