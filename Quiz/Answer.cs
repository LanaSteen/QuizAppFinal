using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Answer
    {
        
            public int AnswerId { get; set; }
            public int QuestionId { get; set; }
            public string AnswerText { get; set; }
            public bool IsCorrect { get; set; }

            // Default constructor (optional)
            public Answer() { }

            // Constructor with parameters
            public Answer(int questionId, string answerText, bool isCorrect)
            {
                QuestionId = questionId;
                AnswerText = answerText;
                IsCorrect = isCorrect;
            }
        
    }

}
