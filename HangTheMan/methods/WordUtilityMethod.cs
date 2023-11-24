using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TheHangMan
{
    public static class WordUtilityMethod
    {
        const string FilePathWord = "json/hangmanwordbank.json"; //filename for storing words


        // Load existing words from the file
        public static List<wordBank> LoadWords()
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

        //Saves a word after putting it in
        public static void SaveWord(List<wordBank> data)
        {
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(FilePathWord, json);
        }

        //adds a word from user input  
        public static void AddWord()
        {
            Console.Clear();
            string category;
            Console.WriteLine($"------------------");
            Console.WriteLine("ADD NEW WORD");
            Console.WriteLine($"------------------");
            Console.WriteLine("");
            Console.WriteLine("Place the word in a category: ");
            Console.WriteLine("");
            Console.WriteLine("Animals");
            Console.WriteLine("Cities");
            Console.WriteLine("Countries");
            Console.WriteLine("Objects");
            Console.WriteLine("");

            while (true)
            {
                Console.Write("Type in category name: ");
                category = Console.ReadLine().ToLower();

                if (category != "animals" && category.ToLower() != "countries" && category.ToLower() != "cities" && category.ToLower() != "objects")
                {
                    Console.WriteLine("Invalid input. Spell out the cateogry name.");
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("");
            Console.WriteLine("----");
            Console.WriteLine($"Ok!");
            Console.WriteLine($"Category: {category}");
            Console.WriteLine("");

            while (true)
            {

                Console.Write("Add the new word: ");
                string newWord = Console.ReadLine().ToLower();

                if (!string.IsNullOrWhiteSpace(newWord))
                {

                    List<wordBank> existingWords = WordUtilityMethod.LoadWords();

                    if (!existingWords.Any(w => w.Word.Equals(newWord, StringComparison.OrdinalIgnoreCase)))
                    {
                        wordBank word = new wordBank(newWord, category);
                        existingWords.Add(word);
                        SaveWord(existingWords);
                        Console.WriteLine("");
                        Console.WriteLine("----");
                        Console.WriteLine("Word added successfully!");
                        Console.Write("Press enter to return to the menu");
                        Console.ReadLine();
                        break;
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
        }



        //Loads hangmanword from json-file
        public static string LoadHangmanWord(int level, string category)
        {
            if (File.Exists(FilePathWord))
            {
                List<wordBank> words = WordUtilityMethod.LoadWords();
                Random r = new Random();
                List<wordBank> filteredWords = words; // Initialize with all words

                // Category filter
                if (!string.IsNullOrEmpty(category) && category.ToLower() != "all")//if there is an 'all' category
                {
                    filteredWords = filteredWords.Where(w => w.Category.ToLower() == category.ToLower()).ToList();
                }
                // Apply difficulty
                if (level == 1) // Easy
                {
                    filteredWords = filteredWords
                        .Where(w => !w.Word.Contains("z") && !w.Word.Contains("y") && !w.Word.Contains("x") && !w.Word.Contains("q") && !w.Word.Contains("w") && !w.Word.Contains("v"))
                        .Where(w => w.Word.Length <= 5)
                        .ToList();
                }
                else if (level == 2) // Medium
                {
                    filteredWords = filteredWords
                        .Where(w => !w.Word.Contains("z") && !w.Word.Contains("x") && !w.Word.Contains("q"))
                        .Where(w => w.Word.Length > 4 && w.Word.Length <= 7)
                        .ToList();
                }
                else if (level == 3) // Hard
                {
                    filteredWords = filteredWords
                        .Where(w => (w.Word.Length > 7) || (w.Word.Length > 4 && (w.Word.Contains("z") || w.Word.Contains("x") || w.Word.Contains("v") || w.Word.Contains("w")) && w.Word.Contains("q")))
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


    }
}
