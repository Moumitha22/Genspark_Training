namespace Day_11
{
    internal class Task_04
    {
    //4) Take username and password from user.Check if user name is "Admin" and password is "pass" if yes then print success message.
    //Give 3 attempts to user.In the end of eh 3rd attempt if user still is unable to provide valid creds then exit the application after print the message
    //"Invalid attempts for 3 times. Exiting...."

        static bool Authenticate(string username, string password)
        {
            const string adminUsername = "Admin";
            const string adminPassword = "pass";

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password cannot be empty.");
                return false;
            }

            if (!(username == adminUsername && password == adminPassword))
            {
                Console.WriteLine("Invalid username or password.");
                return false;
            }
            return true;

        }
        public static void Run()
        {
            int maxAttempts = 3;
            int currentAttempt = 0;

            while (currentAttempt < maxAttempts)
            {
                Console.Write("Enter user name: ");
                string? name = Console.ReadLine();

                Console.Write("\nEnter password: ");
                string? password = Console.ReadLine();

                if (Authenticate(name, password))
                {
                    Console.WriteLine("Login successful! Welcome Admin.");
                    return;
                }
                else
                {
                    currentAttempt++;
                    Console.WriteLine($"Attempts left: {maxAttempts - currentAttempt}");
                }
            }
            Console.WriteLine("Invalid attempts for 3 times. Exiting....");
        }
    }
    
}
