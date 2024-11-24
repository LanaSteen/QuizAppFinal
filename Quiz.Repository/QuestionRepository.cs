using Quiz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Quiz.Repository
{
    public class QuestionRepository
    {
        private readonly string _filePath;

        public QuestionRepository(string dataFolder)
        {
            _filePath = Path.Combine(dataFolder, "questions.json");
        }


        public void SaveQuestion(Question question)
        {
            var questions = LoadQuestions(); 

    
            question.QuestionId = questions.Any() ? questions.Max(q => q.QuestionId) + 1 : 0;

            questions.Add(question);

    
            SaveToFile(questions);
        }

    
        public List<Question> LoadQuestions()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Question>();  
            }

            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
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


        public void DeleteQuestion(int questionId)
        {
            var questions = LoadQuestions();
            var questionToDelete = questions.FirstOrDefault(q => q.QuestionId == questionId);

            if (questionToDelete != null)
            {

                questions.Remove(questionToDelete);

                
                SaveToFile(questions);

                Console.WriteLine($"Question with ID {questionId} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Question with ID {questionId} not found.");
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

  
        private T LoadFromFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var jsonData = reader.ReadToEnd();
                    return JsonSerializer.Deserialize<T>(jsonData);
                }
            }
            return default; 
        }
    }
}
