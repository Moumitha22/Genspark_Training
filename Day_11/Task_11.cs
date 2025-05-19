namespace Day_11

{
    // 11)  In the question ten extend it to validate a sudoku game.
    // Validate all 9 rows(use int[,] board = new int[9, 9])

    internal class Task_11
    {
        static int GetValidNumber()
        {
            int num;
            while (!int.TryParse(Console.ReadLine(), out num) || num < 1 || num > 9)
            {
                Console.Write("Invalid input. Please enter a number between 1 and 9: ");
            }
            return num;
        }

        static bool IsValidGroup(int[] group)
        {
            HashSet<int> seen = new();
            foreach (int num in group)
            {
                if (num < 1 || num > 9 || !seen.Add(num))
                   return false;
            }
            return true;
        }


        static bool AreRowsValid(int[,] board)
        {
            for (int i = 0; i < 9; i++)
            {
                int[] row = new int[9];
                for (int j = 0; j < 9; j++)
                {
                    row[j] = board[i, j];
                }

                if (!IsValidGroup(row))
                {
                    Console.WriteLine($"\nRow {i + 1} is invalid.");
                    return false;
                }
            }
            return true;
        }

        static bool AreColumnsValid(int[,] board)
        {
            for (int j = 0; j < 9; j++)
            {
                int[] column = new int[9];
                for (int i = 0; i < 9; i++)
                {
                    column[i] = board[i, j];
                }

                if (!IsValidGroup(column))
                {
                    Console.WriteLine($"\nColumn {j + 1} is invalid.");
                    return false;
                }
            }
            return true;
        }

        static bool AreSubgridsValid(int[,] board)
        {
            for (int rowStart = 0; rowStart < 9; rowStart += 3)
            {
                for (int colStart = 0; colStart < 9; colStart += 3)
                {
                    int[] box = new int[9];
                    int index = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            box[index++] = board[rowStart + i, colStart + j];
                        }
                    }

                    if (!IsValidGroup(box))
                    {
                        Console.WriteLine($"\n3x3 box starting at ({rowStart + 1},{colStart + 1}) is invalid.");
                        return false;
                    }
                }
            }
            return true;
        }

        static bool IsValidSudoku(int[,] board)
        {
            return AreRowsValid(board) && AreColumnsValid(board) && AreSubgridsValid(board);
        }


        public static void Run()
        {

            //int[,] board = new int[9, 9];

            //Console.WriteLine("Enter the Sudoku board row by row (each row should have 9 numbers between 1 and 9):");

            //for (int i = 0; i < 9; i++)
            //{
            //    Console.WriteLine($"\nRow {i + 1}:");
            //    for (int j = 0; j < 9; j++)
            //    {
            //        Console.Write($"Element {j + 1}: ");
            //        board[i, j] = GetValidNumber();
            //    }
            //}

            // Valid 
            //int[,] board = {
            //    {7, 9, 2, 1, 5, 4, 3, 8, 6},
            //    {6, 4, 3, 8, 2, 7, 1, 5, 9},
            //    {8, 5, 1, 3, 9, 6, 7, 2, 4},
            //    {2, 6, 5, 9, 7, 3, 8, 4, 1},
            //    {4, 8, 9, 5, 6, 1, 2, 7, 3},
            //    {3, 1, 7, 4, 8, 2, 9, 6, 5},
            //    {1, 3, 6, 7, 4, 8, 5, 9, 2},
            //    {9, 7, 4, 2, 1, 5, 6, 3, 8},
            //    {5, 2, 8, 6, 3, 9, 4, 1, 7}
            //};

            // Valid
            //int[,] board = {
            //    {5,3,4,6,7,8,9,1,2},
            //    {6,7,2,1,9,5,3,4,8},
            //    {1,9,8,3,4,2,5,6,7},
            //    {8,5,9,7,6,1,4,2,3},
            //    {4,2,6,8,5,3,7,9,1},
            //    {7,1,3,9,2,4,8,5,6},
            //    {9,6,1,5,3,7,2,8,4},
            //    {2,8,7,4,1,9,6,3,5},
            //    {3,4,5,2,8,6,1,7,9}
            //};

            // Invalid
            int[,] board = {
                {5,3,4,6,7,8,9,1,2},
                {6,7,2,1,9,5,3,4,8},
                {1,9,8,3,4,2,5,6,7},
                {8,5,8,7,6,1,4,2,3},
                {4,2,6,8,5,3,7,9,1},
                {7,1,3,9,2,4,8,5,6},
                {9,6,1,5,3,7,2,8,4},
                {2,8,7,4,1,9,6,3,5},
                {3,4,5,2,8,6,1,7,9}
            };

            for (int i = 0; i < board.GetLength(0); i++) // Rows
            {
                for (int j = 0; j < board.GetLength(1); j++) // Columns
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }

            if (IsValidSudoku(board))
            {
                Console.WriteLine("\nThe Sudoku board is valid!");
            }
            else
            {
                Console.WriteLine("\nThe Sudoku board is invalid.");
            }
        }
    }
}
