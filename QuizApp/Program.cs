using Quiz.Models;
using Quiz.Repository;

namespace QuizApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Define the directory where your data is stored
            string dataFolder = @"C:\Users\l4nst\source\repos\QuizAppFinal\Quiz.Repository\Data\"; // Adjust this path if necessary
            Directory.CreateDirectory(dataFolder); // Ensure the directory exists

            var authRepo = new AuthRepository(dataFolder);

            // Read the username and password from the console
            Console.WriteLine("Please enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Please enter your password:");
            string password = Console.ReadLine(); // In a real-world app, you'd hash this password

            // Create an AuthUser object with the entered data
            var loginUser = new AuthUser
            {
                Username = username,
                PasswordHash = password // In practice, you should hash the password
            };

            // Check if the user already exists
            var existingUser = authRepo.GetAuthUserByUsername(username);

            if (existingUser != null)
            {
                // If the user exists, it's a re-login
                Console.WriteLine($"{existingUser.Username} - WELCOME BACK!");
            }
            else
            {
                // If the user doesn't exist, it's a new log-in
                Console.WriteLine($"{loginUser.Username} - Nice to see you in our game!");
            }

            // Save the user (create a new one or update the existing one)
            authRepo.SaveAuthUser(loginUser);

            // Optionally, display all users to verify ActiveNow status
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
