using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionaries;

public class Program
{
	public static void Main()
	{
		bool running = true;
		while (running)
		{
			Console.Clear();
			Console.WriteLine("Меню:");
			Console.WriteLine("1. Создать словарь");
			Console.WriteLine("2. Добавить слово и перевод");
			Console.WriteLine("3. Заменить перевод слова");
			Console.WriteLine("4. Удалить слово");
			Console.WriteLine("5. Удалить перевод");
			Console.WriteLine("6. Найти перевод слова");
			Console.WriteLine("7. Экспортировать слово в файл");
			Console.WriteLine("8. Сохранить словари в файл");
			Console.WriteLine("9. Загрузить словари из файла");
			Console.WriteLine("0. Выйти");
			Console.Write("Выберите опцию: ");
			string option = Console.ReadLine();

			switch (option)
			{
				case "1":
					CreateDictionary();
					break;
				case "2":
					AddWord();
					break;
				case "3":
					ReplaceTranslation();
					break;
				case "4":
					RemoveWord();
					break;
				case "5":
					RemoveTranslation();
					break;
				case "6":
					FindWord();
					break;
				case "7":
					ExportWord();
					break;
				case "8":
					SaveDictionaries();
					break;
				case "9":
					LoadDictionaries();
					break;
				case "0":
					running = false;
					break;
			}
		}
    }
	static void CreateDictionary()
	{
		Console.Write("Введите название словаря (например, Англо-Русский): ");
		string name = Console.ReadLine();
		dictionaryManager.CreateDictionary(name);
		Console.WriteLine("Словарь создан.");
	}

	static void AddWord()
	{
		Console.Write("Введите название словаря: ");
		string dictionaryName = Console.ReadLine();
		var dictionary = dictionaryManager.GetDictionary(dictionaryName);

		if (dictionary != null)
		{
			Console.Write("Введите слово: ");
			string word = Console.ReadLine();
			Console.Write("Введите перевод: ");
			string translation = Console.ReadLine();
			dictionary.AddWord(word, translation);
			Console.WriteLine("Слово добавлено.");
		}
		else
		{
			Console.WriteLine("Словарь не найден.");
		}
	}

}

public class WordEntry
{
	public string Word { get; set; }
	public List<string> Translations { get; set; }

	public WordEntry(string word)
	{
		Word = word;
		Translations = new List<string>();
	}

	public void AddTranslation(string translation)
	{
		if (!Translations.Contains(translation))
		{
			Translations.Add(translation);
		}
	}

	public void ReplaceTranslation(int index, string newTranslation)
	{
		if (index >= 0 && index < Translations.Count)
		{
			Translations[index] = newTranslation;
		}
	}

	public void RemoveTranslation(string translation)
	{
		if (Translations.Count > 1)
		{
			Translations.Remove(translation);
		}
	}

	public override string ToString()
	{
		return $"{Word}: {string.Join(", ", Translations)}";
	}
}

public class Dictionary
{
	public string Name { get; set; }
	private Dictionary<string, WordEntry> words;

	public Dictionary(string name)
	{
		Name = name;
		words = new Dictionary<string, WordEntry>();
	}

	public void AddWord(string word, string translation)
	{
		if (!words.ContainsKey(word))
		{
			words[word] = new WordEntry(word);
		}

		words[word].AddTranslation(translation);
	}

	public void ReplaceTranslation(string word, int translationIndex, string newTranslation)
	{
		if (words.ContainsKey(word))
		{
			words[word].ReplaceTranslation(translationIndex, newTranslation);
		}
	}

	public void RemoveWord(string word)
	{
		if (words.ContainsKey(word))
		{
			words.Remove(word);
		}
	}

	public void RemoveTranslation(string word, string translation)
	{
		if (words.ContainsKey(word))
		{
			words[word].RemoveTranslation(translation);
		}
	}

	public WordEntry FindWord(string word)
	{
		return words.ContainsKey(word) ? words[word] : null;
	}

	public void ExportWordToFile(string word, string filePath)
	{
		var wordEntry = FindWord(word);
		if (wordEntry != null)
		{
			System.IO.File.WriteAllText(filePath, wordEntry.ToString());
		}
	}

	public override string ToString()
	{
		var result = $"Словарь: {Name}\n";
		foreach (var entry in words)
		{
			result += entry.Value + "\n";
		}

		return result;
	}
}

public class DictionaryManager
{
	private List<Dictionary> dictionaries;

	public DictionaryManager()
	{
		dictionaries = new List<Dictionary>();
	}

	public void CreateDictionary(string name)
	{
		dictionaries.Add(new Dictionary(name));
	}

	public Dictionary GetDictionary(string name)
	{
		return dictionaries.Find(d => d.Name == name);
	}

	public void SaveDictionariesToFile(string filePath)
	{
		using (StreamWriter writer = new StreamWriter(filePath))
		{
			foreach (var dictionary in dictionaries)
			{
				writer.WriteLine(dictionary.ToString());
			}
		}
	}

	public void LoadDictionariesFromFile(string filePath)
	{
		if (File.Exists(filePath))
		{
			dictionaries.Clear();
			var lines = File.ReadAllLines(filePath);
			Dictionary currentDictionary = null;

			foreach (var line in lines)
			{
				if (line.StartsWith("Словарь:"))
				{
					var dictionaryName = line.Replace("Словарь: ", "").Trim();
					currentDictionary = new Dictionary(dictionaryName);
					dictionaries.Add(currentDictionary);
				}
				else if (currentDictionary != null)
				{
					var parts = line.Split(':');
					if (parts.Length > 1)
					{
						var word = parts[0].Trim();
						var translations = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

						foreach (var translation in translations)
						{
							currentDictionary.AddWord(word, translation.Trim());
						}
					}
				}
			}
		}
	}
}