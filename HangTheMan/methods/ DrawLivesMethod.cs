using System;

namespace HangTheMan.methods
{
    public static class DrawLivesMethod
    {
        //Drawing the hangman-status
        public static void DrawLives(int Lives)
        {
            switch (Lives)
            {
                case 10:
                    Console.WriteLine("   ");
                    Console.WriteLine("                ");
                    Console.WriteLine("         ");
                    Console.WriteLine("   ");
                    Console.WriteLine("   ");
                    Console.WriteLine("   ");
                    Console.WriteLine("   ");
                    Console.WriteLine("   ");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 9:
                    Console.WriteLine("   ");
                    Console.WriteLine("                ");
                    Console.WriteLine("         ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 8:
                    Console.WriteLine("   ");
                    Console.WriteLine("   |              ");
                    Console.WriteLine("   |        ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 7:
                    Console.WriteLine("   |");
                    Console.WriteLine("   |  /              ");
                    Console.WriteLine("   | /        ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 6:
                    Console.WriteLine("   |-------------------");
                    Console.WriteLine("   |  /              ");
                    Console.WriteLine("   | /        ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 5:
                    Console.WriteLine("   |-------------------");
                    Console.WriteLine("   |  /               |");
                    Console.WriteLine("   | /        ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 4:
                    Console.WriteLine("   |-------------------");
                    Console.WriteLine("   |  /               |");
                    Console.WriteLine("   | /               ('')");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 3:
                    Console.WriteLine("   |-------------------");
                    Console.WriteLine("   |  /               |");
                    Console.WriteLine("   | /               ('')");
                    Console.WriteLine("   |                 /|| ");
                    Console.WriteLine("   |                  ||");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;

                case 2:
                    Console.WriteLine("   |-------------------");
                    Console.WriteLine("   |  /               |");
                    Console.WriteLine("   | /               ('')");
                    Console.WriteLine("   |                 /|| ");
                    Console.WriteLine("   |                  ||");
                    Console.WriteLine("   |                 /   ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;
                case 1:
                    Console.WriteLine("   |-------------------                           ");
                    Console.WriteLine("   |  /               |      |--------------|   ");
                    Console.WriteLine("   | /               (**) ---| LAST CHANCE! |   ");
                    Console.WriteLine("   |                 /||     |______________|   ");
                    Console.WriteLine("   |                  ||              ");
                    Console.WriteLine("   |                 / ");
                    Console.WriteLine("   |");
                    Console.WriteLine("   |");
                    Console.WriteLine("----------------------------       ");
                    break;
                case 0:
                    Console.WriteLine("   |-------------------");
                    Console.WriteLine("   |  /               |   ");
                    Console.WriteLine("   | /                |   ");
                    Console.WriteLine("   |                  |  ");
                    Console.WriteLine("   |                 (--) ");
                    Console.WriteLine("   |                 /||   ");
                    Console.WriteLine("   |                  ||      ");
                    Console.WriteLine("   |                  /           ");
                    Console.WriteLine("----------------------------       ");

                    break;

                default:
                    Console.WriteLine("   ");
                    Console.WriteLine("      ");
                    Console.WriteLine("      ");
                    Console.WriteLine("       ");
                    Console.WriteLine("     ");
                    Console.WriteLine("    ");
                    Console.WriteLine("    ");
                    Console.WriteLine("      ");
                    Console.WriteLine("----------------------------       ");
                    break;
            }
        }
    }
}