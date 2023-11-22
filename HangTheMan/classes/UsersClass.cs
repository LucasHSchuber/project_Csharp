using System;
using TheHangMan;  // Ensure that the correct namespace is used for the User class
using HangTheMan;
using HangTheMan.methods;
namespace TheHangMan
{

    [Serializable]

    public class User
    {
        public string Name { get; set; }
        public int Money { get; set; }
        public int Lives { get; set; }


        //Custructor -- Constructors have the same name as the class and do not have a return type.
        public User(string name, int money, int lives)
        {
            Name = name;
            Money = money;
            Lives = lives;
        }

    }
}