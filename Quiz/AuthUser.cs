namespace Quiz.Models
{
    public class AuthUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool? ActiveNow { get; set; }
        public DateTime LastLogin { get; set; }
    }
}