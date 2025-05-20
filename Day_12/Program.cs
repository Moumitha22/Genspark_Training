namespace Day_12
{
    internal class Program
    {
        static void Main()
        {
            int choice;
            do
            {
                Console.WriteLine("-------------------------- MENU --------------------------");
                Console.WriteLine("1. Task 01 - Instagram Posts");
                Console.WriteLine("2. Task 02 - Employee Promotion (Easy)");
                Console.WriteLine("3. Task 02 - Employee Operations - Search, Filter (Medium)");
                Console.WriteLine("4. Task 02 - Employee Management (Hard)");
                Console.WriteLine("5. Task 03 - Products Mangement");
                Console.WriteLine("6. Exit");
                ConsoleHelper.PrintGreen("\nEnter your choice (1-5): ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    ConsoleHelper.PrintRed("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Task_01.Run();
                        break;

                    case 2:
                        Task_02_Easy.Run();
                        break;

                    case 3:
                        Task_02_Medium.Run();
                        break;

                    case 4:
                        Task_02_Hard.Run();
                        break;

                    case 5:
                        Task_03.Run();
                        break;

                    case 6:
                        ConsoleHelper.PrintBlue("Exiting......");
                        break;

                    default:
                        ConsoleHelper.PrintRed("Invalid choice. Please try again.");
                        break;
                }

            } while (choice != 6);
        }
    }
}
