using System;

namespace Day_12
{
    public static class UserInputHelper
    {
        public static int GetValidPositiveInt(string prompt)
        {
            int value;
            ConsoleHelper.PrintGreen(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                ConsoleHelper.PrintRed("Invalid input. Please enter a valid positive integer:");
            }
            return value;
        }

        public static int GetValidNonNegativeInt(string prompt)
        {
            int value;
            ConsoleHelper.PrintGreen(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                ConsoleHelper.PrintRed("Invalid input. Please enter a valid non-negative integer:");
            }
            return value;
        }

        public static string GetValidString(string prompt)
        {
            string? input;
            ConsoleHelper.PrintGreen(prompt);
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    ConsoleHelper.PrintRed("Input cannot be empty. Please enter a valid value:");
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        public static double GetValidDouble(string prompt)
        {
            double value;
            ConsoleHelper.PrintGreen(prompt);
            while (!double.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                ConsoleHelper.PrintRed("Invalid input. Please enter a valid non-negative number:");
            }
            return value;
        }
    }
}
