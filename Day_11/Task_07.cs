namespace Day_11

{
    // 7) create a program to rotate the array to the left by one position.
    //Input: {10, 20, 30, 40, 50}
    //Output: { 20, 30, 40, 50, 10}
    internal class Task_07
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

        static void RotateLeftOneTime(int[] arr)
        {
            int size = arr.Length;
            int firstNum = arr[0];

            for (int i = 0; i < size - 1; i++)
            {
                arr[i] = arr[i + 1];
            }
            arr[size - 1] = firstNum;

        }

        static void Reverse(int[] arr, int l, int r)
        {
            while (l < r)
            {
                int temp = arr[l];
                arr[l] = arr[r];
                arr[r] = temp;
                l++;
                r--;
            }
        }

        static void RotateLeftKTimes(int[] arr, int k)
        {
            int n = arr.Length;
            Reverse(arr, 0, k - 1);
            Reverse(arr, k, n - 1);
            Reverse(arr, 0, n - 1);
        }
        public static void Run()
        {
            Console.Write("Please enter the size of the array: ");
            int n = GetValidInteger();

            int[] arr = new int[n];

            Console.WriteLine($"\nPlease enter {n} integers.\n");

            for (int i = 0; i < n; i++)
            {
                Console.Write($"Please enter number {i + 1}: ");
                arr[i] = GetValidInteger();
            }

            Console.WriteLine("\nOriginal array: " + string.Join(", ", arr));

            RotateLeftOneTime(arr);

            Console.WriteLine("\nArray after rotating left by one place: " + string.Join(", ", arr));

            Console.Write("\nPlease enter the number of times to rotate the array: ");
            int k = GetValidInteger();

            RotateLeftKTimes(arr, k);

            Console.WriteLine($"\nArray after rotating left by {k} places" + string.Join(", ", arr));
        }
    }
}
