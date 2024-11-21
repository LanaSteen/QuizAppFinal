using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class AuthUser
    {
        public int UserId { get; set; }  
        public string Username { get; set; }  
        public string PasswordHash { get; set; }  
        public bool ActiveNow { get; set; }  
        public DateTime LastLogin { get; set; } 
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
