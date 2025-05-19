namespace Day_11

{
    internal class Task_10
    {
        //10) write a program that accepts a 9-element array representing a Sudoku row.
        //Validates if the row:
        //Has all numbers from 1 to 9.
        //Has no duplicates.
        //Displays if the row is valid or invalid.

        static int GetValidNumber()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Invalid input. Please try again: ");
            }
            return num;
        }
        public static void Run()
        {
            int size = 9;
            Console.WriteLine("Enter a row of sudoku (9 numbers)");

            int[] sudokuRow = new int[size];

            Console.WriteLine("Please enter the numbers:");
            for (int i = 0; i < size; i++)
            {
                Console.Write($"Element {i + 1}: ");
                sudokuRow[i] = GetValidNumber();
                
            }

            HashSet<int> numbers = new();
            foreach (int num in sudokuRow) {
                if(num < 1 || num > 9)
                {
                    Console.WriteLine("All numbers must be between 1 and 9.");
                    return;
                }

                if (!numbers.Add(num))
                {
                    Console.WriteLine("Duplicate number found. Invalid row.");
                    return;
                }
            }
             

            Console.WriteLine("The Sudoku row is valid");
        }


    }
}
