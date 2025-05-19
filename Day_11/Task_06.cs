namespace Day_11

{
    
    //6) Count the Frequency of Each Element
    //Given an array, count the frequency of each element and print the result.
    //Input: { 1, 2, 2, 3, 4, 4, 4}

    //output
    //1 occurs 1 times  
    //2 occurs 2 times  
    //3 occurs 1 times  
    //4 occurs 3 times
    internal class Task_06
    {
        static int GetValidInteger()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num))
            {
                Console.Write("Invalid input. Please try again: ");
            }
            return num;
        }

        static Dictionary<int, int> CountFrequencies(int[] arr)
        {
            Dictionary<int, int> freq = new Dictionary<int, int>();

            foreach (int num in arr)
            {
                if (freq.ContainsKey(num))
                {
                    freq[num]++;
                }
                else
                {
                    freq[num] = 1;
                }
            }
            return freq;

        }

        public static void Run()
        {
            Console.Write("Please enter the size of the array.");
            int n;
            while (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.Write("Invalid input. Please enter a positive integer: ");
            }

            int[] arr = new int[n];

            Console.WriteLine($"\nPlease enter {n} integers.\n");

            for (int i = 0; i < n; i++)
            {
                Console.Write($"Please enter number {i + 1}: ");
                arr[i] = GetValidInteger();
            }

            Dictionary<int, int> freq = CountFrequencies(arr);

            Console.WriteLine("\n------------Frequencies------------");
            foreach (var pair in freq)
            {
                Console.WriteLine($"{pair.Key} occurs {pair.Value} times.");
            }
        }
    }
}
