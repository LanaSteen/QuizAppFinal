using Quiz.Models;
using Quiz.Repository;
using System;

namespace QuizApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataFolder = @"C:\Users\l4nst\source\repos\QuizAppFinal\Quiz.Repository\Data\";
            Directory.CreateDirectory(dataFolder);

            var authRepo = new AuthRepository(dataFolder);

            Console.WriteLine("Please enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Please enter your password:");
            string password = Console.ReadLine();

            var loginUser = new AuthUser
            {
                Username = username,
                PasswordHash = password
            };

            var existingUser = authRepo.GetAuthUserByUsername(username);

            if (existingUser != null)
            {
                if (existingUser.PasswordHash == password)
                {
                    Console.WriteLine($"{existingUser.Username} - WELCOME BACK!");
                    authRepo.SaveAuthUser(existingUser, password);
                }
                else
                {
                    Console.WriteLine("Incorrect password! Please try again.");
                }
            }
            else
            {
                Console.WriteLine($"{loginUser.Username} - Nice to see you in our game!");
                authRepo.SaveAuthUser(loginUser, password);
            }

            var allUsers = authRepo.LoadAuthUsers();
            Console.WriteLine("All users in the system:");
            foreach (var u in allUsers)
            {
                Console.WriteLine($"UserId: {u.UserId}, Username: {u.Username}, ActiveNow: {u.ActiveNow}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
