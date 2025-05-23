using SecureFileAccessSystem.Core;
using SecureFileAccessSystem.Helpers;
using SecureFileAccessSystem.Interfaces;
using SecureFileAccessSystem.Models;

namespace SecureFileAccessSystem
{
    class Program
    {
        static Dictionary<string, User> users = new Dictionary<string, User>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. Read File");
                Console.WriteLine("3. Exit");
                Console.Write("\nEnter choice: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddUser();
                        break;

                    case "2":
                        PerformFileOperation();
                        break;

                    case "3":
                        Console.WriteLine("Exiting program...");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        static void AddUser()
        {
            string username = InputHelper.ReadNonEmptyString("\nEnter username: ");

            string roleInput = InputHelper.ReadNonEmptyString("Enter role (Admin/User/Guest): ");

            if (!Enum.TryParse<Role>(roleInput, true, out Role role))
            {
                Console.WriteLine("Invalid role. Must be Admin, User, or Guest.");
                return;
            }

            users[username.ToLower()] = new User(username, role);
            Console.WriteLine($"\nUser '{username}' with role '{role}' added successfully.");
        }


        static void PerformFileOperation()
        {
            string username = InputHelper.ReadNonEmptyString("\nEnter your username: ");

            if (!users.ContainsKey(username.ToLower()))
            {
                Console.WriteLine("User not found. Please add the user first.");
                return;
            }

            User currentUser = users[username.ToLower()];
            IFile proxyFile = new ProxyFile("C:\\Users\\moumithar\\Documents\\Training\\Day15\\SecureFileAccessSystem\\ConfidentialFile.txt", currentUser);
            Console.WriteLine($"\nUser: {currentUser.UserName} | Role: {currentUser.Role}");
            proxyFile.Read();
        }


    }

}
