using System;
using System.Collections.Generic;
using System.Linq;

namespace Day_12
{
    public class EmployeeOperations
    {
        private Dictionary<int, Employee> employees = new Dictionary<int, Employee>();

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
            ConsoleHelper.PrintBlue("\nEmployee added successfully!\n");
        }

        public void SortAndDisplayEmployees()
        {
            var list = employees.Values.ToList();
            list.Sort(); // uses CompareTo
            ConsoleHelper.PrintBlue("\nEmployees sorted by Salary:");
            foreach (var emp in list)
                ConsoleHelper.PrintBlue(emp.ToString());
        }

        public void GetEmployeeById()
        {
            int id = UserInputHelper.GetValidPositiveInt("Enter Employee ID to search: ");

            //if (employees.TryGetValue(id, out var emp))
            //    ConsoleHelper.PrintBlue(emp.ToString());

            var emp = employees.Values.FirstOrDefault(e => e.Id == id); // Using LINQ

            if (emp != null)
                ConsoleHelper.PrintBlue(emp.ToString());
            else
                ConsoleHelper.PrintRed("Employee not found.");
        }

        public void SearchByName()
        {
            string name = UserInputHelper.GetValidString("Enter name to search: ");

            var matches = employees.Values
                .Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count == 0)
                ConsoleHelper.PrintRed("No employees found with that name.");
            else
            {
                ConsoleHelper.PrintBlue($"Found {matches.Count} employee(s):");
                matches.ForEach(e => ConsoleHelper.PrintBlue(e.ToString()));
            }
        }


        public void FindElderThanEmployee()
        {
            int id = UserInputHelper.GetValidPositiveInt("Enter employee ID to compare age with: ");

            if (employees.TryGetValue(id, out var emp))
            {
                int age = emp.Age;
                var elders = employees.Values.Where(e => e.Age > age).ToList();

                if (elders.Count == 0)
                    ConsoleHelper.PrintBlue("No employees are elder.");
                else
                {
                    ConsoleHelper.PrintBlue("Employees elder than selected:");
                    elders.ForEach(e => ConsoleHelper.PrintBlue(e.ToString()));
                }
            }
            else
            {
                ConsoleHelper.PrintRed("Employee ID not found.");
            }
        }
    }
}
