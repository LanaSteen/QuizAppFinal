
using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Quize 
    {
        public int QuizId { get; set; } 
        public string Name { get; set; }
        public int UserId { get; set; }  // UserId of the AuthUser creating the quiz

        public List<Question> Questions { get; set; }  

        public Quize()
        {
            Questions = new List<Question>();  
        }
    }
   
}
