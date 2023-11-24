// using System;

// namespace TheHangMan
// {

//     [Serializable] 

//     public class wordBank
//     {
//         //properties
//         public string Word { get; set; }
//         public string Category { get; set; }

//         //Custructor -- Constructors have the same name as the class and do not have a return type.
//         public wordBank(string word, string category)
//         {
//             Word = word;
//             Category = category;
            
//         }

//     }
// }

using System;

namespace TheHangMan
{
    [Serializable] 
    public class wordBank
    {
        // Private fields
        private string word;
        private string category;

        // Properties
        public string Word
        {
            get { return word; }
            set { word = value; }
        }

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        // Constructor
        public wordBank(string word, string category)
        {
            Word = word;
            Category = category;
        }
    }
}
