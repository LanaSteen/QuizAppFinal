using Quiz.Models;
using Quiz.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application
{
    public class CommonService
    {
        private QuizRepository quizRepo;
        private QuestionRepository questionRepo;
        private AnswerRepository answerRepo;
        private PlayerRepository playerRepo;

        public CommonService(string dataFolder)
        {
            quizRepo = new QuizRepository(dataFolder);
            questionRepo = new QuestionRepository(dataFolder);
            answerRepo = new AnswerRepository(dataFolder);
            playerRepo = new PlayerRepository(dataFolder);
        }

        public void CreateQuiz(AuthUser logedinUser)
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

            for (int i = 0; i < 2; i++) // TODO Limit to 2 questions for now to ttest need to be five latter
            {
                var newQuestion = CreateQuestion(newQuiz.QuizId);
                newQuiz.QuestionIds.Add(newQuestion.QuestionId);
            }

            quizRepo.SaveQuiz(newQuiz);
            Console.WriteLine("Quiz saved successfully!");
        }

        public void DeleteQuiz(AuthUser logedinUser)
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
                if (quizToDelete.UserId != logedinUser.UserId)
                {
                    Console.WriteLine("You cannot delete a quiz that is not yours.");
                    return;
                }

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

        public void UpdateQuiz(AuthUser logedinUser)
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
                if (quizToUpdate.UserId != logedinUser.UserId)
                {
                    Console.WriteLine("You cannot update a quiz that is not yours.");
                    return;
                }

                Console.WriteLine("Enter the new quiz name (or press enter to keep the current name):");
                string newQuizName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newQuizName))
                {
                    quizToUpdate.Name = newQuizName;
                }

       
                bool updateMoreQuestions = true;
                while (updateMoreQuestions)
                {
                    Console.WriteLine("Do you want to update any questions? (yes/no)");
                    string updateQuestionResponse = Console.ReadLine()?.Trim().ToLower();

                    if (updateQuestionResponse == "yes")
                    {
                       
                        Console.WriteLine("Current questions in the quiz:");
                        for (int i = 0; i < quizToUpdate.QuestionIds.Count; i++)
                        {
                            var question = questionRepo.LoadQuestions().FirstOrDefault(q => q.QuestionId == quizToUpdate.QuestionIds[i]);
                            if (question != null)
                            {
                                Console.WriteLine($"QuestionId: {question.QuestionId}, Text: {question.QuestionText}");
                            }
                        }

                  
                        Console.WriteLine("Enter the QuestionId of the question you want to update:");
                        int questionIdToUpdate = Convert.ToInt32(Console.ReadLine());

                        var questionToUpdate = questionRepo.LoadQuestions().FirstOrDefault(q => q.QuestionId == questionIdToUpdate);

                        if (questionToUpdate != null)
                        {
                          
                            Console.WriteLine("Do you want to delete this question and its answers? (yes/no)");
                            string deleteResponse = Console.ReadLine()?.Trim().ToLower();

                            if (deleteResponse == "yes")
                            {
                                DeleteQuestionAndAssociatedAnswers(questionToUpdate.QuestionId);
                            }

                          
                            var newQuestion = CreateQuestion(quizToUpdate.QuizId);
                            quizToUpdate.QuestionIds.Add(newQuestion.QuestionId);
                            Console.WriteLine($"Question {newQuestion.QuestionId} updated successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Question not found.");
                        }
                    }
                    else
                    {
                        updateMoreQuestions = false; 
                    }
                }

                quizRepo.SaveQuiz(quizToUpdate);
                Console.WriteLine("Quiz updated successfully!");
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }
        }

        private void DeleteQuestionAndAssociatedAnswers(int questionId)
        {
            var questionToDelete = questionRepo.LoadQuestions().FirstOrDefault(q => q.QuestionId == questionId);

            if (questionToDelete != null)
            {
                var answersToDelete = answerRepo.LoadAnswers().Where(a => questionToDelete.AnswerIds.Contains(a.AnswerId)).ToList();
                foreach (var answer in answersToDelete)
                {
                    answerRepo.DeleteAnswer(answer.AnswerId);
                }

                questionRepo.DeleteQuestion(questionId);
                Console.WriteLine("Question and its answers deleted successfully.");
            }
        }
       
        private Question CreateQuestion(int quizId)
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

        private Answer CreateAnswer(ref bool correctAnswerAssigned)
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

        private bool GetTrueFalseInput(string prompt)
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

        public void PlayQuiz(Player player)
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
            bool timeIsUp = false; 
            TimeSpan timeLimit = TimeSpan.FromMinutes(0.5); //TODO time should be 2 minutes
            var timerService = new TimerService(timeLimit);
            timerService.StartTimer();  

            foreach (var question in questions)
            {
                
                if (timerService.IsTimeUp())
                {
                    Console.WriteLine("Time is up! You did not complete the quiz in time.");
                    timeIsUp = true;  
                    break;  
                }

               
                Console.WriteLine(question.QuestionText);
                var answers = answerRepo.LoadAnswers().Where(a => question.AnswerIds.Contains(a.AnswerId)).ToList();

                
                foreach (var answer in answers)
                {
                    Console.WriteLine($"{answer.AnswerId}. {answer.AnswerText}");
                }

              
                if (timerService.IsTimeUp())
                {
                    Console.WriteLine("Time is up! You did not complete the quiz in time.");
                    timeIsUp = true; 
                    break; 
                }

                Console.WriteLine("Enter the number of your answer:");
                int userAnswer = Convert.ToInt32(Console.ReadLine());

              
                if (timerService.IsTimeUp())
                {
                    Console.WriteLine("Time is up! You did not complete the quiz in time.");
                    timeIsUp = true;  
                    break; 
                }
                var selectedAnswer = answers.FirstOrDefault(a => a.AnswerId == userAnswer);

                if (selectedAnswer != null && selectedAnswer.IsCorrect)
                {
                    score += 20;
                }
              
                timerService.DisplayTimeLeft();
            }

            if (!timeIsUp)
            {
                player.BestScore += score;
                playerRepo.AddOrUpdatePlayer(player);
            }

            Console.WriteLine($"Your score: {score}.");
            Console.WriteLine($"Your total score: {player.BestScore}.");
        }

    }
}
