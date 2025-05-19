namespace Day_11

{
    // 5) Take 10 numbers from user and print the number of numbers that are divisible by 7	
    internal class Task_05
    {
        static int GetValidInteger()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Enter valid integer: ");
            }
            return num;
        }

        static bool CheckIfDivisibleBy7(int num)
        {
            return (num % 7 == 0);
        }

        static int CountDivisibleBy7(int[] nums)
        {
            int count = 0;
            foreach (int num in nums)
            {
                if (CheckIfDivisibleBy7(num))
                {
                    count++;
                }
            }
            return count;
        }
        public static void Run()
        {
            int[] numbers = new int[10];

            Console.WriteLine("Enter 10 integers:");

            for (int i = 0; i < 10; i++)
            {
                Console.Write($"Enter number {i + 1}: ");
                numbers[i] = GetValidInteger();
            }

            int count = CountDivisibleBy7(numbers);

            Console.WriteLine($"\nCount of numbers divisible by 7: {count}");
        }
    }
}
