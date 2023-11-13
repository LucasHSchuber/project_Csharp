using System;

namespace HM
{

    [Serializable] 

    public class wordBank
    {
        public string Word { get; set; }

        //Custructor -- Constructors have the same name as the class and do not have a return type.
        public wordBank(string word)
        {
            Word = word;
            
        }

    }
}