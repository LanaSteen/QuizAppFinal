using Quiz.Models;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace Quiz.Repository
{
    public class QuizRepository
    {
        private readonly string _filePath;
        private readonly string _questionFilePath;
        private readonly string _answerFilePath;

        public QuizRepository(string dataFolder)
        {
            _filePath = Path.Combine(dataFolder, "quiz.json");
            _questionFilePath = Path.Combine(dataFolder, "question.json");
            _answerFilePath = Path.Combine(dataFolder, "answer.json");
        }

        public void SaveQuiz(Quize quiz)
        {
            var quizzes = LoadQuizzes();

            quiz.QuizId = quizzes.Any() ? quizzes.Max(q => q.QuizId) + 1 : 0;
            quizzes.Add(quiz);

            SaveToFile(quizzes);
        }

        public List<Quize> LoadQuizzes()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Quize>();
            }

            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var jsonData = reader.ReadToEnd();
                try
                {
                    return JsonSerializer.Deserialize<List<Quize>>(jsonData) ?? new List<Quize>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing quizzes: {ex.Message}");
                    return new List<Quize>();
                }
            }
        }

        private void SaveToFile<T>(T data)
        {
            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }

        public void DeleteQuiz(int quizId)
        {
            var quizzes = LoadQuizzes();
            var quizToDelete = quizzes.FirstOrDefault(q => q.QuizId == quizId);

            if (quizToDelete != null)
            {
                var questionRepo = new QuestionRepository(_questionFilePath);
                var answerRepo = new AnswerRepository(_answerFilePath);

                foreach (var questionId in quizToDelete.QuestionIds)
                {
                    var question = questionRepo.LoadQuestions().FirstOrDefault(q => q.QuestionId == questionId);
                    if (question != null)
                    {
                        foreach (var answerId in question.AnswerIds)
                        {
                            var answer = answerRepo.LoadAnswers().FirstOrDefault(a => a.AnswerId == answerId);
                            if (answer != null)
                            {
                                answerRepo.DeleteAnswer(answer.AnswerId);
                            }
                        }
                        questionRepo.DeleteQuestion(question.QuestionId);
                    }
                }

                quizzes.Remove(quizToDelete);
                SaveToFile(quizzes);

                Console.WriteLine("Quiz and its associated questions and answers deleted successfully!");
            }
            else
            {
                Console.WriteLine("Quiz not found.");
            }
        }
        public void AddQuestionToQuiz(int quizId, int questionId)
        {
            var quiz = LoadQuizzes().FirstOrDefault(q => q.QuizId == quizId);

            if (quiz != null)
            {
                quiz.QuestionIds.Add(questionId);  // Add the QuestionId to the Quiz's list of QuestionIds
                SaveQuiz(quiz);  // Save the updated quiz
            }
        }
    }

}
