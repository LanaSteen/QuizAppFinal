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
   
}
