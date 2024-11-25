using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Answer
    {

        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }

        public Answer() { }

        public Answer(int answerId, string answerText, bool isCorrect)
        {
            AnswerId = answerId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }
    }

}
