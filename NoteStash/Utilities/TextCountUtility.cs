using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NoteStash.Utilities;

/// <summary>
/// Utility with functions to return different text metrics such as word count, line count, etc.
/// </summary>
public static class TextCountUtility
{
	private static readonly string[] _wordDelimiters = { " ", "\r\n", "\n", "\r", "\t", "\v" };

	/// <summary>
	/// Returns the character count of its argument, including spaces.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>Returns an int of how many characters, including spaces, there are in the input string.</returns>
	public static int CharCount(string text)
	{
		if (string.IsNullOrEmpty(text)) return 0;

		int spaceCount = text.Count(c => c == ' ');
		return CharNoSpaceCount(text) + spaceCount;
	}

	/// <summary>
	/// Returns the character count of its argument, excluding spaces.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>Returns an int of how many characters, excluding spaces, there are in the input string.</returns>
	public static int CharNoSpaceCount(string text)
	{
		if (string.IsNullOrEmpty(text)) return 0;

		text = _wordDelimiters.Aggregate(text, (current, del) => current.Replace(del, ""));
		return text.Length;
	}

	/// <summary>
	/// Returns the word count of its argument.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>Returns an int of how many words there are in the input string.</returns>
	/// <remarks>Delimiters including whitespace, tabs, and the different kinds of returns.</remarks>
	public static int WordCount(string text)
	{
		if (string.IsNullOrEmpty(text)) return 0;

		return text.Split(_wordDelimiters, StringSplitOptions.RemoveEmptyEntries).Length;
	}

	/// <summary>
	/// Returns the line count of its argument. If the argument is null or empty, 0 is returned.
	/// Otherwise, if there are no new lines at all but al least one character, 1 is returned.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>Returns an int of how many lines there are in the input string.</returns>
	/// <remarks>
	/// Only looks for '\n'. Empty or null strings return 0. At least a single character
	/// and at least 1 is returned.
	/// </remarks>
	public static int LineCount(string text)
	{
		if (string.IsNullOrEmpty(text)) return 0;

		return text.Count(c => c == '\n') + 1;
	}

	/// <summary>
	/// Returns the paragraph count of its argument. Uses newlines followed by a word character.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>Returns an int of how many paragraphs there are in the input string.</returns>
	public static int ParagraphCount(string text)
	{
		if (string.IsNullOrEmpty(text)) return 0;

		// Regex searches for newline followed by zero or more whitespace characters followed by a newline 
		string[] paragraphs = Regex.Split(text, @"\n\s*\n", RegexOptions.Compiled);
		return paragraphs.Count(p => p != "");
	}
}
