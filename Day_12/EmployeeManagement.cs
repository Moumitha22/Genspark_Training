using System;
using System.Collections.Generic;

namespace Day_12
{
    public class EmployeeManagement
    {
        private Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

        public void DisplayAllEmployees()
        {
            if (employees.Count == 0)
            {
                ConsoleHelper.PrintRed("No employees found.");
                return;
            }

            ConsoleHelper.PrintBlue("\nEmployee Details:");
            foreach (var emp in employees.Values)
            {
                ConsoleHelper.PrintBlue(emp.ToString());
            }
        }

        public void AddEmployee()
        {
            Employee emp = new Employee();
            emp.TakeEmployeeDetailsFromUser();

            if (employees.ContainsKey(emp.Id))
            {
                ConsoleHelper.PrintRed("An employee with this ID already exists.\n");
                return;
            }

            employees.Add(emp.Id, emp);
            ConsoleHelper.PrintGreen("Employee added successfully!\n");
        }

        public void ModifyEmployeeDetails()
        {
            ConsoleHelper.PrintGreen("\nEnter Employee ID to modify: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                if (employees.ContainsKey(id))
                {
                    Employee emp = employees[id];
                    ConsoleHelper.PrintGreen("\nModify details for " + emp.Name);

                    // Modify name, age, or salary (ID is not modifiable)
                    ConsoleHelper.PrintGreen("Enter new name (leave blank to keep current): ");
                    string newName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newName))
                        emp.Name = newName;

                    ConsoleHelper.PrintGreen("Enter new age (leave blank to keep current): ");
                    string ageInput = Console.ReadLine();
                    if (int.TryParse(ageInput, out int newAge))
                        emp.Age = newAge;

                    ConsoleHelper.PrintGreen("Enter new salary (leave blank to keep current): ");
                    string salaryInput = Console.ReadLine();
                    if (double.TryParse(salaryInput, out double newSalary))
                        emp.Salary = newSalary;

                    ConsoleHelper.PrintGreen("Employee details updated successfully!\n");
                }
                else
                {
                    ConsoleHelper.PrintRed("Employee with this ID not found.");
                }
            }
            else
            {
                ConsoleHelper.PrintRed("Invalid ID.");
            }
        }



        public void PrintEmployeeById()
        {
            int id = UserInputHelper.GetValidPositiveInt("Enter Employee ID to view details: ");

            if (employees.ContainsKey(id))
            {
                ConsoleHelper.PrintBlue(employees[id].ToString());
            }
            else
            {
                ConsoleHelper.PrintRed($"Employee with ID {id} not found.");
            }
        }

        public void DeleteEmployeeById()
        {
            int id = UserInputHelper.GetValidPositiveInt("Enter Employee ID to delete: ");

            if (employees.ContainsKey(id))
            {
                employees.Remove(id);
                ConsoleHelper.PrintBlue("\nEmployee deleted successfully!\n");
            }
            else
            {
                ConsoleHelper.PrintRed($"Employee with ID {id} not found.");
            }
        }
    }
}
