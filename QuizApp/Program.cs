using Quiz.Models;
using Quiz.Repository;

internal class Program
{
    static string dataFolder = @"C:\Users\l4nst\source\repos\QuizAppFinal\Quiz.Repository\Data\";
    static QuizRepository quizRepo = new QuizRepository(dataFolder);
    static QuestionRepository questionRepo = new QuestionRepository(dataFolder);
    static AnswerRepository answerRepo = new AnswerRepository(dataFolder);
    static PlayerRepository playerRepo = new PlayerRepository(dataFolder); 

    static void Main(string[] args)
    {
        Directory.CreateDirectory(dataFolder);
        var authRepo = new AuthRepository(dataFolder);

        Console.WriteLine("Please enter your username:");
        string username = Console.ReadLine();

        Console.WriteLine("Please enter your password:");
        string password = Console.ReadLine();

        var loginUser = new AuthUser
        {
            Username = username,
            PasswordHash = PasswordHelper.HashPassword(password)
        };

        var existingUser = authRepo.GetAuthUserByUsername(username);

        if (existingUser != null)
        {
            if (existingUser.PasswordHash == PasswordHelper.HashPassword(password))
            {
                Console.WriteLine($"{existingUser.Username} - WELCOME BACK!");
                authRepo.SaveAuthUser(existingUser, PasswordHelper.HashPassword(password));
            }
            else
            {
                Console.WriteLine("Incorrect password! Please try again.");
            }
        }
        else
        {
            Console.WriteLine($"{loginUser.Username} - Nice to see you in our game!");
            authRepo.SaveAuthUser(loginUser, PasswordHelper.HashPassword(password));
            existingUser = authRepo.GetAuthUserByUsername(username);  

        }

        var player = playerRepo.LoadPlayers().FirstOrDefault(p => p.UserId == existingUser.UserId);
        if (player == null)
        {
            player = new Player
            {
                UserId = existingUser.UserId,
                Name = existingUser.Username,
                BestScore = 0
            };
            playerRepo.AddOrUpdatePlayer(player);
        }

        Console.WriteLine("Do you want to play an existing quiz (1), create your own quiz (0), delete a quiz (2), or update a quiz (3)?");
        int choice = Convert.ToInt32(Console.ReadLine());

        if (choice == 0)
        {
            CreateQuiz(existingUser);
        }
        else if (choice == 1)
        {
            PlayQuiz(player);
        }
        else if (choice == 2)
        {
            DeleteQuiz(existingUser);
        }
        else if (choice == 3)
        {
            UpdateQuiz(existingUser);
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void CreateQuiz(AuthUser logedinUser)
    {
        string quizName;
        bool isQuizNameUnique;

        do
        {
            Console.WriteLine("Enter the name of your quiz:");
            quizName = Console.ReadLine();

            var existingQuizzes = quizRepo.LoadQuizzes();
            isQuizNameUnique = !existingQuizzes.Any(q => q.Name.Equals(quizName, StringComparison.OrdinalIgnoreCase));

            if (!isQuizNameUnique)
            {
                Console.WriteLine("This quiz name already exists. Please choose a different name.");
            }
        } while (!isQuizNameUnique);

        var newQuiz = new Quize
        {
            QuizId = quizRepo.LoadQuizzes().Any() ? quizRepo.LoadQuizzes().Max(q => q.QuizId) + 1 : 0,
            Name = quizName,
            UserId = logedinUser.UserId,
            QuestionIds = new List<int>()
        };

        for (int i = 0; i < 2; i++)  
        {
            var newQuestion = CreateQuestion(newQuiz.QuizId);
            newQuiz.QuestionIds.Add(newQuestion.QuestionId);
        }

        quizRepo.SaveQuiz(newQuiz);
        Console.WriteLine("Quiz saved successfully!");
    }

    static void DeleteQuiz(AuthUser logedinUser)
    {
        var userQuizzes = quizRepo.LoadQuizzes().Where(q => q.UserId == logedinUser.UserId).ToList();

        if (userQuizzes.Count == 0)
        {
            Console.WriteLine("You do not have any quizzes to delete.");
            return;
        }

        Console.WriteLine("Your quizzes:");
        foreach (var quiz in userQuizzes)
        {
            Console.WriteLine($"QuizId: {quiz.QuizId}, Name: {quiz.Name}");
        }

        Console.WriteLine("Enter the QuizId of the quiz you want to delete:");
        int quizIdToDelete = Convert.ToInt32(Console.ReadLine());

        var quizToDelete = userQuizzes.FirstOrDefault(q => q.QuizId == quizIdToDelete);

        if (quizToDelete != null)
        {
            var questionsToDelete = questionRepo.LoadQuestions().Where(q => quizToDelete.QuestionIds.Contains(q.QuestionId)).ToList();
            foreach (var question in questionsToDelete)
            {
                var answersToDelete = answerRepo.LoadAnswers().Where(a => question.AnswerIds.Contains(a.AnswerId)).ToList();
                foreach (var answer in answersToDelete)
                {
                    answerRepo.DeleteAnswer(answer.AnswerId);  
                }
                questionRepo.DeleteQuestion(question.QuestionId);  
            }

            quizRepo.DeleteQuiz(quizIdToDelete);  
            Console.WriteLine("Quiz deleted successfully!");
        }
        else
        {
            Console.WriteLine("Quiz not found.");
        }
    }

    static void UpdateQuiz(AuthUser logedinUser)
    {
        var userQuizzes = quizRepo.LoadQuizzes().Where(q => q.UserId == logedinUser.UserId).ToList();

        if (userQuizzes.Count == 0)
        {
            Console.WriteLine("You do not have any quizzes to update.");
            return;
        }

        Console.WriteLine("Your quizzes:");
        foreach (var quiz in userQuizzes)
        {
            Console.WriteLine($"QuizId: {quiz.QuizId}, Name: {quiz.Name}");
        }

        Console.WriteLine("Enter the QuizId of the quiz you want to update:");
        int quizIdToUpdate = Convert.ToInt32(Console.ReadLine());

        var quizToUpdate = userQuizzes.FirstOrDefault(q => q.QuizId == quizIdToUpdate);

        if (quizToUpdate != null)
        {

            DeleteQuizAndAssociatedData(quizToUpdate.QuizId);

       
            Console.WriteLine("Enter the new quiz name (or press enter to keep the current name):");
            string newQuizName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newQuizName))
            {
                quizToUpdate.Name = newQuizName;
            }

           
            var newQuiz = new Quize
            {
                QuizId = quizRepo.LoadQuizzes().Any() ? quizRepo.LoadQuizzes().Max(q => q.QuizId) + 1 : 0,
                Name = quizToUpdate.Name,
                UserId = logedinUser.UserId,
                QuestionIds = new List<int>()
            };

            for (int i = 0; i < quizToUpdate.QuestionIds.Count; i++)
            {
                var newQuestion = CreateQuestion(newQuiz.QuizId); 
                newQuiz.QuestionIds.Add(newQuestion.QuestionId);
            }

            quizRepo.SaveQuiz(newQuiz);  
            Console.WriteLine("Quiz updated successfully!");
        }
        else
        {
            Console.WriteLine("Quiz not found.");
        }
    }

    static void DeleteQuizAndAssociatedData(int quizId)
    {
       
        var quizToDelete = quizRepo.LoadQuizzes().FirstOrDefault(q => q.QuizId == quizId);

        if (quizToDelete != null)
        {
            
            var questionsToDelete = questionRepo.LoadQuestions().Where(q => quizToDelete.QuestionIds.Contains(q.QuestionId)).ToList();
            foreach (var question in questionsToDelete)
            {
               
                var answersToDelete = answerRepo.LoadAnswers().Where(a => question.AnswerIds.Contains(a.AnswerId)).ToList();
                foreach (var answer in answersToDelete)
                {
                    answerRepo.DeleteAnswer(answer.AnswerId);
                }
                questionRepo.DeleteQuestion(question.QuestionId); 
            }

         
            quizRepo.DeleteQuiz(quizId);  
            Console.WriteLine("Old quiz and associated data deleted successfully.");
        }
    }
    static Question CreateQuestion(int quizId)
    {
        Console.WriteLine("Enter the question text:");
        string questionText = Console.ReadLine();

        var newQuestion = new Question
        {
            QuestionText = questionText,
            AnswerIds = new List<int>()  
        };

        bool correctAnswerAssigned = false;

        for (int i = 0; i < 4; i++)  
        {
            var newAnswer = CreateAnswer(ref correctAnswerAssigned);  

            if (newAnswer != null)
            {
                newQuestion.AnswerIds.Add(newAnswer.AnswerId);  
            }
            else
            {
                Console.WriteLine("Skipping this answer due to validation failure.");
                i--;  
            }
        }

        questionRepo.SaveQuestion(newQuestion);  
        quizRepo.AddQuestionToQuiz(quizId, newQuestion.QuestionId);  

        return newQuestion;
    }

    static Answer CreateAnswer(ref bool correctAnswerAssigned)
    {
        Console.WriteLine("Enter the answer text:");
        string answerText = Console.ReadLine();

        bool isCorrect = GetTrueFalseInput("Is this the correct answer? (y/n): ");

        if (isCorrect && correctAnswerAssigned)
        {
            Console.WriteLine("Only one correct answer is allowed per question.");
            return null;  
        }

        if (isCorrect)
        {
            correctAnswerAssigned = true; 
        }

        int answerId = answerRepo.LoadAnswers().Any() ? answerRepo.LoadAnswers().Max(a => a.AnswerId) + 1 : 1;

        var newAnswer = new Answer(answerId, answerText, isCorrect);

        answerRepo.SaveAnswer(newAnswer);  
        return newAnswer;
    }

    static bool GetTrueFalseInput(string prompt)
    {
        string userInput;
        do
        {
            Console.WriteLine(prompt);
            userInput = Console.ReadLine().ToLower();

            if (userInput == "y")
                return true;
            else if (userInput == "n")
                return false;
            else
                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
        } while (true);
    }


    static void PlayQuiz(Player player)
    {
        var availableQuizzes = quizRepo.LoadQuizzes().Where(q => q.UserId != player.UserId).ToList();  
        if (availableQuizzes.Count == 0)
        {
            Console.WriteLine("There are no available quizzes to play.");
            return;
        }

        Console.WriteLine("Select a quiz to play:");
        for (int i = 0; i < availableQuizzes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {availableQuizzes[i].Name}");
        }
        int selectedQuizIndex = Convert.ToInt32(Console.ReadLine()) - 1;

        var selectedQuiz = availableQuizzes[selectedQuizIndex];
        var questions = questionRepo.LoadQuestions().Where(q => selectedQuiz.QuestionIds.Contains(q.QuestionId)).ToList();

        int score = 0;
        var startTime = DateTime.Now;
        TimeSpan timeLimit = TimeSpan.FromMinutes(2);

        foreach (var question in questions)
        {
            Console.WriteLine(question.QuestionText);
            var answers = answerRepo.LoadAnswers().Where(a => question.AnswerIds.Contains(a.AnswerId)).ToList();

            foreach (var answer in answers)
            {
                Console.WriteLine($"{answer.AnswerId}. {answer.AnswerText}");
            }

            Console.WriteLine("Enter the number of your answer:");
            int userAnswer = Convert.ToInt32(Console.ReadLine());

            var selectedAnswer = answers.FirstOrDefault(a => a.AnswerId == userAnswer);

            if (selectedAnswer != null && selectedAnswer.IsCorrect)
            {
                score += 20;
            }

            if (DateTime.Now - startTime > timeLimit)
            {
                Console.WriteLine("Time is up! You did not complete the quiz in time.");
                break;
            }
        }

     
        player.BestScore =player.BestScore + score;
        playerRepo.AddOrUpdatePlayer(player);  

        Console.WriteLine($"Your score: {score}.");
        Console.WriteLine($"Your total score: {player.BestScore}.");
    }

    

}
