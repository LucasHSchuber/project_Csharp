
using System;
using System.Collections.Generic;

namespace HM
{
    class Program
    {

        static List<string> guessesLetter = new List<string>(); // Move the list declaration outside the method

        static void Main(string[] args)
        {
            //START APP WITH CLEAR CONSOLE
            Console.Clear();

            //retrieve the hangmanword
            string theWord = "hangman"; // = "hangman"
            //converting retrieved word to underscored string
            string currentState = new string('_', theWord.Length); // = "______"

            //retrieve the new hangmanword after guessing
            currentState = LoadWord(currentState);
            // string userInput = currentState;


            int Lives = 2; //declare var, set it to 5

            Console.WriteLine($"----------------------");
            Console.WriteLine("THE GAME HAS STARTED!");
            Console.WriteLine($"----------------------");
            Console.WriteLine("Enter your first guess: ");


            while (currentState.Length > 0)
            {

                if (currentState != null)
                {

                    // char[] word = userInput.ToCharArray();

                    // Console.WriteLine(word);

                    //USER GUESSING
                    Console.WriteLine("Enter a letter: ");
                    string? let1 = Console.ReadLine();

                    //if let1 is not null and include a letter
                    if (let1 != null && let1.Length == 1 && Char.IsLetter(let1[0]))
                    {

                        //if user guess a aldready guessed letter
                        if (guessesLetter.Contains(let1))
                        {
                            Console.Clear();
                            Console.WriteLine($"You have aldready guessed: {let1.ToUpper()}. Guess again. ");
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

                        GetMoreLives(currentState);

                    }
                    else if (purchase == "n" || purchase == "N")
                    {
                        Console.Clear();
                        Console.WriteLine($"THE END! Thanks for playing!");
                        Console.WriteLine($"The correct word was: '{theWord.ToUpper()}'.");
                        Console.ReadKey();
                        break;

                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine($"Do you want to buy more lives?");
                        Console.Write($"YES or NO? ");
                        Console.ReadKey();
                    }



                }
            }
        }

        //****METHODS****

        //loads hangmanword from json-file
        static string LoadWord(string userInputNew) //make it possible to choose different difficulty levels from a menu at start
        {

            return userInputNew;
        }

        //adds a word from user input 
        static void AddWord()
        {

        }
        //if user guess correct letter
        static string correctGuess(string let1, string currentState, ref string theWord)
        {

            // Add a line break after printing the guessed words
            Console.Clear();
            Console.WriteLine($"-------------------------------------");
            Console.WriteLine($"YES!!! The letter '{let1.ToUpper()}' is in the word!");

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
            Console.WriteLine($"NO!!! The letter '{let1.ToUpper()}' is not in the word.");
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


        //make user bet before start
        static void Bets()
        {

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



