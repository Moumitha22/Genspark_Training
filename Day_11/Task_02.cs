namespace Day_11
{
    // 2) Take 2 numbers from user and print the largest
    internal class Task_02
    {
        static int GetValidInteger()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Invalid input. Please enter valid integer: ");
            }
            return num;
        }
        static void FindLargest(int num1, int num2)
        {
            if (num1 > num2)
            {
                Console.WriteLine($"\nThe largest number is {num1}.");
            }
            else if (num2 > num1)
            {
                Console.WriteLine($"\nThe largest number is {num2}.");
            }
            else
            {
                Console.WriteLine($"\n{num1} and {num2} are equal.");
            }
        }
        public static void Run()
        {
            Console.Write("Enter first number: ");

            int num1 = GetValidInteger();

            Console.Write("\nEnter second number: ");

            int num2 = GetValidInteger();

            FindLargest(num1, num2);

        }
    }
}
