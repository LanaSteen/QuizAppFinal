using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class AuthUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }


        private string _passwordHash;

        public bool? ActiveNow { get; set; }
        public DateTime LastLogin { get; set; }


        public void SetPassword(string password)
        {

            _passwordHash = PasswordHelper.HashPassword(password);
        }


        public string GetPasswordHash()
        {
            return _passwordHash;
        }
    }
}
