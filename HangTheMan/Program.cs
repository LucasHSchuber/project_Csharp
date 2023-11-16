using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
// using System.Text.Json;


using HangTheMan.methods;


namespace TheHangMan
{
    class Program
    {

        public static List<wordBank> words = new List<wordBank>();
        public static List<User> users = new List<User>();

        public static List<Riddle> riddles = new List<Riddle>();

        static List<string> guessesLetter = new List<string>(); // Move the list declaration outside the method
        const string FilePathWord = "json/hangmanwordbank.json"; //filename for storing words
        const string FilePathUsers = "json/users.json"; //filename for storing users
        const string FilePathRiddle = "json/riddles.json"; //filename for storing words


        static void Main(string[] args)
        {

            bool exit = false;
            do
            {
                DisplayMenu();

                Console.Write("Choose: ");
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
                        RulesMethod.Rules();
                        break;

                    case "5":
                        ExitApp();
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Exiting.");
                        break;
                }
            } while (!exit);





            static void PlayHangman(string userName, int userLives, int userMoney)
            {


                int Lives = userLives;
                int Money = userMoney;

                List<User> users = LoadUsers();
                int check_bought_lives = 0;
                if (userLives <= 0 && userMoney > 0)
                {
                    StartGameWithZeroLives(userName, userLives, userMoney);
                    users = LoadUsers();
                    User userLivesUpd = users.Find(user => user.Name == userName);
                    Lives = userLivesUpd.Lives;
                }
                else if (userLives <= 0 && userMoney <= 0)
                {
                    oneLastChance(userName, userMoney);
                }


                string categoryString;
                int category;

                while (true)
                {
                    Console.WriteLine($"------");
                    Console.WriteLine("Choose a category for the Hangman game sequence");
                    Console.WriteLine("------");
                    Console.WriteLine("[Press 1] Animals");
                    Console.WriteLine("[Press 2] Countries");
                    Console.Write("Choose: ");

                    if (!int.TryParse(Console.ReadLine(), out category) || (category < 1 || category > 2))
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    else
                    {
                        // Convert the chosen category to a string
                        categoryString = (category == 1) ? "Animals" : "Countries";
                        break;
                    }
                }

                // Now, you can use the chosen categoryString in your code.


                int level;
                while (true)
                {
                    Console.WriteLine($"------");
                    Console.WriteLine("Choose a difficulty level for the Hangman game sequence");
                    Console.WriteLine($"------");
                    Console.WriteLine("[Press 1] EASY - win: 6 USD");
                    Console.WriteLine("[Press 2] MEDIUM - win: 8 USD");
                    Console.WriteLine("[Press 3] HARD - win: 10 USD");
                    Console.Write("Choose: ");

                    if (!int.TryParse(Console.ReadLine(), out level))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    if (level < 1 || level > 3)
                    {
                        Console.WriteLine("Invalid difficulty level. Please choose 1 (easy), 2 (medium), or 3 (hard).");
                    }
                    else
                    {
                        break;
                    }
                }

                //LOAD WORD 
                string theWord = LoadHangmanWord(level, categoryString).ToLower();
                //convert loaded word to underscored string
                string currentState = GetInitialState(theWord);
                // Function to get the initial state with spaces
                string GetInitialState(string word)
                {
                    // Replace spaces with spaces, and other characters with underscores
                    string initialState = new string(word.Select(c => (c == ' ') ? ' ' : '_').ToArray());
                    return initialState;
                }
                // string currentState = new string('_', theWord.Length);

                char[] guessesLetterArray = new char[theWord.Length]; // Reset the guessed letters array
                Array.Fill(guessesLetterArray, '_'); // Fill the array with underscores


                users = LoadUsers();
                User updUser = users.Find(user => user.Name == userName);

                //IF USER HAS MONEY MAKE IT AVAILABLE TO BET 
                int bet;
                if (updUser.Money != null && updUser.Money > 0)
                {
                    bet = Bets(userName);
                }
                else //IF NO MONEY
                {
                    bet = 0;
                }

                string levelString;
                int levelPoints;
                if (level == 1)
                {
                    levelString = "Easy";
                    levelPoints = 6;
                }
                else if (level == 2)
                {
                    levelString = "Medium";
                    levelPoints = 8;
                }
                else
                {
                    levelString = "Hard";
                    levelPoints = 10;
                }

                //SHOWING GAME DETAILS BEFORE STARTING GAME
                GameDetails(userName, levelString, categoryString, levelPoints, updUser.Money, bet);
                Console.Write($"Press enter to start the game ");
                Console.ReadLine();

                //GAME HAS STARTED
                Console.Clear();
                Console.WriteLine($"----------------------");
                Console.WriteLine("THE GAME HAS STARTED!");
                Console.WriteLine($"----------------------");
                Console.WriteLine($"");
                Console.WriteLine("Enter your first guess.");
                Console.WriteLine($"");
                Console.WriteLine($"{currentState}");
                Console.WriteLine($"");

                DrawLivesMethod.DrawLives(Lives);
                Console.Write($"Lives: {Lives} ");
                LifeAsHeartMethod.LivesAsHeart(Lives);
                Console.WriteLine($"");
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

                                ShowCurrentGameStatus(currentState, Lives);
                                Console.Write($"Lives: {Lives} ");
                                LifeAsHeartMethod.LivesAsHeart(Lives);
                                Console.WriteLine($"");
                                Console.WriteLine($"");

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
                                DrawLivesMethod.DrawLives(Lives);
                                Console.Write($"Lives: {Lives} ");
                                LifeAsHeartMethod.LivesAsHeart(Lives);
                                Console.WriteLine($"");
                                Console.WriteLine($"");
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine($"----------------------");
                            Console.WriteLine("Please enter a valid single letter.");
                            Console.WriteLine($"----------------------");

                            ShowCurrentGameStatus(currentState, Lives);
                            Console.Write($"Lives: {Lives} ");
                            LifeAsHeartMethod.LivesAsHeart(Lives);
                            Console.WriteLine($"");
                            Console.WriteLine($"");
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
                        Console.WriteLine($"CONGRATULATIONS! You won! The correct word was: '{theWord.ToUpper()}'.");
                        Console.WriteLine($"----------------------");

                        //IF USER HAS MADE A BET - ADD IT TO USER
                        if (bet > 0)
                        {
                            users = LoadUsers();
                            User currentUser___ = users.Find(user => user.Name == userName);

                            if (currentUser___ != null)
                            {
                                currentUser___.Lives = Lives;
                                currentUser___.Money += (bet + bet);
                                SaveUsers(users);
                            }
                        }

                        users = LoadUsers();
                        User currentUser_update = users.Find(user => user.Name == userName);
                        int new_Lives = Lives;
                        currentUser_update.Lives = Lives;
                        currentUser_update.Money += levelPoints;
                        SaveUsers(users);
                        Console.WriteLine($"Winnings:");
                        Console.WriteLine($"For level {levelString.ToUpper()}: {levelPoints} USD.");
                        Console.WriteLine($"Bettings: {bet} USD.");
                        Console.WriteLine($"Total winnings: {bet + levelPoints} USD");
                        Console.WriteLine($"");
                        Console.WriteLine($"Player: {currentUser_update.Name}");
                        Console.WriteLine($"Money: {currentUser_update.Money} USD");
                        Console.WriteLine($"Lives: {new_Lives}");
                        Console.WriteLine($"----------------------");
                        Console.Write($"Press enter to return to menu");
                        Console.ReadLine();
                        // Reset guessesLetter List and guessesLetterArray
                        guessesLetter.Clear();
                        guessesLetterArray = new char[theWord.Length];
                        Array.Fill(guessesLetterArray, '_');
                        currentState = string.Empty;
                        break;
                    }
                    else if (Lives <= 0)
                    {
                        List<User> users_ = LoadUsers(); // Load existing users
                        User user = users_.Find(user => user.Name == userName);

                        if (user != null)
                        {
                            user.Lives = 0;
                            SaveUsers(users_);
                        }
                        Console.Clear();
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"GAME OVER!");
                        Console.WriteLine($"----------------------");
                        DrawLivesMethod.DrawLives(Lives);
                        Console.WriteLine($"");

                        GameOver(theWord, currentState, ref Lives, bet, userName, userMoney);

                    }
                }
            }




            //****METHODS****
            static void DisplayMenu()
            {

                Console.Clear();
                Console.WriteLine($"--------------");
                Console.WriteLine($"WELCOME!");
                Console.WriteLine($"");
                Console.WriteLine($"1. Play Hangman");
                Console.WriteLine($"2. Add new word");
                Console.WriteLine($"3. Players");
                Console.WriteLine($"4. Rules");
                Console.WriteLine($"5. Quit game");
                Console.WriteLine($"");
                Console.WriteLine($"--------------");
            }
            static void GameDetails(string userName, string levelString, string categoryString, int levelPoints, int updUserMoney, int bet)
            {
                Console.Clear();
                Console.WriteLine($"------");
                Console.WriteLine("Game details");
                Console.WriteLine($"------");
                Console.WriteLine($"Player: {userName}");
                Console.WriteLine($"Game level: {levelString}, Category: {categoryString}");
                Console.WriteLine($"Chance to win: {levelPoints} USD");
                Console.WriteLine($"Current money: {updUserMoney} USD");
                Console.WriteLine($"Bettings: {bet} USD ");
                Console.WriteLine($"");
            }

            static void ViewPlayers()
            {
                users = LoadUsers();

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

            //Add a new user when user presses 'N' at start
            static void AddNewUser()
            {

                string name = NewPlayerStoryIntro();

                if (!string.IsNullOrEmpty(name))
                {
                    List<User> allUsers = LoadUsers(); // Load existing users
                    int StarMoney = 20;
                    int StartLives = 7;
                    User newUser = new User(name, StarMoney, StartLives); // Create a new user (name,USD,lives)
                    allUsers.Add(newUser); // Add the new user to the list toghether with the other players
                    SaveUsers(allUsers); // Save the updated list to the json file

                    Console.Clear();
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine($"You have been given {StarMoney} USD and {StartLives} lives to start with. ");
                    Console.WriteLine($"");
                    Console.WriteLine($"Name: {name}");
                    Console.WriteLine($"Money: {StarMoney} USD");
                    Console.WriteLine($"Lives: {StartLives}");


                    users = LoadUsers(); // Load the updated list into the global variable

                    int index = users.FindIndex(user => user.Name == name);
                    Console.WriteLine("");
                    Console.Write("Press enter to continue ");
                    Console.ReadLine();
                    Console.Clear();

                    SelectedPlayer(index);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid name.");
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

                Console.Clear();
                Console.WriteLine("-------------");
                Console.WriteLine("PLAYERS:");
                Console.WriteLine("-------------");
                //Displays all player in game
                int Number = 1;
                foreach (var user in users)
                {
                    Console.WriteLine($"[{Number}] {user.Name} - Money:{user.Money}, Lives:{user.Lives}");
                    Number++; //Adding 1 to each message when printing them in console
                }
                Console.WriteLine("");
                Console.WriteLine("['N'] - Create new player");
                Console.WriteLine("");
                Console.WriteLine("-------------");
                Console.WriteLine("Select a player by entering the corresponding number");
                Console.Write("Choose: ");
                string? choice = Console.ReadLine();

                if (!String.IsNullOrEmpty(choice) && (choice == "N" || choice == "n"))
                {
                    AddNewUser();
                }
                else if (int.TryParse(choice, out int index) && index <= users.Count)
                {
                    Console.Clear();
                    SelectedPlayer(index -= 1); // Adjust the index to choose right player
                }
            }

            //Send selected player with data to Hangman-Game
            static void SelectedPlayer(int index)
            {
                // Console.Clear();
                Console.WriteLine($"-------------------------------");
                Console.WriteLine($"Player: {users[index].Name}");
                Console.WriteLine($"Money: {users[index].Money} USD");
                Console.WriteLine($"Lives: {users[index].Lives}");
                Console.WriteLine($"");


                // Directly modify the user's data
                User selectedUser = users[index];
                // Update Lives with the lives from the selected user
                int Lives = selectedUser.Lives;
                // Update money with the money from the selected user
                int Money = selectedUser.Money;
                // Update name with the money from the selected user
                string Name = selectedUser.Name;

                // Save the changes to the JSON file
                SaveUsers(users);


                PlayHangman(Name, Lives, Money);
            }







            //Loads hangmanword from json-file
            //Loads hangmanword from json-file
            static string LoadHangmanWord(int level, string category)
            {
                if (File.Exists(FilePathWord))
                {
                    List<wordBank> words = LoadWords();
                    Random r = new Random();
                    List<wordBank> filteredWords = words; // Initialize with all words

                    // Category filter
                    if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")
                    {
                        filteredWords = filteredWords.Where(w => w.Category.ToLower() == category.ToLower()).ToList();
                    }

                    // Apply difficulty
                    if (level == 1) // Easy
                    {
                        filteredWords = filteredWords
                            .Where(w => !w.Word.Contains("z") && !w.Word.Contains("y") && !w.Word.Contains("x"))
                            .Where(w => w.Word.Length <= 5)
                            .ToList();
                    }
                    else if (level == 2) // Medium
                    {
                        filteredWords = filteredWords
                            .Where(w => !w.Word.Contains("z") && !w.Word.Contains("y") && !w.Word.Contains("x"))
                            .Where(w => w.Word.Length > 5 && w.Word.Length <= 7)
                            .ToList();
                    }
                    else if (level == 3) // Hard
                    {
                        filteredWords = filteredWords
                            .Where(w => w.Word.Length > 7 || (w.Word.Length > 4 && (w.Word.Contains("z") || w.Word.Contains("y") || w.Word.Contains("x") || w.Word.Contains("v") || w.Word.Contains("w"))))
                            .ToList();
                    }
                    else
                    {
                        Console.WriteLine("Invalid difficulty level. Please choose 1 (easy), 2 (medium), or 3 (hard).");
                        return null;
                    }

                    if (filteredWords.Count > 0)
                    {
                        string randomWord = filteredWords[r.Next(0, filteredWords.Count)].Word;
                        return randomWord;
                    }
                    else
                    {
                        Console.WriteLine("There are no words available in this difficulty at the moment. Try another one.");
                    }
                }
                else
                {
                    Console.WriteLine("No words available.");
                }

                return null;
            }






            static void StartGameWithZeroLives(string userName, int userLives, int userMoney)
            {

                User currentUser = users.Find(user => user.Name == userName);

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"You are out of lives. You need to buy more lives to play.");
                    Console.WriteLine($"Do you want to buy more lives?");
                    Console.Write($"Y/N: ");
                    string? purchase = Console.ReadLine();

                    if (!String.IsNullOrEmpty(purchase) && (purchase == "y" || purchase == "Y"))
                    {

                        Console.Clear();
                        Console.WriteLine($"---------------");
                        Console.WriteLine($"THE LIFE SHOP");
                        Console.WriteLine($"---------------");
                        Console.WriteLine($"1 life = 1 USD");
                        Console.WriteLine($"");
                        Console.WriteLine($"Money: {currentUser.Money} USD");
                        Console.WriteLine($"");
                        Console.Write($"How many lives to you want to purchase? ");
                        string? purchasedLives = Console.ReadLine();
                        int.TryParse(purchasedLives, out int newLives);

                        if (newLives > currentUser.Money) //since one life cost the same as one USD 
                        {
                            Console.WriteLine($"You don't have enought money to buy {newLives} new lives.");
                        }
                        else
                        {
                            Console.WriteLine($"Are you sure you want to buy {newLives} lives for {newLives} USD? ");
                            Console.Write($"You're new balance will be {currentUser.Money - newLives} USD? ");
                            Console.Write($"Y/N: ");
                            string? choice = Console.ReadLine();

                            if (!String.IsNullOrEmpty(choice) && (choice == "y" || choice == "Y"))
                            {
                                Console.Clear();
                                Console.WriteLine($"You have bought {newLives} new lives!");

                                currentUser.Lives = newLives;
                                if (currentUser != null)
                                {
                                    currentUser.Lives = 0;
                                    currentUser.Lives += newLives;
                                    currentUser.Money -= newLives;
                                    SaveUsers(users); // Save the changes to the JSON file
                                }
                                break;
                            }
                            else if (!String.IsNullOrEmpty(choice) && (choice == "n" || choice == "N"))
                            {
                                Console.WriteLine($"No lives bought.");

                            }
                            else if (String.IsNullOrEmpty(choice))
                            {
                                Console.WriteLine($"Y/N");

                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(purchase) && purchase == "n" || purchase == "N")
                    {
                        Console.Clear();
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"Exiting game.");
                        Console.WriteLine($"----------------------");
                        Console.ReadLine();

                        Environment.Exit(0);
                        break;
                    }
                }
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
                string category;
                Console.Write("Place the word in a category: ");
                Console.WriteLine("------");
                Console.WriteLine("Animals");
                Console.WriteLine("Countries");
                Console.WriteLine("");

                while (true)
                {
                    Console.Write("Type: ");
                    category = Console.ReadLine().ToLower();

                    if (category.ToLower() != "animals" && category.ToLower() != "countries")
                    {
                        Console.WriteLine("Invalid input. Spell out the");
                    }

                    Console.Write("Add a new word: ");
                    string newWord = Console.ReadLine().ToLower();

                    if (!string.IsNullOrWhiteSpace(newWord))
                    {

                        List<wordBank> existingWords = LoadWords();

                        if (!existingWords.Any(w => w.Word.Equals(newWord, StringComparison.OrdinalIgnoreCase)))
                        {
                            wordBank word = new wordBank(newWord, category);
                            existingWords.Add(word);
                            SaveData(existingWords);
                            Console.WriteLine("Word added successfully!");
                            Console.Write("Press enter to return to the menu");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("The word already exists. Please enter a different word.");
                        }

                        break; // This is where you should break out of the loop
                    }

                    else
                    {
                        Console.WriteLine("Invalid input. The word cannot be empty.");
                    }
                }
            }




            //if user guess correct letter
            static string correctGuess(string let1, string currentState, ref string theWord)
            {
                // Add a line break after printing the guessed words
                Console.Clear();
                Console.WriteLine($"-------------------------------------");
                Console.WriteLine($"CORRECT! The letter '{let1.ToUpper()}' is in the word!");
                Console.WriteLine($"-------------------------------------");
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
                Console.WriteLine($"WRONG! The letter '{let1.ToUpper()}' is not in the word");
                Console.WriteLine($"-------------------------------------");
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
            static void GameOver(string theWord, string currentState, ref int Lives, int bet, string userName, int userMoney)
            {

                User currentUser = users.Find(user => user.Name == userName);

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"----------------------");
                    Console.WriteLine($"GAME OVER!");
                    Console.WriteLine($"----------------------");
                    // Thread.Sleep(2000);
                    DrawLivesMethod.DrawLives(Lives);

                    if (currentUser.Money == 0)
                    {
                        Console.WriteLine($"You have no money or lives left!");
                        Thread.Sleep(2000);
                        oneLastChance(userName, userMoney);
                        return;
                    }

                    Console.WriteLine($"Do you want to buy more lives and keep playing?");
                    Console.Write($"Y/N: ");
                    string? purchase = Console.ReadLine();

                    if (!String.IsNullOrEmpty(purchase) && (purchase == "y" || purchase == "Y"))
                    {
                        Console.Clear();
                        Console.WriteLine($"---------------");
                        Console.WriteLine($"THE LIFE SHOP");
                        Console.WriteLine($"---------------");
                        Console.WriteLine($"1 life = 1 USD");
                        Console.WriteLine($"");
                        Console.WriteLine($"Money: {currentUser.Money} USD");
                        Console.WriteLine($"");
                        Console.Write($"How many lives to you want to purchase? ");
                        string? purchasedLives = Console.ReadLine();
                        int.TryParse(purchasedLives, out int newLives);

                        if (newLives > currentUser.Money) //since one life cost the same as one USD 
                        {
                            Console.WriteLine($"You don't have enought money to buy {newLives} new lives.");
                        }
                        else
                        {
                            Console.WriteLine($"Are you sure you want to buy {newLives} lives for {newLives} USD? ");
                            Console.WriteLine($"You're new balance will be {currentUser.Money - newLives} USD?");
                            Console.Write($"Y/N: ");
                            string? choice = Console.ReadLine();

                            if (!String.IsNullOrEmpty(choice) && (choice == "y" || choice == "Y"))
                            {
                                Console.Clear();
                                Console.WriteLine($"You have bought {newLives} new lives!");

                                Lives = newLives;
                                if (currentUser != null)
                                {
                                    currentUser.Lives = 0;
                                    currentUser.Lives += newLives;
                                    currentUser.Money -= newLives;
                                    SaveUsers(users); // Save the changes to the JSON file
                                }

                                GetMoreLives(currentState, Lives);
                                break;
                            }
                            else if (!String.IsNullOrEmpty(choice) && (choice == "n" || choice == "N"))
                            {
                                Console.WriteLine($"No lives bought.");

                            }
                            else if (String.IsNullOrEmpty(choice))
                            {
                                Console.WriteLine($"Y/N");

                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(purchase) && purchase == "n" || purchase == "N")
                    {
                        Console.Clear();
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"THE END! Thanks for playing.");
                        Console.WriteLine($"The correct word was: '{theWord.ToUpper()}'.");
                        Console.WriteLine($"----------------------");

                        User currentUser_ = users.Find(user => user.Name == userName);

                        if (currentUser_ != null)
                        {
                            // currentUser_.Money -= bet;
                            currentUser_.Lives = 0;
                            SaveUsers(users); // Save the changes immediately
                        }

                        Console.WriteLine($"You lost your bet of {bet} USD!");
                        Console.WriteLine($"Player: {currentUser_.Name}");
                        Console.WriteLine($"Money: {currentUser_.Money} USD");
                        Console.WriteLine($"Lives: {currentUser_.Lives}");
                        Console.WriteLine($"----------------------");
                        Console.Write($"Press enter to exit.");
                        ExitApp();
                        break;

                    }
                }
            }



            static void ShowCurrentGameStatus(string currentState, int Lives)
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
                DrawLivesMethod.DrawLives(Lives);

            }



            //make user pay for more lives
            static void GetMoreLives(string currentState, int Lives)
            {
                Console.Clear();
                Console.WriteLine($"-------------------------------------");
                Console.WriteLine($"LET'S GO!");
                Console.WriteLine($"-------------------------------------");

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

                DrawLivesMethod.DrawLives(Lives);
                Console.Write($"Lives: {Lives} ");
                LifeAsHeartMethod.LivesAsHeart(Lives);
                Console.WriteLine($"");
                Console.WriteLine($"");

            }



            //make user bet before start
            static int Bets(string userName)
            {

                while (true)
                {
                    Console.WriteLine("Do you want to make a bet?");
                    Console.Write($"Y/N: ");
                    string? input = Console.ReadLine();

                    User currentUser = users.Find(user => user.Name == userName);


                    if (!string.IsNullOrEmpty(input) && (input == "y" || input == "Y"))
                    {
                        Console.Write("How much (USD)? ");
                        int bets = Convert.ToInt16(Console.ReadLine());

                        if (currentUser != null && currentUser.Money < bets)
                        {
                            Console.WriteLine("You don't have enough money to make that bet.");
                        }
                        else
                        {
                            // Console.Clear();
                            currentUser.Money -= bets;
                            SaveUsers(users); // Save the changes immediately
                            return bets;
                        }

                    }
                    else if (!string.IsNullOrEmpty(input) && (input == "n" || input == "N"))
                    {
                        Console.WriteLine("No bets.");
                        int bets = 0;
                        Console.Clear();
                        return bets;

                    }
                }
            }

            // //Drawing the hangman-status
            // static void DrawLives(int Lives)
            // {
            //     switch (Lives)
            //     {
            //         case 10:
            //             Console.WriteLine("   ");
            //             Console.WriteLine("                ");
            //             Console.WriteLine("         ");
            //             Console.WriteLine("   ");
            //             Console.WriteLine("   ");
            //             Console.WriteLine("   ");
            //             Console.WriteLine("   ");
            //             Console.WriteLine("   ");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 9:
            //             Console.WriteLine("   ");
            //             Console.WriteLine("                ");
            //             Console.WriteLine("         ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 8:
            //             Console.WriteLine("   ");
            //             Console.WriteLine("   |              ");
            //             Console.WriteLine("   |        ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 7:
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |  /              ");
            //             Console.WriteLine("   | /        ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 6:
            //             Console.WriteLine("   |-------------------");
            //             Console.WriteLine("   |  /              ");
            //             Console.WriteLine("   | /        ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 5:
            //             Console.WriteLine("   |-------------------");
            //             Console.WriteLine("   |  /               |");
            //             Console.WriteLine("   | /        ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 4:
            //             Console.WriteLine("   |-------------------");
            //             Console.WriteLine("   |  /               |");
            //             Console.WriteLine("   | /               ('')");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 3:
            //             Console.WriteLine("   |-------------------");
            //             Console.WriteLine("   |  /               |");
            //             Console.WriteLine("   | /               ('')");
            //             Console.WriteLine("   |                 /|| ");
            //             Console.WriteLine("   |                  ||");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;

            //         case 2:
            //             Console.WriteLine("   |-------------------");
            //             Console.WriteLine("   |  /               |");
            //             Console.WriteLine("   | /               ('')");
            //             Console.WriteLine("   |                 /|| ");
            //             Console.WriteLine("   |                  ||");
            //             Console.WriteLine("   |                 /   ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;
            //         case 1:
            //             Console.WriteLine("   |-------------------                           ");
            //             Console.WriteLine("   |  /               |      |--------------|   ");
            //             Console.WriteLine("   | /               (**) ---| LAST CHANCE! |   ");
            //             Console.WriteLine("   |                 /||     |______________|   ");
            //             Console.WriteLine("   |                  ||              ");
            //             Console.WriteLine("   |                 / ");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("   |");
            //             Console.WriteLine("----------------------------       ");
            //             break;
            //         case 0:
            //             Console.WriteLine("   |-------------------");
            //             Console.WriteLine("   |  /               |   ");
            //             Console.WriteLine("   | /                |   ");
            //             Console.WriteLine("   |                  |  ");
            //             Console.WriteLine("   |                 (--) ");
            //             Console.WriteLine("   |                 /||   ");
            //             Console.WriteLine("   |                  ||      ");
            //             Console.WriteLine("   |                  /           ");
            //             Console.WriteLine("----------------------------       ");

            //             break;

            //         default:
            //             Console.WriteLine("   ");
            //             Console.WriteLine("      ");
            //             Console.WriteLine("      ");
            //             Console.WriteLine("       ");
            //             Console.WriteLine("     ");
            //             Console.WriteLine("    ");
            //             Console.WriteLine("    ");
            //             Console.WriteLine("      ");
            //             Console.WriteLine("----------------------------       ");
            //             break;
            //     }
            // }




            //The very last change
            static void oneLastChance(string userName, int userMoney)
            {

                //getting a random riddle from method
                Riddle riddle = GetRandomRiddle();
                //question = riddle.Question
                //answer = riddle.Answer
                //keyord = riddle.Keyword

                Console.Clear();
                Console.WriteLine("");
                Thread.Sleep(1000);

                Console.Write("Wait");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    Console.Write(".");
                }
                Thread.Sleep(1000);
                Console.WriteLine("");
                Console.Write("The Hangman is coming");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    Console.Write(".");
                }
                Thread.Sleep(1000);

                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.Clear();


                Thread.Sleep(1000);
                Console.WriteLine("                                     ");
                Console.WriteLine("                                     ");
                Console.WriteLine("        \\\\|||////                        ");
                Console.WriteLine("      .  =======                          ");
                Console.WriteLine("     / \\| `x x´ |                          ");
                Console.WriteLine("     \\ / \\ ___ /                        ");
                Console.WriteLine("      #   _| |_                             ");
                Console.WriteLine("     (#) (     )                         ");
                Console.WriteLine("      #\\//|* *|\\\\                         ");
                Console.WriteLine("      #\\/(  *  )\\\\                       ");
                Console.WriteLine("      #   =====  \\\\                     ");
                Console.WriteLine("      #   ( U )  (*)                     ");
                Console.WriteLine("      #   || ||                       ");
                Console.WriteLine("      # '_'| |`_'                  ");
                Console.WriteLine("      # *--' `--*                  ");
                Console.WriteLine("");
                Thread.Sleep(1000);

                string sentence1 = $"Hangman: 'Hello {userName}'";
                string sentence2 = $"Hangman: 'I can see you have neither any money or lives left'";
                string sentence3 = $"Hangman: 'Well, luckily for you I'm in a good mode today...'";
                string sentence4 = $"Hangman: 'and I'm willing to offer you a chance to stay a live a little longer'";
                string sentence5 = $"Hangman: 'Are you interested? Y/N:'";

                string answer_NO = $"{userName}: 'No'.";
                string sentence_NO = $"Hangman: 'HAHAHAHAHA well then, good bye {userName}, I'll see you on the other side'";
                string sentence_NO2 = $"Hangman: 'HAHAHAHAHAHAHHAHAHHAAHAHAHAHAHAHAHAHAHAHA'";

                string answer_YES = $"{userName}: 'Yes'";
                string sentence_YES = $"Hangman: 'Of course you are!'";
                string sentence_YES2 = $"Hangman: 'If you can guess my riddle, I will give you 10 new lives'";
                string sentence_YES3 = $"Hangman: 'Get ready! Here comes the riddle'";
                string sentence_YES4 = $"Hangman: 'The riddle is: {riddle.Question}'";

                string sentence_YES_correct1 = $"Hangman: '...I'm impressed'";
                string sentence_YES_correct2 = $"Hangman: 'Well... I guess I have to keep my word now. Or do I...?'";
                string sentence_YES_correct3 = $"Hangman: 'Here are 10 new lives'";
                string sentence_YES_correct4 = $"Hangman: 'You got away this time.'";
                string sentence_YES_correct5 = $"Hangman: 'But I'll see you soon again {userName}...'";

                string sentence_YES_wrong1 = $"Hangman: 'Ops... That's wrong HAHAHAHA'";
                string sentence_YES_wrong2 = $"Hangman: 'The answer is {riddle.Answer}!'";
                string sentence_YES_wrong3 = $"Hangman: 'You're out of the game. Bye bye {userName}!'";




                foreach (char letter in sentence1)
                {
                    Console.Write(letter);
                    Thread.Sleep(20);
                }
                Console.ReadLine();

                foreach (char letter in sentence2)
                {
                    Console.Write(letter);
                    Thread.Sleep(20);
                }
                Console.ReadLine();

                foreach (char letter in sentence3)
                {
                    Console.Write(letter);
                    Thread.Sleep(20);
                }
                Console.WriteLine("");
                foreach (char letter in sentence4)
                {
                    Console.Write(letter);
                    Thread.Sleep(20);
                }
                Console.ReadLine();
                foreach (char letter in sentence5)
                {
                    Console.Write(letter);
                    Thread.Sleep(20);
                }

                while (true)
                {
                    string? answer = Console.ReadLine();

                    //if chance is taken
                    if (!String.IsNullOrEmpty(answer) && (answer == "Y" || answer == "y"))
                    {
                        foreach (char letter in answer_YES)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        // Console.WriteLine("");
                        Console.ReadLine();
                        foreach (char letter in sentence_YES)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence_YES2)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence_YES3)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence_YES4)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.WriteLine("");

                        while (true)
                        {

                            Console.Write($"{userName}: ");
                            string? answer_riddle = Console.ReadLine();

                            if (!String.IsNullOrEmpty(answer_riddle) && answer_riddle.Contains(riddle.Keyword, StringComparison.OrdinalIgnoreCase))
                            {

                                foreach (char letter in sentence_YES_correct1)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.ReadLine();
                                foreach (char letter in sentence_YES_correct2)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.ReadLine();
                                foreach (char letter in sentence_YES_correct3)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }

                                //gives user 10 more lives
                                Console.WriteLine("");
                                Console.WriteLine("");
                                Console.WriteLine("Hangman has given you 10 new lives");
                                Console.WriteLine("");

                                List<User> users = LoadUsers(); // Load existing users
                                User user = users.Find(user => user.Name == userName);

                                if (user != null)
                                {
                                    user.Lives = 10;
                                    SaveUsers(users);
                                }

                                Thread.Sleep(1000);
                                foreach (char letter in sentence_YES_correct4)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.ReadLine();
                                foreach (char letter in sentence_YES_correct5)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.ReadLine();

                                Console.WriteLine("");
                                Console.WriteLine("The Hangman has kicked you out of the game");
                                Console.WriteLine("Start the game again to receive your new lives");
                                Console.WriteLine("");
                                Console.ReadLine();

                                // Reset guessesLetter List and guessesLetterArray
                                guessesLetter.Clear();

                                Console.ResetColor();
                                Console.Clear();
                                Environment.Exit(0);



                                // PlayHangman(user.Name, user.Lives, user.Money);
                                // Environment.Exit(0); //exit game
                                return;

                            }
                            else if (String.IsNullOrEmpty(answer_riddle))
                            {
                                Console.Write("Invaild input");

                            }
                            else
                            {
                                foreach (char letter in sentence_YES_wrong1)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.ReadLine();
                                foreach (char letter in sentence_YES_wrong2)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.ReadLine();
                                foreach (char letter in sentence_YES_wrong3)
                                {
                                    Console.Write(letter);
                                    Thread.Sleep(20);
                                }
                                Console.WriteLine("");
                                removeUser(userName);
                                Thread.Sleep(1000);

                                Console.ResetColor();
                                Console.Clear();
                                Environment.Exit(0);

                            }
                        }
                    }
                    else if (!String.IsNullOrEmpty(answer) && (answer == "N" || answer == "n"))
                    {
                        foreach (char letter in answer_NO)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence_NO)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.WriteLine("");
                        foreach (char letter in sentence_NO2)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Thread.Sleep(1000);
                        Console.WriteLine();
                        removeUser(userName); //run method to remomve user
                        Console.ResetColor();
                        Environment.Exit(0); //exit game

                    }
                    else
                    {
                        Console.Write($"Hangman: 'Are you interested? Y/N:'");
                    }
                }

            }


            static string NewPlayerStoryIntro()
            {

                Console.Clear();
                Console.WriteLine("--------------------------");
                Console.WriteLine("Let's create a new player: ");
                Console.WriteLine("");
                Console.Write("Enter your name: ");
                string? name = Console.ReadLine();

                while (true)
                {

                    if (!string.IsNullOrEmpty(name))
                    {
                        Thread.Sleep(1000);
                        Console.WriteLine($"Ok great. Let's begin.");
                        Thread.Sleep(2000);
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Clear();
                        Thread.Sleep(1000);

                        Console.WriteLine("");
                        Console.WriteLine("   ,,,,,");
                        Console.WriteLine("  |     |");
                        Console.WriteLine("  |-O-O-|");
                        Console.WriteLine(" (|  ^  |)");
                        Console.WriteLine("  | --- |");
                        Console.WriteLine("   \\___/");
                        Console.WriteLine("   |   |");
                        Console.WriteLine("");
                        Thread.Sleep(1000);

                        string sentence1 = $"Instructor: 'Hi {name}'";
                        string sentence2 = $"Instructor: 'Welcome to the Hangman game'";
                        string sentence3 = $"Instructor: 'As you might know, there is a Hangman in town'";
                        string sentence4 = $"Instructor: 'And he ain't a nice guy. Last week I played riddles with my life on the line with him'";
                        string sentence5 = $"Instructor: 'Luckily, the riddle was fairly easy, and here I am. Still alive. Thank God'";
                        string sentence6 = $"Instructor: 'Well anyway. I want to wish you good luck.'";
                        string sentence7 = $"Instructor: '...Oh. I almost forgot'";
                        string sentence8 = $"Instructor: 'You're going to need this...'";
                        string sentence9 = $"Instructor: 'Ok now. Are you ready?'";
                        string sentence10 = $"Instructor: 'Off you go then. Good luck now!'";

                        foreach (char letter in sentence1)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence2)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence3)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence4)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence5)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence6)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence7)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence8)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        Console.WriteLine("");
                        Console.WriteLine("Instructor has given you 20 USD and 7 Lives");

                        Console.ReadLine();
                        foreach (char letter in sentence9)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        foreach (char letter in sentence10)
                        {
                            Console.Write(letter);
                            Thread.Sleep(20);
                        }
                        Console.ReadLine();
                        Console.ResetColor();
                        return name;

                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                }

            }





            static void removeUser(string userName)
            {
                List<User> users = LoadUsers(); // Load existing users
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

            // static void Rules()
            // {
            //     Console.Clear();
            //     Console.WriteLine("-----------------------");
            //     Console.WriteLine("RULES");
            //     Console.WriteLine("-----------------------");
            //     Console.WriteLine("BASICS:");
            //     Console.WriteLine("------:");
            //     Console.WriteLine("   To play the Hangman game you going to need money and lives.");
            //     Console.WriteLine("   If you run out of lives, you can always buy new lives for the money.");
            //     Console.WriteLine("BETTING:");
            //     Console.WriteLine("------:");
            //     Console.WriteLine("   To earn more money, you need to bet before the Hangman game.");
            //     Console.WriteLine("   You will either loose or win the betting amount depending on if you win or loose the Hangmsn sequence.");
            //     Console.WriteLine("WINNIGS:");
            //     Console.WriteLine("------:");
            //     Console.WriteLine("   You win money by winning a Hangman sequence.");
            //     Console.WriteLine("   For level easy = 6 USD, for medium = 8 USD, for hard = 10 USD.");
            //     Console.WriteLine("");
            //     Console.WriteLine("GOOD LUCK!");
            //     Console.WriteLine("-----------------------");


            //     Console.WriteLine("");
            //     Console.Write("Press enter to return to menu");
            //     Console.ReadLine();

            // }



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


            static Riddle GetRandomRiddle()
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

            static void ExitApp()
            {
                Console.WriteLine("");
                Console.Write("Quiting game");
                for (int i = 0; i < 4; i++)
                {
                    Thread.Sleep(400);
                    Console.Write(".");
                }
                Console.WriteLine("");
                Thread.Sleep(100);
                Console.WriteLine("Good Bye!");
                Environment.Exit(0);
            }


            // static void LivesAsHeart(int amount)
            // {

            //     for (int i = 0; i < amount; i++)
            //     {
            //         Console.Write("❤ ");
            //     }
            // }
        }
    }
}



