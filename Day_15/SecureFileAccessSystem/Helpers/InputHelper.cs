using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileAccessSystem.Helpers
{
    public class InputHelper
    {
        public static string ReadNonEmptyString(string prompt, string errorMsg = "Input cannot be empty.")
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                Console.WriteLine(errorMsg);
            }
        }


    }

}
