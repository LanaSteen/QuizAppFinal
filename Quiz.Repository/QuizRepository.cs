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


                foreach (var question in quizToDelete.Questions)
                {
                    var answersToDelete = answerRepo.LoadAnswers().Where(a => a.QuestionId == question.QuestionId).ToList();
                    foreach (var answer in answersToDelete)
                    {
                        answerRepo.DeleteAnswer(answer.AnswerId);  
                    }
                    questionRepo.DeleteQuestion(question.QuestionId);  
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


        public void DeleteQuestion(int questionId)
        {
            var questions = LoadQuestions();
            var questionToDelete = questions.FirstOrDefault(q => q.QuestionId == questionId);

            if (questionToDelete != null)
            {
          
                questions.Remove(questionToDelete);

           
                SaveQuestions(questions);

                Console.WriteLine("Question deleted successfully!");
            }
            else
            {
                Console.WriteLine("Question not found.");
            }
        }


        public void DeleteAnswer(int answerId)
        {
            var answers = LoadAnswers();
            var answerToDelete = answers.FirstOrDefault(a => a.AnswerId == answerId);

            if (answerToDelete != null)
            {
              
                answers.Remove(answerToDelete);

              
                SaveAnswers(answers);

                Console.WriteLine("Answer deleted successfully!");
            }
            else
            {
                Console.WriteLine("Answer not found.");
            }
        }


        private List<Question> LoadQuestions()
        {
            if (!File.Exists(_questionFilePath))
            {
                return new List<Question>(); 
            }

            using (var fileStream = new FileStream(_questionFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var jsonData = reader.ReadToEnd();
                try
                {
                    return JsonSerializer.Deserialize<List<Question>>(jsonData) ?? new List<Question>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing questions: {ex.Message}");
                    return new List<Question>();  
                }
            }
        }


        private void SaveQuestions(List<Question> questions)
        {
            using (var fileStream = new FileStream(_questionFilePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                var json = JsonSerializer.Serialize(questions, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }

    
        private List<Answer> LoadAnswers()
        {
            if (!File.Exists(_answerFilePath))
            {
                return new List<Answer>(); 
            }

            using (var fileStream = new FileStream(_answerFilePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var jsonData = reader.ReadToEnd();
                try
                {
                    return JsonSerializer.Deserialize<List<Answer>>(jsonData) ?? new List<Answer>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing answers: {ex.Message}");
                    return new List<Answer>();  
                }
            }
        }


        private void SaveAnswers(List<Answer> answers)
        {
            using (var fileStream = new FileStream(_answerFilePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                var json = JsonSerializer.Serialize(answers, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }
    }
}
