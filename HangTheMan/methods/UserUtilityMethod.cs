using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TheHangMan
{
    public static class UserUtilityMethod
    {
        private const string FilePathUsers = "json/users.json";

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

        //Saves a user after creating one
        public static void SaveUsers(List<User> userList)
        {
            string json = JsonConvert.SerializeObject(userList, Formatting.Indented);
            File.WriteAllText(FilePathUsers, json);
        }


        //delete user
        public static void removeUser(string userName)
        {
            List<User> users = UserUtilityMethod.LoadUsers(); // Load existing users


            // Find the user with the specified userName
            User userToRemove = users.Find(user => user.Name == userName);
            if (userToRemove != null)
            {
                // Remove the user from the list
                users.Remove(userToRemove);
                // Save the updated list to the JSON file
                SaveUsers(users);
                Console.WriteLine($"User '{userName}' has been removed from the game.");
            }
            else
            {
                Console.WriteLine($"User '{userName}' not found.");
            }
        }

    }
}
