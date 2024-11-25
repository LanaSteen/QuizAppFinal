using System.Diagnostics.CodeAnalysis;

namespace Quiz.Models
{
    public class Question
    {
      
            public int QuestionId { get; set; }
            public string QuestionText { get; set; }
            public List<int> AnswerIds { get; set; }

            public Question()
            {
                AnswerIds = new List<int>();
            }
        
    }

}
