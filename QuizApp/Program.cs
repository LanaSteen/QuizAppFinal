using Application;
using Quiz.Models;
using Quiz.Repository;

internal class Program
{
    static string dataFolder = @"C:\Users\l4nst\source\repos\QuizAppFinal\Quiz.Repository\Data\";
    static CommonService commonService = new CommonService(dataFolder);

    static void Main(string[] args)
    {
        var authRepo = new AuthRepository(dataFolder);

        Console.WriteLine("Please enter your username:");
        string username = Console.ReadLine();

        Console.WriteLine("Please enter your password:");
        string password = Console.ReadLine();

        var loginUser = new AuthUser
        {
            Username = username,
            PasswordHash = PasswordHelper.HashPassword(password),
            ActiveNow = true,  
            LastLogin = DateTime.Now 
        };

        var existingUser = authRepo.GetAuthUserByUsername(username);


        if (existingUser != null)
        {
            if (existingUser.PasswordHash == PasswordHelper.HashPassword(password))
            {
                Console.WriteLine($"{existingUser.Username} - WELCOME BACK!");
                existingUser.ActiveNow = true; 
                existingUser.LastLogin = DateTime.Now; 
                authRepo.SaveAuthUser(existingUser, existingUser.PasswordHash);
            }
            else
            {
                Console.WriteLine("Incorrect password! Please try again.");
                return; 
            }
        }
        else
        {
   
            Console.WriteLine($"{loginUser.Username} - Nice to see you in our game!");
            authRepo.SaveAuthUser(loginUser, loginUser.PasswordHash);
            existingUser = authRepo.GetAuthUserByUsername(username);
        }


        var player = new PlayerRepository(dataFolder).LoadPlayers().FirstOrDefault(p => p.UserId == existingUser.UserId)
            ?? new Player
            {
                UserId = existingUser.UserId,
                Username = existingUser.Username,
                BestScore = 0,
                QuizzesCreated = null,
                ActiveNow = null,
                LastLogin = existingUser.LastLogin
            };

     
        if (player.BestScore == 0)
        {
            new PlayerRepository(dataFolder).AddOrUpdatePlayer(player);
        }


        bool exitGame = false;
        while (!exitGame)
        {
     
            Console.WriteLine("Do you want to play an existing quiz (1), create your own quiz (0), delete a quiz (2), or update a quiz (3)?");
            int choice;

      
            bool validChoice = int.TryParse(Console.ReadLine(), out choice);
            if (!validChoice || choice < 0 || choice > 3)
            {
                Console.WriteLine("Invalid option. Please select a valid option (0, 1, 2, or 3).");
                continue; 
            }


            switch (choice)
            {
                case 0:
                    commonService.CreateQuiz(existingUser);
                    break;
                case 1:
                    commonService.PlayQuiz(player);
                    break;
                case 2:
                    commonService.DeleteQuiz(existingUser);
                    break;
                case 3:
                    commonService.UpdateQuiz(existingUser);
                    break;
            }

            Console.WriteLine("Do you want to perform another action? (yes/no)");
            string answer = Console.ReadLine()?.Trim().ToLower();

            if (answer != "yes")
            {
                exitGame = true;  
            }
        }

 
        Console.WriteLine("Thank you! Press any key to exit...");
        Console.ReadKey();
    }
}