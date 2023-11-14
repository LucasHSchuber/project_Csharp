﻿using System;
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
                Console.WriteLine($"4. Rules");
                Console.WriteLine($"5. Quit game");
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
                        Rules();
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

                List<User> users = LoadUsers();

                if (userLives == 0 && userMoney > 0)
                {
                    StartGameWithZeroLives(userName, userLives, userMoney);
                }
                else if (userLives == 0 && userMoney == 0)
                {
                    oneLastChance(userName, userMoney);
                }

                int bet;
                if (userMoney > 0)
                {
                    bet = Bets(userName);
                }
                else
                {
                    bet = 0;
                }


                //START APP WITH CONSOLE CLEAR 
                Console.Clear();
                //Load word 
                string theWord = LoadHangmanWord().ToLower();
                //convert loaded word to underscored string
                string currentState = new string('_', theWord.Length);

                char[] guessesLetterArray = new char[theWord.Length]; // Reset the guessed letters array
                Array.Fill(guessesLetterArray, '_'); // Fill the array with underscores

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
                        // List<User> users_update = LoadUsers();
                        User currentUser_update = users.Find(user => user.Name == userName);
                        int new_Lives = Lives;
                        currentUser_update.Lives = Lives;
                        SaveUsers(users);
                        Console.WriteLine($"You won {bet} USD!");
                        Console.WriteLine($"Player: {currentUser_update.Name}");
                        Console.WriteLine($"Money: {currentUser_update.Money} USD");
                        Console.WriteLine($"Lives: {new_Lives}");
                        Console.WriteLine($"----------------------");
                        Console.WriteLine($"Press enter to return to menu");
                        Console.ReadLine();
                        // Reset guessesLetter List and guessesLetterArray
                        guessesLetter.Clear();
                        guessesLetterArray = new char[theWord.Length];
                        Array.Fill(guessesLetterArray, '_');

                        break;
                    }
                    else if (Lives == 0)
                    {
                        GameOver(theWord, currentState, ref Lives, bet, userName, userMoney);

                        // Reset guessesLetter List and guessesLetterArray
                        guessesLetter.Clear();
                        guessesLetterArray = new char[theWord.Length];
                        Array.Fill(guessesLetterArray, '_');

                        break;
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
                        int StarMoney = 20;
                        int StartLives = 7;
                        User newUser = new User(name, StarMoney, StartLives); // Create a new user (name,USD,lives)
                        allUsers.Add(newUser); // Add the new user to the list
                        SaveUsers(allUsers); // Save the updated list to the json file

                        Console.Clear();
                        Console.WriteLine("------------------------------------------------");
                        Console.WriteLine($"Your user '{name}' is created!");
                        Console.WriteLine($"You have been given {StarMoney} USD and {StartLives} lives to start with. ");
                        Console.WriteLine($"");
                        Console.WriteLine($"Name: {name}");
                        Console.WriteLine($"Money: {StarMoney} USD");
                        Console.WriteLine($"Lives: {StartLives}");

                        // List<User> users = LoadUsers(); // Load list after saving
                        users = LoadUsers(); // Load the updated list into the global variable

                        int index = users.FindIndex(user => user.Name == name);
                        Console.WriteLine($"Index: {index}");
                        Console.WriteLine("");

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
                    SelectedPlayer(index -= 1); // Adjust the index to choose right player
                }
            }

            //Send selected player with data to Hangman-Game
            static void SelectedPlayer(int index)
            {
                // Console.Clear();
                Console.WriteLine($"You selected: {users[index].Name}");

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





            static void StartGameWithZeroLives(string userName, int userLives, int userMoney)
            {

                User currentUser = users.Find(user => user.Name == userName);

                while (true)
                {

                    Console.WriteLine($"Do you want to buy more lives and keep playing?");
                    Console.Write($"Y/N : ");
                    string? purchase = Console.ReadLine();

                    if (!String.IsNullOrEmpty(purchase) && (purchase == "y" || purchase == "Y"))
                    {

                        Console.WriteLine(" ");
                        Console.WriteLine($"THE LIFE SHOP");
                        Console.WriteLine($"----------------------");
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
                            Console.Write($"Are you sure you want to buy {newLives} lives for {newLives} USD? Y/N: ");
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
            static void GameOver(string theWord, string currentState, ref int Lives, int bet, string userName, int userMoney)
            {

                User currentUser = users.Find(user => user.Name == userName);

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"----------------------");
                    Console.WriteLine($"GAME OVER!");
                    Console.WriteLine($"----------------------");
                    DrawLives(Lives);

                    if (currentUser.Money == 0)
                    {
                        oneLastChance(userName, userMoney);

                    }

                    Console.WriteLine($"Do you want to buy more lives and keep playing?");
                    Console.Write($"Y/N : ");
                    string? purchase = Console.ReadLine();

                    if (!String.IsNullOrEmpty(purchase) && (purchase == "y" || purchase == "Y"))
                    {

                        Console.WriteLine(" ");
                        Console.WriteLine($"THE LIFE SHOP");
                        Console.WriteLine($"----------------------");
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
                            Console.Write($"Are you sure you want to buy {newLives} lives for {newLives} USD? Y/N: ");
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
            static void GetMoreLives(string currentState, int Lives)
            {
                Console.Clear();
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

                DrawLives(Lives);
                Console.WriteLine($"Lives: {Lives}");
                Console.WriteLine($"");

            }



            //make user bet before start
            static int Bets(string userName)
            {

                while (true)
                {
                    Console.Write("Do you want to make a bet? Y/N? ");
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

            //Drawing the hangman-status
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




            //The very last change
            static void oneLastChance(string userName, int userMoney)
            {
                Console.WriteLine($"You have no money or lives left!");
                Console.WriteLine("˙◠˙");
                Thread.Sleep(2000);

                Console.WriteLine("");
                Console.Write("Wait");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(500);
                    Console.Write(".");
                }
                Thread.Sleep(2000);
                Console.WriteLine("");
                Console.Write("The Hangman is coming");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(500);
                    Console.Write(".");
                }

                Thread.Sleep(1000);
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("       \\\\|||////                        ");
                Console.WriteLine("    .   =======                          ");
                Console.WriteLine("  // \\ | O   O |                          ");
                Console.WriteLine("  \\ //  \\`___'//                          ");
                Console.WriteLine("     #    _| |_                             ");
                Console.WriteLine("    (#)  (     )                         ");
                Console.WriteLine("     #\\ //|* *|\\                         ");
                Console.WriteLine("     #\\ /(  *  )/                       ");
                Console.WriteLine("      #   =====                        ");
                Console.WriteLine("      #   ( U )                          ");
                Console.WriteLine("      #   || ||                       ");
                Console.WriteLine("     *#---'| |`----.                  ");
                Console.WriteLine("     '#----' `-----'                  ");
                Console.WriteLine("");
                Thread.Sleep(2000);

                string sentence1 = $"Hangman: 'Hello {userName}'. ";
                string sentence2 = "Hangman: 'I can see you have neither any money or lives left HAHAHA'.";
                string sentence3 = "Hangman: 'Well, luckily for you I'm in a good mode today...'";
                string sentence4 = "Hangman: 'and I'm willing to offer you a chance to stay a live a little longer'.";
                string sentence5 = "Hangman: 'Are you interested? Y/N:'";
                string sentence_YES = "Hangman: 'Ok then..'.";
                string sentence_NO = $"Hangman: 'HAHAHAHAHA ok then good bye {userName}, maybe I'll see you on the other side.'.";
                string sentence_NO2 = "Hangman: 'HAHAHAHAHAHAHHAHAHHAAHAHAHAHAHAHAHAHAHAHA'.";


                foreach (char letter in sentence1)
                {
                    Console.Write(letter);
                    Thread.Sleep(30); // Adjust the sleep duration according to your preference
                }
                Console.ReadLine();

                foreach (char letter in sentence2)
                {
                    Console.Write(letter);
                    Thread.Sleep(30); // Adjust the sleep duration according to your preference
                }
                Console.ReadLine();

                foreach (char letter in sentence3)
                {
                    Console.Write(letter);
                    Thread.Sleep(30); // Adjust the sleep duration according to your preference
                }
                Console.WriteLine("");
                foreach (char letter in sentence4)
                {
                    Console.Write(letter);
                    Thread.Sleep(30); // Adjust the sleep duration according to your preference
                }
                Console.ReadLine();
                foreach (char letter in sentence5)
                {
                    Console.Write(letter);
                    Thread.Sleep(30); // Adjust the sleep duration according to your preference
                }
                string? answer = Console.ReadLine();

                //if chance is taken
                if (!String.IsNullOrEmpty(answer) && (answer == "Y" || answer == "y"))
                {
                    foreach (char letter in sentence_YES)
                    {
                        Console.Write(letter);
                        Thread.Sleep(30); // Adjust the sleep duration according to your preference
                    }







                }
                else if (!String.IsNullOrEmpty(answer) && (answer == "N" || answer == "n"))
                {
                    foreach (char letter in sentence_NO)
                    {
                        Console.Write(letter);
                        Thread.Sleep(30); // Adjust the sleep duration according to your preference
                    }
                    Console.WriteLine("");
                    foreach (char letter in sentence_NO2)
                    {
                        Console.Write(letter);
                        Thread.Sleep(30); // Adjust the sleep duration according to your preference
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    removeUser(userName); //run method to remomve user

                    Environment.Exit(0); //exit game
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

            static void Rules()
            {
                Console.Clear();
                Console.WriteLine("-----------------------");
                Console.WriteLine("RULES");
                Console.WriteLine("-----------------------");
                Console.WriteLine("BASICS:");
                Console.WriteLine("   To play the Hangman game you going to need money and lives.");
                Console.WriteLine("   If you run out of lives, you can always buy new lives for the money.");
                Console.WriteLine("BETTING:");
                Console.WriteLine("   To earn more money, you need to bet before the Hangman game.");
                Console.WriteLine("   You will either loose or win the betting amount depending on if you win or loose the game.");
                Console.WriteLine("-----------------------");


                Console.WriteLine("");
                Console.WriteLine("Press enter to return to menu");
                Console.ReadLine();


            }

            static void ExitApp()
            {

                Console.Write("Quiting game");
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(500);
                    Console.Write(".");
                }
                Console.Write("");
                Environment.Exit(0);
            }

        }
    }
}



