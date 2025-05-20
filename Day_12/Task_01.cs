using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_12
{
    class Post
    {
        public string Caption { get; set; }
        public int Likes { get; set; }
        
    }
    class Task_01
    {
        static int GetValidUserCount()
        {
            int userCount;
            while (!int.TryParse(Console.ReadLine(), out userCount) || userCount <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid number:");
            }
            return userCount;
        }
        static int GetValidPostCount()
        {
            int postCount;
            while (!int.TryParse(Console.ReadLine(), out postCount) || postCount < 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid number of posts (0 or more):");
            }
            return postCount;
        }

        static int GetValidLikes()
        {
            int likes;
            while (!int.TryParse(Console.ReadLine(), out likes) || likes < 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid number of likes (0 or more):");
            }
            return likes;
        }

        static string GetValidCaption()
        {
            string caption;
            do
            {
                caption = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(caption))
                {
                    Console.WriteLine("Invalid caption. Please enter a non-empty caption:");
                }
            } while (string.IsNullOrWhiteSpace(caption));
            return caption;
        }

        static Post[][] GetUserPosts(int userCount)
        {
            Post[][] userPosts = new Post[userCount][];

            for (int i = 0; i < userCount; i++)
            {
                Console.Write($"\nUser {i + 1}: How many posts? ");
                int postCount = GetValidPostCount();
                userPosts[i] = new Post[postCount];

                for (int j = 0; j < postCount; j++)
                {
                    Console.Write($"Enter caption for post {j + 1}: ");
                    string caption = GetValidCaption();

                    Console.Write("Enter likes: ");
                    int likes = GetValidLikes();

                    userPosts[i][j] = new Post{
                       Caption = caption, 
                       Likes = likes 
                    };
                }
            }

            return userPosts;
        }

        static void DisplayUserPosts(Post[][] userPosts)
        {
            Console.WriteLine("\n------ Displaying Instagram Posts ------");
            for (int i = 0; i < userPosts.Length; i++)
            {
                Console.WriteLine($"User {i + 1}:");

                if (userPosts[i].Length == 0)
                {
                    Console.WriteLine("No posts available.");
                }
                else
                {
                    for (int j = 0; j < userPosts[i].Length; j++)
                    {
                        var post = userPosts[i][j];
                        Console.WriteLine($"Post {j + 1} - Caption: {post.Caption} | Likes: {post.Likes}");
                    }
                }

                Console.WriteLine();
            }
        }

        public static void Run()
        {
            Console.Write("Enter number of users: ");
            int userCount = GetValidUserCount();

            Post[][] userPosts = GetUserPosts(userCount);
            DisplayUserPosts(userPosts);
        }

    }
}
