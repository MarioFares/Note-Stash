using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace NoteStash.Utilities;

/// <summary>
/// Utility with functions to change cases of strings.
/// </summary>
public static class CaseUtility
{
	/// <summary>
	/// Takes a string as input and returns its sentence case equivalent. Also splits strings based on '.'
	/// and then rejoins them with a '.' and a single space. It finally trims both the start and end of the
	/// sentence. If the argument empty, it is returned as is.
	/// </summary>
	/// <param name="arg">Any string.</param>
	/// <returns>A sentence case equivalent of the argument.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <remarks>Does not handle new lines or other punctuation. It is up to the application user
	/// to use this method in the right place and in the right text.</remarks>
	public static string SentenceCase(string arg)
	{
		ArgumentNullException.ThrowIfNull(arg);
		if (arg == string.Empty) return arg;

		string[] sentences = arg.Split('.');
		for (int i = 0; i < sentences.Length; ++i)
		{
			string[] words = sentences[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (words.Length == 0) continue;

			words[0] = char.ToUpper(words[0][0]) + words[0].Substring(1).ToLower();
			for (int j = 1; j < words.Length; j++)
			{
				words[j] = words[j].ToLower();
			}

			sentences[i] = string.Join(" ", words);
		}

		return string.Join(". ", sentences).Trim();
	}

	/// <summary>
	/// Takes a string as input and returns a random case equivalent. When run several times,
	/// different outputs should be returned, but not guaranteed.
	/// </summary>
	/// <param name="arg"></param>
	/// <returns></returns>
	public static string RandomCase(string arg)
	{
		ArgumentNullException.ThrowIfNull(arg);
		if (arg == string.Empty) return arg;

		StringBuilder result = new();
		Random random = new();

		foreach (char c in arg)
		{
			if (char.IsLetter(c))
			{
				bool convertToUpper = random.Next(2) == 0;
				char convertedChar = convertToUpper ? char.ToUpper(c) : char.ToLower(c);
				result.Append(convertedChar);
			}
			else
			{
				result.Append(c);
			}
		}

		return result.ToString();
	}
}