namespace Day_11

{
    // 8) Given two integer arrays, merge them into a single array.
    //Input: {1, 3, 5}
    //and {2, 4, 6}
    //Output: { 1, 3, 5, 2, 4, 6}

    internal class Task_08
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

        static int[] MergeArrays(int[] a, int[] b)
        {
            int[] result = new int[a.Length + b.Length];

            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i];
            }

            for (int i = 0; i < b.Length; i++)
            {
                result[a.Length + i] = b[i];
            }

            return result;
        }
        public static void Run()
        {
            Console.Write("Please enter the size of the first array: ");
            
            int n1;
            
            while (!int.TryParse(Console.ReadLine(), out n1) || n1 <= 0)
            {
                Console.Write("Invalid input. Please enter a positive integer: ");
            }

            int[] arr1 = new int[n1];

            Console.WriteLine($"\nPlease enter {n1} numbers for first array.\n");

            for (int i = 0; i < n1; i++)
            {
                Console.Write($"Please enter number {i + 1}: ");
                arr1[i] = GetValidInteger();
            }

            Console.Write("\nPlease enter the size of the second array: ");
            
            int n2;
            
            while (!int.TryParse(Console.ReadLine(), out n2) || n2 <= 0)
            {
                Console.Write("Invalid input. Please enter a positive integer: ");
            }

            int[] arr2 = new int[n2];

            Console.WriteLine($"\nPlease enter {n1} numbers for first array.\n");

            for (int i = 0; i < n1; i++)
            {
                Console.Write($"Please enter number {i + 1}: ");
                arr2[i] = GetValidInteger();
            }

            int[] merged = MergeArrays(arr1, arr2);

            Console.WriteLine("\nArray1: " + string.Join(", ", arr1));
            Console.WriteLine("\nArray2: " + string.Join(", ", arr2));
            Console.WriteLine("\nMerged Array: " + string.Join(", ", merged));

        }
    }
}
