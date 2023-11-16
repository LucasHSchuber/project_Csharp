using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TheHangMan
{
    public static class RiddleUtilityMethod
    {
        const string FilePathRiddle = "json/riddles.json"; //filename for storing words



        //Loads alla riddles in users LIST - User
        static List<Riddle> LoadRiddles()
        {
            if (File.Exists(FilePathRiddle))
            {
                string json = File.ReadAllText(FilePathRiddle);
                return JsonConvert.DeserializeObject<List<Riddle>>(json) ?? new List<Riddle>();
            }
            else
            {
                return new List<Riddle>();
            }
        }


        public static Riddle GetRandomRiddle()
        {
            List<Riddle> riddles = LoadRiddles();
            if (riddles.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, riddles.Count);
                return riddles[randomIndex];
            }
            else
            {
                Console.WriteLine("No riddles available.");
                return null;
            }
        }




    }
}
