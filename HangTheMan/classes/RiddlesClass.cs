using System;

namespace TheHangMan
{

    [Serializable]

    public class Riddle
    {
        //properties
        public string Question { get; set; }
        public string Answer { get; set; }

         public string Keyword { get; set; }


        //Custructor -- Constructors have the same name as the class and do not have a return type.
        public Riddle(string question, string answer, string keyword)
        {
            Question = question;
            Answer = answer;
            Keyword = keyword;

        }

    }
}