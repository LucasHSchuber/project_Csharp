using System;
using TheHangMan;

namespace TheHangMan
{
    public static class LifeAsHeartMethod
    {
        //Drawing the hangman-status
         public static void LivesAsHeart(int amount)
            {

                for (int i = 0; i < amount; i++)
                {
                    Console.Write("❤ ");
                }
            }
    }
}