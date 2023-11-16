using System;
using System.Collections.Generic;
using TheHangMan;  // Ensure that the correct namespace is used for the User class
using HangTheMan;

namespace TheHangMan
{
    public static class ViewPlayersMethod
    {

        private static List<User> users; // Declare it here

        //Drawing the hangman-status
        public static void ViewPlayers()
        {
            users = UserUtilityMethod.LoadUsers();  // Use the class name to call the method

            Console.Clear();
            Console.WriteLine("-------------");
            Console.WriteLine("PLAYERS:");
            Console.WriteLine("-------------");
            if (users.Count == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("The user list is empty.");
            }
            else
            {
                int Number = 1;
                foreach (var user in users)
                {
                    Console.WriteLine($"[{Number}] {user.Name} - Money:{user.Money}, Lives:{user.Lives}");
                    Number++; //Adding 1 to each message when printing them in console
                }
            }
            Console.WriteLine("");
            Console.Write("Press enter to return to menu");
            Console.ReadLine();

        }
    }
}