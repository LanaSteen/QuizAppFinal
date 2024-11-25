
using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Quize 
    {
     
            public int QuizId { get; set; }
            public string Name { get; set; }
            public int UserId { get; set; }

            public List<int> QuestionIds { get; set; }

            public Quize()
            {
                QuestionIds = new List<int>();
            }
        
    }
   
}
