using System.Diagnostics.CodeAnalysis;

namespace MiniBank.Models
{
    public class Answer
    {
        public Guid AnswerId { get; set; }  
        public Guid QuestionId { get; set; }  
        public string AnswerText { get; set; } 
        public bool IsCorrect { get; set; }  

        public Answer(Guid questionId, string answerText, bool isCorrect)
        {
            QuestionId = questionId;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }
    }
    //public class AccountEquilityComparer : IEqualityComparer<Quiz>
    //{
    //    public bool Equals(Quiz x, Quiz y) => x.Id == y.Id &&
    //            x.Iban.Trim().ToLower() == y.Iban.Trim().ToLower() &&
    //            x.Currency.Trim().ToLower() == y.Currency.Trim().ToLower() &&
    //            x.Balance == y.Balance &&
    //            x.CustomerId == y.CustomerId;

    //    public int GetHashCode([DisallowNull] Quiz obj) => obj.Id;
    //}
}
