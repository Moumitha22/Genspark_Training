namespace Day_11

{
    //9) Write a program that:
    //Has a predefined secret word(e.g., "GAME").
    //Accepts user input as a 4-letter word guess.
    //Compares the guess to the secret word and outputs:
    //X Bulls: number of letters in the correct position.
    //Y Cows: number of correct letters in the wrong position.
    //Continues until the user gets 4 Bulls(i.e., correct guess).
    //Displays the number of attempts.

    //Bull = Correct letter in correct position.
    //Cow = Correct letter in wrong position.

    //Secret Word User Guess  Output Explanation
    //GAME GAME	4 Bulls, 0 Cows Exact match
    //GAME    MAGE    2 Bull, 2 Cows AE in correct position, MG misplaced
    //GAME GUYS	1 Bull, 0 Cows G in correct place, rest wrong
    //GAME AMGE	2 Bulls, 2 Cows A, E right; M, G misplaced
    //NOTE TONE    2 Bulls, 2 Cows O, E right; T, N misplaced

    internal class Task_09
    {
        public static void Run()
        {
            const string secretWord = "GAME";
            Console.WriteLine("Clue: It's a 4-letter word!");

            int attempts = 0;

            while (true)
            {
                Console.Write("Enter your guess: ");
                string? guess = Console.ReadLine()?.ToUpper().Trim();
                attempts++;

                if (guess == null || guess.Length != 4)
                {
                    Console.WriteLine("Invalid input. Please enter exactly 4 letters.");
                    continue;
                }

                List<char> bulls = new List<char>();
                List<char> cows = new List<char>();

                bool[] usedSecret = new bool[4];
                bool[] usedGuess = new bool[4];

                // Counting bulls
                for (int i = 0; i < 4; i++)
                {
                    if (guess[i] == secretWord[i])
                    {
                        bulls.Add(guess[i]);
                        usedSecret[i] = true;
                        usedGuess[i] = true;
                    }
                }
                // Counting cows
                for (int i = 0; i < 4; i++)
                {
                    if (usedGuess[i])  // skip if already correct(bull)
                        continue;

                    for (int j = 0; j < 4; j++)
                    {
                        if (!usedSecret[j] && guess[i] == secretWord[j])
                        {
                            cows.Add(guess[i]);
                            usedSecret[j] = true;
                            break;
                        }
                    }
                }

                Console.WriteLine($"{guess} - {bulls.Count} Bulls, " +
                  $"{cows.Count} Cows" +
                  $"{(bulls.Count > 0 ? $"  {string.Join("", bulls)} in correct position" : "")}" +
                  $"{(cows.Count > 0 ? $", {string.Join("", cows)} misplaced" : "")}");

                if (bulls.Count == 4)
                {
                    Console.WriteLine($"You found the secret word in {attempts} attempt(s)!");
                    break;
                }
            }

        }
    }
}
