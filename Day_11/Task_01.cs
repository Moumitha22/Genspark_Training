using System;

namespace Day_11
{
    // 1) create a program that will take name from user and greet the user
    internal class Task_01
    {
        static void GreetUser(string name)
        {
            if (!string.IsNullOrEmpty(name.Trim()))
            {
                Console.WriteLine($"Hi, {name}! Have a great day!");
            }
            else
            {
                Console.WriteLine("\nEnter a valid name.");
            }
        }
        public static void Run()
        {
            Console.Write("Enter your name : ");
            string? name = Console.ReadLine();
            GreetUser(name);
        }
    }
}
