using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HM
{
    public static class UserMethods
    {
        private static readonly string FilePathUsers = "path/to/your/file.json"; // Replace with the actual path

        // Loads all users in the users LIST - User
        public static List<User> LoadUsers()
        {
            if (File.Exists(FilePathUsers))
            {
                string json = File.ReadAllText(FilePathUsers);
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            else
            {
                return new List<User>();
            }
        }

        // Saves a user after creating one
        public static void SaveUsers(List<User> userList)
        {
            string json = JsonConvert.SerializeObject(userList, Formatting.Indented);
            File.WriteAllText(FilePathUsers, json);
        }
    }
}
