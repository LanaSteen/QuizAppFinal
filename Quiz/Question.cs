using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; } 
        public List<Answer> Answers { get; set; }  

        public Question()
        {
            Answers = new List<Answer>();  
        }
    }

}
