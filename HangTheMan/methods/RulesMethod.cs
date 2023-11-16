using System;

namespace HangTheMan.methods
{
    public static class RulesMethod
    {
        //Drawing the hangman-status
        public static void Rules()
        {
            Console.Clear();
            Console.WriteLine("-----------------------");
            Console.WriteLine("RULES");
            Console.WriteLine("-----------------------");
            Console.WriteLine("BASICS:");
            Console.WriteLine("------:");
            Console.WriteLine("   To play the Hangman game you going to need money and lives.");
            Console.WriteLine("   If you run out of lives, you can always buy new lives for the money.");
            Console.WriteLine("BETTING:");
            Console.WriteLine("------:");
            Console.WriteLine("   To earn more money, you need to bet before the Hangman game.");
            Console.WriteLine("   You will either loose or win the betting amount depending on if you win or loose the Hangmsn sequence.");
            Console.WriteLine("WINNIGS:");
            Console.WriteLine("------:");
            Console.WriteLine("   You win money by winning a Hangman sequence.");
            Console.WriteLine("   For level easy = 6 USD, for medium = 8 USD, for hard = 10 USD.");
            Console.WriteLine("");
            Console.WriteLine("GOOD LUCK!");
            Console.WriteLine("-----------------------");


            Console.WriteLine("");
            Console.Write("Press enter to return to menu");
            Console.ReadLine();

        }
    }
}