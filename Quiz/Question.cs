using System.Diagnostics.CodeAnalysis;

namespace MiniBank.Models
{
    public class Question
    {
        public Guid QuestionId { get; set; } 
        public string QuestionText { get; set; } 
        public List<Answer> Answers { get; set; }  

        public Question()
        {
            Answers = new List<Answer>();  
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
