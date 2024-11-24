using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Player : AuthUser
    {
        public string Name { get; set; } 
        public int BestScore { get; set; }  
        public List<Quize> QuizzesCreated { get; set; } 

        public Player()
        {
            QuizzesCreated = new List<Quize>(); 
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
