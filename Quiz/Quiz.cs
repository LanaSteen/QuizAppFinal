using MiniBank.Models;
using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Quiz 
    {
        public Guid QuizId { get; set; } 
        public string Name { get; set; }  
        public List<Question> Questions { get; set; }  

        public Quiz()
        {
            Questions = new List<Question>();  
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
