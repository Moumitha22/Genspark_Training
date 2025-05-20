namespace Day_12
{
    class Task_02_Easy
    {
        public static void Run()
        {
            int choice;
            EmployeePromotion promotion = new EmployeePromotion();
            do
            {
                Console.WriteLine("\n------ Employee Promotion Tasks-----");
                Console.WriteLine("1. Add employee names");
                Console.WriteLine("2. Find promotion position of an employee");
                Console.WriteLine("3. Trim extra memory used by the list");
                Console.WriteLine("4. Print promoted employee list in ascending order");
                Console.WriteLine("5. Exit");
                ConsoleHelper.PrintGreen("\nEnter your choice (1-5): ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    ConsoleHelper.PrintRed("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        promotion.EnterEmployeeNames();
                        break;
                    case 2:
                        promotion.FindEmployeePosition();
                        break;
                    case 3:
                        promotion.ShrinkListMemory();
                        break;
                    case 4:
                        promotion.DisplayPromotedEmployees();
                        break;
                    case 5:
                        ConsoleHelper.PrintBlue("\nExiting program.\n");
                        break;
                    default:
                        ConsoleHelper.PrintRed("\nInvalid choice. Try again.");
                        break;
                }

            } while (choice != 5);
        }

    }
}
