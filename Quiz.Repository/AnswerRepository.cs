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

        // Save a new answer with a dynamically generated AnswerId
        public void SaveAnswer(Answer answer)
        {
            var answers = LoadAnswers();  // Load existing answers

            // Generate the next AnswerId (incrementing from the max existing AnswerId)
            answer.AnswerId = answers.Any() ? answers.Max(a => a.AnswerId) + 1 : 0;

            answers.Add(answer);

            // Save the updated list of answers to the file
            SaveToFile(answers);
        }

        // Load all answers from the file
        public List<Answer> LoadAnswers()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Answer>();  // Return an empty list if the file doesn't exist
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
                    return new List<Answer>();  // Return an empty list on error
                }
            }
        }

        // Delete an answer by its AnswerId
        public void DeleteAnswer(int answerId)
        {
            var answers = LoadAnswers();
            var answerToDelete = answers.FirstOrDefault(a => a.AnswerId == answerId);

            if (answerToDelete != null)
            {
                // Remove the answer from the list
                answers.Remove(answerToDelete);

                // Save the updated list of answers back to the file
                SaveToFile(answers);

                Console.WriteLine($"Answer with ID {answerId} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Answer with ID {answerId} not found.");
            }
        }

        // Helper method to save data to the file (for answers)
        private void SaveToFile<T>(T data)
        {
            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                writer.Write(json);
            }
        }

        // Generic method to load data from a file (for general usage)
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
            return default;  // Return default if the file doesn't exist
        }
    }
}
