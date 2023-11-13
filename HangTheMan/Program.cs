using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace HM
{
    class Program
    {

        public static List<wordBank> words = new List<wordBank>();

        static List<string> guessesLetter = new List<string>(); // Move the list declaration outside the method
        const string FilePath = "json/hangmanwordbank.json"; //filename for storing words


        static void Main(string[] args)
        {

            Console.Clear();
            Console.WriteLine($"--------------");
            Console.WriteLine($"WELCOME!");
            Console.WriteLine($"");
            Console.WriteLine($"1. Play Hangman");
            Console.WriteLine($"2. Add new word!");
            Console.WriteLine($"");
            Console.WriteLine($"--------------");

            Console.Write("Request: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    PlayHangman();
                    break;

                case "2":
                    AddWord();
                    break;

                default:
                    Console.WriteLine("Invalid choice. Exiting.");
                    break;
            }






            static void PlayHangman()
            {


                string? user = AddUser();
                Console.WriteLine($"Hi {user}");
                int bet = Bets();

                //START APP WITH CLEAR 
                Console.Clear();
                // string theWord = "hangman"; 
                string theWord = LoadHangmanWord().ToLower();
                //converting retrieved word to underscored string
                string currentState = new string('_', theWord.Length);

                int Lives = 5; //declare var, set it to 5

                Console.WriteLine($"----------------------");
                Console.WriteLine("THE GAME HAS STARTED!");
                Console.WriteLine($"----------------------");
                Console.WriteLine($"You have bet {bet} USD ");
                Console.WriteLine($"");
                Console.WriteLine($"{currentState}");
                Console.WriteLine($"");
                Console.WriteLine("Enter your first guess: ");


                while (currentState.Length > 0)
                {

                    if (currentState != null)
                    {

                        // char[] word = userInput.ToCharArray();

                        // Console.WriteLine(word);

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
                        Console.ReadKey();
                        break;

                    }
                    else if (Lives == 0)
                    {
                        GameOver(theWord, currentState, ref Lives);


                    }
                }
            }

            //****METHODS****

            static string AddUser()
            {
                while (true)
                {
                    Console.WriteLine("Enter your name: ");
                    string? name = Console.ReadLine();
                    if (!string.IsNullOrEmpty(name))
                    {
                        Console.Clear();
                        return name;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid name.");
                    }
                }
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



            //loads hangmanword from json-file
            static string LoadHangmanWord()
            {
                if (File.Exists(FilePath))
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



            static void SaveData(List<wordBank> data)
            {
                string json = JsonConvert.SerializeObject(data);
                File.WriteAllText(FilePath, json);
            }



            // Load existing words from the file
            static List<wordBank> LoadWords()
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
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
            static void GameOver(string theWord, string currentState, ref int Lives)
            {
                Console.Clear();
                Console.WriteLine($"----------------------");
                Console.WriteLine($"GAME OVER! You have no more lives.");
                Console.WriteLine($"----------------------");
                Console.WriteLine($"Do you want to buy more lives?");
                Console.Write($"Y/N : ");
                string? purchase = Console.ReadLine();

                if (purchase == "y" || purchase == "Y")
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

                }
                else if (purchase == "n" || purchase == "N")
                {
                    Console.Clear();
                    Console.WriteLine($"----------------------");
                    Console.WriteLine($"THE END! Thanks for playing.");
                    Console.WriteLine($"The correct word was: '{theWord.ToUpper()}'.");
                    Console.WriteLine($"----------------------");
                    Console.ReadKey();
                    Environment.Exit(0);

                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Do you want to buy more lives?");
                    Console.Write($"YES or NO? ");
                    Console.ReadKey();
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
        }
    }
}



