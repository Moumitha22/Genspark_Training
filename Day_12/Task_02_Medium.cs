namespace Day_12
{
    public class Task_02_Medium
    {
        public static void Run()
        {
            EmployeeOperations manager = new EmployeeOperations();
            int choice;

            do
            {
                Console.WriteLine("\n--- Employee Management ---");
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. Get Employee by ID");
                Console.WriteLine("3. Sort Employees by Salary");
                Console.WriteLine("4. Search Employees by Name");
                Console.WriteLine("5. Find Employees Elder Than Given Employee");
                Console.WriteLine("6. Exit");
                ConsoleHelper.PrintGreen("\nEnter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    ConsoleHelper.PrintRed("Invalid input. Enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        manager.AddEmployee(); 
                        break;
                    case 2: 
                        manager.GetEmployeeById(); 
                        break;
                    case 3: 
                        manager.SortAndDisplayEmployees(); 
                        break;
                    case 4:
                        manager.SearchByName();
                        break;
                    case 5: 
                        manager.FindElderThanEmployee(); 
                        break;
                    case 6: 
                        ConsoleHelper.PrintBlue("Exiting...");
                        break;
                    default: 
                        ConsoleHelper.PrintRed("Invalid choice."); 
                        break;
                }
            } while (choice != 6);
        }
    }
}
