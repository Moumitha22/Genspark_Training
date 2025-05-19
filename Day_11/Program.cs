namespace Day_11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Choose a task to run:");
                Console.WriteLine("1  - Greet the user by name");
                Console.WriteLine("2  - Find the largest of two numbers");
                Console.WriteLine("3  - Perform an arithmetic operation on two numbers");
                Console.WriteLine("4  - Login system with 3 attempts (username: Admin, password: pass)");
                Console.WriteLine("5  - Count numbers divisible by 7 (input 10 numbers)");
                Console.WriteLine("6  - Frequency count of elements in an array");
                Console.WriteLine("7  - Left rotate array by one position");
                Console.WriteLine("8  - Merge two integer arrays");
                Console.WriteLine("9  - Bulls and Cows word game (4-letter guess vs secret word)");
                Console.WriteLine("10 - Validate a Sudoku row (9 unique numbers from 1 to 9)");
                Console.WriteLine("11 - Validate entire Sudoku board (9x9 grid)");
                Console.WriteLine("12 - Encrypt/Decrypt message using Caesar cipher (shift by 3)");
                Console.WriteLine("0  - Exit");
                Console.Write("\nEnter your choice: ");
                string? input = Console.ReadLine();
                Console.WriteLine();
                switch (input)
                {
                    case "1":
                        Task_01.Run();
                        break;

                    case "2":
                        Task_02.Run();
                        break;

                    case "3":
                        Task_03.Run();
                        break;

                    case "4":
                        Task_04.Run();
                        break;

                    case "5":
                        Task_05.Run();
                        break;

                    case "6":
                        Task_06.Run();
                        break;

                    case "7":
                        Task_07.Run();
                        break;

                    case "8":
                        Task_08.Run();
                        break;

                    case "9":
                        Task_09.Run();
                        break;

                    case "10":
                        Task_10.Run();
                        break;

                    case "11":
                        Task_11.Run();
                        break;

                    case "12":
                        Task_12.Run();
                        break;

                    case "0":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
