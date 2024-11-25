using Quiz.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Quiz.Repository
{
   
        public class AnswerRepository
        {
            private readonly string _filePath;

            public AnswerRepository(string dataFolder)
            {
                _filePath = Path.Combine(dataFolder, "answers.json");
            }

            public void SaveAnswer(Answer answer)
            {
                var answers = LoadAnswers();
                answer.AnswerId = answers.Any() ? answers.Max(a => a.AnswerId) + 1 : 0;
                answers.Add(answer);
                SaveToFile(answers);
            }

            public List<Answer> LoadAnswers()
            {
                if (!File.Exists(_filePath))
                {
                    return new List<Answer>();
                }

                using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
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

            public void DeleteAnswer(int answerId)
            {
                var answers = LoadAnswers();
                var answerToDelete = answers.FirstOrDefault(a => a.AnswerId == answerId);

                if (answerToDelete != null)
                {
                    answers.Remove(answerToDelete);
                    SaveToFile(answers);
                    Console.WriteLine($"Answer with ID {answerId} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Answer with ID {answerId} not found.");
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
       
        }
}
