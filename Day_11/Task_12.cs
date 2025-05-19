using System;
using System.Text.RegularExpressions;

namespace Day_11

{
    //12) Write a program that:
    //Takes a message string as input(only lowercase letters, no spaces or symbols).
    //Encrypts it by shifting each character forward by 3 places in the alphabet.
    //Decrypts it back to the original message by shifting backward by 3.
    //Handles wrap-around, e.g., 'z' becomes 'c'.
    //Examples
    //Input:     hello
    //Encrypted: khoor
    //Decrypted: hello
    //-------------
    //Input:     xyz
    //Encrypted: abc
    //Test cases
    //| Input | Shift | Encrypted | Decrypted |
    //| ----- | ----- | --------- | --------- |
    //| hello | 3     | khoor     | hello     |
    //| world | 3     | zruog     | world     |
    //| xyz   | 3     | abc       | xyz       |
    //| apple | 1     | bqqmf     | apple     |
    internal class Task_12
    {
        // Encrypt by shifting forward
        // Eg:
        // char(((('a' - 'a') + 3) % 26 ) + 'a') => char((((97 - 97) + 3) % 26) + 97) => char( 3 + 97 ) => char(100) = 'd'
        // char(((('z' - 'a') + 3) % 26 ) + 'a') => char((((122 - 97) + 3) % 26) + 97) => char((28%26) + 97 ) => char( 2 + 97 ) => char(99) = 'c'

        static string Encrypt(string input, int shift)
        {
            char[] result = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch >= 'a' && ch <= 'z')
                {
                    result[i] = (char)((((ch - 'a') + shift) % 26) + 'a');
                }
                else
                {
                    throw new ArgumentException("Only lowercase letters without spaces or symbols are allowed.");
                }
            }

            return new string(result);
        }

        // Decrypt by shifting backward
        // Eg: If shifted forward 3 times for encryption, shifting backwords 3 times gives decrypted message
        static string Decrypt(string input, int shift)

        {
            return Encrypt(input, 26 - (shift % 26)); 
        }

        public static void Run()
        {
            Console.Write("Enter a lowercase message (no spaces/symbols): ");
            string? message = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(message) && !Regex.IsMatch(message, @"^[a-z]+$"))
            {
                Console.WriteLine("Enter a valid input message");
                message = Console.ReadLine();
            }

            int shift;
            Console.Write("Enter shift amount (e.g. 3): ");
            while (!int.TryParse(Console.ReadLine(), out shift))
            {
                Console.WriteLine("Invalid shift amount. Please enter a positive integer: ");
            }

            string encrypted = Encrypt(message, shift);
            string decrypted = Decrypt(encrypted, shift);

            Console.WriteLine($"\nMessage: {message}");
            Console.WriteLine($"Encrypted: {encrypted}");
            Console.WriteLine($"Decrypted: {decrypted}");

        }
    }
}
