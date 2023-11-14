using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
// using System.Text.Json;


namespace HM
{
    class Program
    {

        public static List<wordBank> words = new List<wordBank>();
        public static List<User> users = new List<User>();
        static List<string> guessesLetter = new List<string>(); // Move the list declaration outside the method
        const string FilePathWord = "json/hangmanwordbank.json"; //filename for storing words
        const string FilePathUsers = "json/users.json"; //filename for storing users



        static void Main(string[] args)
        {

            bool exit = false;

            do
            {
                Console.Clear();
                Console.WriteLine($"--------------");
                Console.WriteLine($"WELCOME!");
                Console.WriteLine($"");
                Console.WriteLine($"1. Play Hangman");
                Console.WriteLine($"2. Add new word");
                Console.WriteLine($"3. Players");
                Console.WriteLine($"4. Hangman rules");
                Console.WriteLine($"5. Exit");
                Console.WriteLine($"");
                Console.WriteLine($"--------------");

                Console.Write("Request: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayAllUsers();
                        break;

                    case "2":
                        AddWord();
                        break;

                    case "3":
                        ViewPlayers();
                        break;

                    case "4":
                        //    Rules();
                        break;

                    case "5":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Exiting.");
                        Environment.Exit(0);

                        break;
                }
            } while (!exit);





            static void PlayHangman(string userName, int userLives, int userMoney)
            {


                int bet = Bets();

                if (bet > 0)
                {
                    List<User> users = LoadUsers();
                    User currentUser = users.Find(user => user.Name == userName);

                    if (currentUser != null)
                    {
                        currentUser.Money -= bet;
                        SaveUsers(users);
                    }
                }

                //START APP WITH CONSOLE CLEAR 
                Console.Clear();
                //Load word 
                string theWord = LoadHangmanWord().ToLower();
                //convert loaded word to underscored string
                string currentState = new string('_', theWord.Length);

                int Lives = userLives;
                int Money = userMoney;

                Console.WriteLine($"----------------------");
                Console.WriteLine("THE GAME HAS STARTED!");
                Console.WriteLine($"----------------------");
                Console.WriteLine($"Player: {userName}, Money: {userMoney} - You have bet {bet} USD ");
                Console.WriteLine($"");
                Console.WriteLine("Enter your first guess.");
                Console.WriteLine($"");
                Console.WriteLine($"{currentState}");
                Console.WriteLine($"");

                DrawLives(Lives);
                Console.WriteLine($"Lives: {Lives}");
                Console.WriteLine($"");


                while (currentState.Length > 0)
                {

                    if (currentState != null)
                    {

                        //USER GUESSING
                        Console.Write("Enter a letter: ");
                        string? let1 = Console.ReadLine();

                        //if let1 is not null and include a letter
                        if (let1 != null && let1.Length == 1 && Char.IsLetter(let1[0]))
                        {

                            //if user guess a aldready guessed letter
                            if (guessesLetter.Contains(let1))
                            {
                                Console.Clear();
                                Console.WriteLine($"----------------------");
                                Console.WriteLine($"You have aldready guessed '{let1.ToUpper()}'. Guess again. ");
                                Console.WriteLine($"----------------------");
                                ShowCurrentGameStatus(currentState);

                            }
                            else
                            {

                                char let1_ = Char.ToLower(let1[0]);
                                if (theWord.IndexOf(let1_, StringComparison.OrdinalIgnoreCase) != -1)

                                {
                                    currentState = correctGuess(let1, currentState, ref theWord);
                                }
                                else
                                {
                                    wrongGuess(let1, ref currentState);
                                    Lives--;
                                }

                                //Show amount of lives left to user
                                DrawLives(Lives);
                                Console.WriteLine($"Lives: {Lives}");
                                Console.WriteLine($"");
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine($"----------------------");
                            Console.WriteLine("Please enter a valid single letter.");
                            Console.WriteLine($"----------------------");
                            ShowCurrentGameStatus(currentState);
                        }
                    }
                    else
                    {
                        return;
                    }


                    // if (userInput.Length == userInput_)
                    if (currentState == theWord)
                    {
                        Console.Clear();
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"Congratulations! You won! The correct word was: '{theWord.ToUpper()}'.");
                        Console.WriteLine($"----------------------");

                        if (bet > 0)
                        {
                            User currentUser = users.Find(user => user.Name == userName);

                            if (currentUser != null)
                            {
                                currentUser.Money += bet;
                                SaveUsers(users);
                            }
                        }

                        List<User> users_update = LoadUsers();
                        User currentUser_update = users.Find(user => user.Name == userName);

                        Console.WriteLine($"You won {bet} USD!");
                        Console.WriteLine($"Player: {currentUser_update.Name}");
                        Console.WriteLine($"Money: {currentUser_update.Money} USD");
                        Console.WriteLine($"Lives: {currentUser_update.Lives}");
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"Press enter to return to menu");
                        Console.ReadLine();
                        break;
                    }
                    else if (Lives == 0)
                    {

                        GameOver(theWord, currentState, ref Lives, bet, userName);

                    }
                }
            }




            //****METHODS****

            static void ViewPlayers()
            {
                users = LoadUsers();

                Console.WriteLine("PLAYERS:");
                Console.WriteLine("---------");
                int Number = 1;
                foreach (var user in users)
                {
                    Console.WriteLine($"[{Number}] {user.Name} - Money:{user.Money}, Lives:{user.Lives}");
                    Number++; //Adding 1 to each message when printing them in console
                }

                Console.WriteLine("");
                Console.WriteLine("Press enter to return to menu");
                Console.ReadLine();

            }

            //Add a new user when user presses 'N' at start
            static void AddNewUser()
            {
                Console.Write("Enter your name: ");
                string? name = Console.ReadLine();

                while (true)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        List<User> allUsers = LoadUsers(); // Load existing users
                        User newUser = new User(name, 10, 7); // Create a new user
                        allUsers.Add(newUser); // Add the new user to the list
                        SaveUsers(allUsers); // Save the updated list to the json file

                        Console.WriteLine("User has been added and saved to users.json");


                        List<User> users = LoadUsers(); // Load list after saving
                        // Find index of the added user
                        int index = users.FindIndex(user => user.Name == name);
                        SelectedPlayer(index);

                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid name.");
                    }
                    break;
                }
            }

            //Loads alla users in users LIST - User
            static List<User> LoadUsers()
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
            static void SaveUsers(List<User> userList)
            {
                string json = JsonConvert.SerializeObject(userList, Formatting.Indented);
                File.WriteAllText(FilePathUsers, json);
            }


            //Displays all users from List
            static void DisplayAllUsers()
            {

                users = LoadUsers();

                Console.WriteLine("PLAYERS:");
                Console.WriteLine("---------");
                int Number = 1;
                foreach (var user in users)
                {
                    Console.WriteLine($"[{Number}] {user.Name} - Money:{user.Money}, Lives:{user.Lives}");
                    Number++; //Adding 1 to each message when printing them in console
                }

                Console.WriteLine("");
                Console.WriteLine("Select a player by entering the corresponding number");
                Console.WriteLine("Or press 'N' to create a new player");
                Console.Write("Choose: ");
                string? choice = Console.ReadLine();

                if (!String.IsNullOrEmpty(choice) && (choice == "N" || choice == "n"))
                {
                    AddNewUser();
                }
                else if (int.TryParse(choice, out int index) && index <= users.Count)
                {
                    SelectedPlayer(index - 1); // Adjust the iplayer of the chosen index
                }
            }

            //Send selected player with data to Hangman-Game
            static void SelectedPlayer(int index)
            {
                Console.WriteLine($"You selected: {users[index].Name}");
                // Update Lives with the lives from the selected user
                int Lives = users[index].Lives;
                // Update money with the money from the selected user
                int Money = users[index].Money;
                // Update name with the money from the selected user
                string Name = users[index].Name;

                PlayHangman(Name, Lives, Money);
            }







            //Loads hangmanword from json-file
            static string LoadHangmanWord()
            {
                if (File.Exists(FilePathWord))
                {
                    List<wordBank> randomword = LoadWords();
                    Random r = new Random();

                    if (randomword.Count > 0)
                    {
                        string randomWord = randomword[r.Next(0, randomword.Count)].Word;
                        return randomWord;
                    }
                    else
                    {
                        Console.WriteLine("There are no words available in this difficulty at the moment. Try another one.");
                    }
                }
                else
                {
                    Console.WriteLine("ERROR 404, There is no file.");
                }
                return null;
            }


            //Saves a word after putting it in
            static void SaveData(List<wordBank> data)
            {
                string json = JsonConvert.SerializeObject(data);
                File.WriteAllText(FilePathWord, json);
            }



            // Load existing words from the file
            static List<wordBank> LoadWords()
            {
                if (File.Exists(FilePathWord))
                {
                    string json = File.ReadAllText(FilePathWord);
                    return JsonConvert.DeserializeObject<List<wordBank>>(json) ?? new List<wordBank>();
                }
                else
                {
                    return new List<wordBank>();
                }
            }



            //adds a word from user input 
            static void AddWord()
            {

                Console.Write("Add a new word: ");
                string newWord = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newWord))
                {

                    List<wordBank> existingWords = LoadWords();

                    if (!existingWords.Any(w => w.Word.Equals(newWord, StringComparison.OrdinalIgnoreCase)))
                    {
                        wordBank word = new wordBank(newWord);
                        existingWords.Add(word);
                        SaveData(existingWords);
                        Console.WriteLine("Word added successfully!");
                        Console.WriteLine("Press enter to return to menu");
                        Console.ReadLine();


                    }
                    else
                    {
                        Console.WriteLine("The word already exists. Please enter a different word.");
                    }

                }
                else
                {
                    Console.WriteLine("Invalid input. The word cannot be empty.");
                }
            }



            //if user guess correct letter
            static string correctGuess(string let1, string currentState, ref string theWord)
            {

                // Add a line break after printing the guessed words
                Console.Clear();
                Console.WriteLine($"-------------------------------------");
                Console.WriteLine($"YES!!! The letter '{let1.ToUpper()}' is correct!");

                guessesLetter.Add(let1);
                char[] guessesLetterArray = guessesLetter.Select(s => s[0]).ToArray();

                // Console Guessed words
                Console.WriteLine($"Guessed words:");
                foreach (var guess in guessesLetterArray)
                {
                    Console.Write($"{char.ToUpper(guess)} "); // Convert to capital letters
                }
                // Add a line break after printing the guessed words
                Console.WriteLine();

                char[] updatedState = currentState.ToCharArray();

                for (int i = 0; i < theWord.Length; i++)
                {
                    if (theWord[i] == let1[0])
                    {
                        updatedState[i] = let1[0];
                    }

                }
                string updatedStateString = new string(updatedState);

                // userInput = userInput.Replace(let1, "_");

                Console.WriteLine($"");
                Console.WriteLine(updatedStateString.ToUpper());
                Console.WriteLine($"");

                // userInput = LoadWord(userInput);
                return updatedStateString;


            }
            //if user guess wrong letter
            static void wrongGuess(string let1, ref string currentState)
            {
                Console.Clear();
                Console.WriteLine($"-------------------------------------");
                Console.WriteLine($"NO!!! The letter '{let1.ToUpper()}' is not correct!");
                // var guessesLetter = new List<string>();
                guessesLetter.Add(let1);
                char[] guessesLetterArray = guessesLetter.Select(s => s[0]).ToArray();

                // Console Guessd words
                Console.WriteLine($"Guessed words:");
                foreach (var guess in guessesLetterArray)
                {
                    Console.Write($"{char.ToUpper(guess)} "); // Convert to capital letters
                }
                // Add a line break after printing the guessed words
                Console.WriteLine();

                Console.WriteLine($"");
                Console.WriteLine(currentState.ToUpper());
                Console.WriteLine($"");

            }

            //if user looses game!
            static void GameOver(string theWord, string currentState, ref int Lives, int bet, string userName)
            {
                while (true)
                {


                    Console.Clear();
                    Console.WriteLine($"----------------------");
                    Console.WriteLine($"GAME OVER!");
                    Console.WriteLine($"----------------------");
                    DrawLives(Lives);
                    Console.WriteLine($"Do you want to buy more lives?");
                    Console.Write($"Y/N : ");
                    string? purchase = Console.ReadLine();

                    if (!String.IsNullOrEmpty(purchase) && purchase == "y" || purchase == "Y")
                    {
                        Console.WriteLine(" ");
                        Console.WriteLine($"THE LIFE SHOP");
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"1 life = 1 USD");
                        Console.Write($"How many lives to you want to purchase? ");
                        string? purchasedLives = Console.ReadLine();
                        int.TryParse(purchasedLives, out int newLives);
                        Lives = newLives;

                        Console.Clear();
                        Console.WriteLine($"You have {newLives} new lives.");

                        Lives = newLives;
                        GetMoreLives(currentState);
                        break;

                    }
                    else if (!String.IsNullOrEmpty(purchase) && purchase == "n" || purchase == "N")
                    {
                        Console.Clear();
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"THE END! Thanks for playing.");
                        Console.WriteLine($"The correct word was: '{theWord.ToUpper()}'.");
                        Console.WriteLine($"----------------------");

                        User currentUser = users.Find(user => user.Name == userName);
                        currentUser.Money -= bet;

                        Console.WriteLine($"You lost {bet} USD!");
                        Console.WriteLine($"Player: {currentUser.Name}");
                        Console.WriteLine($"Money: {currentUser.Money} USD");
                        Console.WriteLine($"Lives: {currentUser.Lives}");
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"Press enter to exit.");
                        Console.ReadLine();
                        Environment.Exit(0);
                        break;

                    }
                }
            }


            static void ShowCurrentGameStatus(string currentState)
            {

                char[] guessesLetterArray = guessesLetter.Select(s => s[0]).ToArray();

                // Console Guessd words
                Console.WriteLine($"Guessed words:");
                foreach (var guess in guessesLetterArray)
                {
                    Console.Write($"{char.ToUpper(guess)} "); // Convert to capital letters
                }
                // Add a line break after printing the guessed words
                Console.WriteLine();

                Console.WriteLine($"");
                Console.WriteLine(currentState.ToUpper());
                Console.WriteLine($"");

            }



            //make user pay for more lives
            static void GetMoreLives(string currentState)
            {
                char[] guessesLetterArray = guessesLetter.Select(s => s[0]).ToArray();

                // Console Guessd words
                Console.WriteLine("Guessed words:");
                foreach (var guess in guessesLetterArray)
                {
                    Console.Write($"{char.ToUpper(guess)} "); // Convert to capital letters
                }
                // Add a line break after printing the guessed words
                Console.WriteLine();

                Console.WriteLine($"");
                Console.WriteLine(currentState.ToUpper());
                Console.WriteLine($"");

            }



            //make user bet before start
            static int Bets()
            {

                while (true)
                {
                    Console.Write("Do you want to make a bet? Y/N? ");
                    string? input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input) && input == "y" || input == "Y")
                    {
                        Console.Write("How much (USD)? ");
                        int bets = Convert.ToInt16(Console.ReadLine());
                        Console.Clear();
                        return bets;

                    }
                    else if (!string.IsNullOrEmpty(input) && input == "n" || input == "N")
                    {
                        Console.WriteLine("No bets.");
                        int bets = 0;
                        Console.Clear();
                        return bets;

                    }
                }
            }



            static void DrawLives(int Lives)
            {
                switch (Lives)
                {
                    case 5:
                        Console.WriteLine("   -------------------");
                        Console.WriteLine("   |  /              |");
                        Console.WriteLine("   | /        ");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("----------------------------       ");
                        break;

                    case 4:
                        Console.WriteLine("   -------------------");
                        Console.WriteLine("   |  /              |");
                        Console.WriteLine("   | /              ('')");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("----------------------------       ");
                        break;

                    case 3:
                        Console.WriteLine("   -------------------");
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
                        Console.WriteLine("   -------------------");
                        Console.WriteLine("   |  /              |");
                        Console.WriteLine("   | /              ('')");
                        Console.WriteLine("   |                /|| ");
                        Console.WriteLine("   |                 ||");
                        Console.WriteLine("   |                /   ");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("----------------------------       ");
                        break;
                    case 1:
                        Console.WriteLine("   -------------------      __________________                     ");
                        Console.WriteLine("   |  /              |      |                |   ");
                        Console.WriteLine("   | /              (**) ---|  LAST CHANCE!  |   ");
                        Console.WriteLine("   |                /||     |________________|   ");
                        Console.WriteLine("   |                 ||              ");
                        Console.WriteLine("   |                / ");
                        Console.WriteLine("   |");
                        Console.WriteLine("   |");
                        Console.WriteLine("----------------------------       ");
                        break;
                    case 0:
                        Console.WriteLine("   -------------------");
                        Console.WriteLine("   |  /              |   ");
                        Console.WriteLine("   | /               |   ");
                        Console.WriteLine("   |                 |  ");
                        Console.WriteLine("   |                (--) ");
                        Console.WriteLine("   |                /||   ");
                        Console.WriteLine("   |                 ||      ");
                        Console.WriteLine("   |                 /           ");
                        Console.WriteLine("----------------------------       ");
                        break;

                    default:
                        Console.WriteLine("   -------------------");
                        Console.WriteLine("   |  /   ");
                        Console.WriteLine("   | /   ");
                        Console.WriteLine("   |    ");
                        Console.WriteLine("   |  ");
                        Console.WriteLine("   | ");
                        Console.WriteLine("   | ");
                        Console.WriteLine("   |   ");
                        Console.WriteLine("----------------------------       ");
                        break;
                }
            }
        }
    }
}



