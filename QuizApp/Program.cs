using Quiz.Models;
using Quiz.Repository;

internal class Program
{
    static string dataFolder = @"C:\Users\l4nst\source\repos\QuizAppFinal\Quiz.Repository\Data\";
    static QuizRepository quizRepo = new QuizRepository(dataFolder);
    static QuestionRepository questionRepo = new QuestionRepository(dataFolder);
    static AnswerRepository answerRepo = new AnswerRepository(dataFolder);

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
        }

        Console.WriteLine("Do you want to play an existing quiz (1), create your own quiz (0), delete a quiz (2), or update a quiz (3)?");
        int choice = Convert.ToInt32(Console.ReadLine());

        if (choice == 0)
        {
            CreateQuiz(existingUser);
        }
        else if (choice == 1)
        {
            // Logic to play an existing quiz
            //PlayQuiz(existingUser);
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
            Questions = new List<Question>()
        };

        for (int i = 0; i < 2; i++)  
        {
            var newQuestion = CreateQuestion(newQuiz.QuizId); 
            newQuiz.Questions.Add(newQuestion);
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
        
            var questionsToDelete = questionRepo.LoadQuestions().Where(q => q.QuizId == quizToDelete.QuizId).ToList();
            foreach (var question in questionsToDelete)
            {
                var answersToDelete = answerRepo.LoadAnswers().Where(a => a.QuestionId == question.QuestionId).ToList();
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
            Console.WriteLine("Your questions in this quiz:");
            foreach (var question in quizToUpdate.Questions)
            {
                Console.WriteLine($"QuestionId: {question.QuestionId}, Text: {question.QuestionText}");
            }

            Console.WriteLine("Enter the QuestionId of the question you want to update:");
            int questionIdToUpdate = Convert.ToInt32(Console.ReadLine());

            var questionToUpdate = quizToUpdate.Questions.FirstOrDefault(q => q.QuestionId == questionIdToUpdate);
            if (questionToUpdate != null)
            {
                Console.WriteLine("Enter the new question text:");
                questionToUpdate.QuestionText = Console.ReadLine();
                questionRepo.SaveQuestion(questionToUpdate);  
            }

            Console.WriteLine("Your answers for this question:");
            foreach (var answer in questionToUpdate.Answers)
            {
                Console.WriteLine($"AnswerId: {answer.AnswerId}, Text: {answer.AnswerText}, Correct: {answer.IsCorrect}");
            }

            Console.WriteLine("Enter the AnswerId of the answer you want to update:");
            int answerIdToUpdate = Convert.ToInt32(Console.ReadLine());

            var answerToUpdate = questionToUpdate.Answers.FirstOrDefault(a => a.AnswerId == answerIdToUpdate);
            if (answerToUpdate != null)
            {
                Console.WriteLine("Enter the new answer text:");
                answerToUpdate.AnswerText = Console.ReadLine();
                answerToUpdate.IsCorrect = GetTrueFalseInput("Is this the correct answer? (y/n): ");

                answerRepo.SaveAnswer(answerToUpdate);  
            }

            quizRepo.SaveQuiz(quizToUpdate); 
            Console.WriteLine("Quiz updated successfully!");
        }
        else
        {
            Console.WriteLine("Quiz not found.");
        }
    }

    static Question CreateQuestion(int quizId)
    {
        Console.WriteLine("Enter the question text:");
        string questionText = Console.ReadLine();

        var newQuestion = new Question
        {
            QuizId = quizId,  
            QuestionText = questionText,
            Answers = new List<Answer>()
        };

        bool correctAnswerAssigned = false;  

        for (int i = 0; i < 4; i++) 
        {
            var newAnswer = CreateAnswer(newQuestion.QuestionId, ref correctAnswerAssigned); 

            if (newAnswer != null) 
            {
                newQuestion.Answers.Add(newAnswer);
            }
            else
            {
                Console.WriteLine("Skipping this answer due to validation failure.");
                i--; 
            }
        }

        questionRepo.SaveQuestion(newQuestion);
        return newQuestion;
    }

    static Answer CreateAnswer(int questionId, ref bool correctAnswerAssigned)
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

       
        var newAnswer = new Answer(questionId, answerText, isCorrect)
        {
            AnswerId = answerId  
        };

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
}
