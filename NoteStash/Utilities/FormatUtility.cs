using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace NoteStash.Utilities;

/// <summary>
/// Utility with functions to format and sort strings.
/// </summary>
public static class FormatUtility
{
	private static readonly string[] _lineDelimiters = { "\r\n", "\n", "\r" };

	/// <summary>
	/// Trims whitespace from the start and end of its argument.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>A string trimmed of whitespace at start and end.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static string Trim(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		return text.Trim();
	}

	/// <summary>
	/// Trims whitespace from the end of its argument.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>A string trimmed of whitespace at its end.</returns>
	public static string TrimEnd(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		return text.TrimEnd();
	}

	/// <summary>
	/// Trims whitespace from the start of its argument.
	/// </summary>
	/// <param name="text">String argument.</param>
	/// <returns>A string trimmed of whitespace at its start.</returns>
	public static string TrimStart(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		return text.TrimStart();
	}

	/// <summary>
	/// Trims whitespace from the start and end of each line in the string. It splits the string according to common
	/// newline characters and then rejoins using <see cref="Environment.NewLine"/>.
	/// </summary>
	/// <param name="text">String argument, supposedly multiline.</param>
	/// <returns>A string trimmed of whitespace at start and end of each line.</returns>
	/// <remarks>Uses <see cref="Environment.NewLine"/> to join back the string after it has been split.</remarks>
	public static string TrimLines(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);
		lines = lines.Select(line => line.Trim()).ToArray();
		return string.Join(Environment.NewLine, lines);
	}

	/// <summary>
	/// Trims whitespace from the start of each line in the string. It splits the string according to common
	/// newline characters and then rejoins using <see cref="Environment.NewLine"/>.
	/// </summary>
	/// <param name="text">String argument. Supposedly multiline.</param>
	/// <returns>A string trimmed of whitespace at start of each line.</returns>
	/// <remarks>Uses <see cref="Environment.NewLine"/> to join back the string after it has been split.</remarks>
	public static string TrimLineStarts(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);
		lines = lines.Select(line => line.TrimStart()).ToArray();
		return string.Join(Environment.NewLine, lines);
	}

	/// <summary>
	/// Trims whitespace from the end of each line in the string. It splits the string according to common
	/// newline characters and then rejoins using <see cref="Environment.NewLine"/>.
	/// </summary>
	/// <param name="text">String argument. Supposedly multiline.</param>
	/// <returns>A string trimmed of whitespace at end of each line.</returns>
	/// <remarks>Uses <see cref="Environment.NewLine"/> to join back the string after it has been split.</remarks>
	public static string TrimLineEnds(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);
		lines = lines.Select(line => line.TrimEnd()).ToArray();
		return string.Join(Environment.NewLine, lines);
	}

	/// <summary>
	/// Sorts the words in the input <see cref="string"/> in ascending order. Best used with single line strings
	/// with only whitespace as a word separator. Sorts escape sequences and punctuation as well if not separated
	/// from a word.
	/// </summary>
	/// <param name="text">String argument. Supposedly single line.</param>
	/// <returns>A <see cref="string"/> with words in ascending order. A word is something delineated by whitespace.
	/// </returns>
	/// <remarks>Sorts escape sequences such as new lines and tabs along with punctuation and other non-alphanumeric
	/// characters. Therefore, this should be used with single line strings most of the time with only whitespace
	/// separated words. Whitespace will also be trimmed. Sorting of some escape sequences is not deterministic.
	/// </remarks>
	public static string SortWordsAsc(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		return string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries).OrderBy(w => w));
	}

	/// <summary>
	/// Sorts the words in the input <see cref="string"/> in descending order. Best used with single line strings
	/// with only whitespace as a word separator. Sorts escape sequences and punctuation as well if not separated
	/// from a word.
	/// </summary>
	/// <param name="text">String argument. Supposedly single line.</param>
	/// <returns>A <see cref="string"/> with words in descending order. A word is something delineated by whitespace.
	/// </returns>
	/// <remarks>Sorts escape sequences such as new lines and tabs along with punctuation and other non-alphanumeric
	/// characters. Therefore, this should be used with single line strings most of the time with only whitespace
	/// separated words. Whitespace will also be trimmed. Sorting of some escape sequences is not deterministic.
	/// </remarks>
	public static string SortWordsDesc(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		return string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries).OrderByDescending(w => w));
	}

	/// <summary>
	/// Sorts the lines in the input <see cref="string"/> in ascending order with no modification to content or
	/// whitespace. Best used with multiline strings. If provided a single line string, does not sort the words in
	/// the strings and returns it as is.
	/// </summary>
	/// <param name="text">String argument. Supposedly  multiline.</param>
	/// <returns>A <see cref="string"/> with lines in ascending order. Lines are separated by \n, \r, or \r\n on
	/// Windows. If the string contains one line, then it will be returned unchanged.
	/// </returns>
	public static string SortLinesAsc(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);
		lines = lines.OrderBy(line => line).ToArray();
		return string.Join(Environment.NewLine, lines);
	}

	/// <summary>
	/// Sorts the lines in the input <see cref="string"/> in descending order with no modification to content or
	/// whitespace. Best used with multiline strings. If provided a single line string, does not sort the words in
	/// the strings and returns it as is.
	/// </summary>
	/// <param name="text">String argument. Supposedly  multiline.</param>
	/// <returns>A <see cref="string"/> with lines in descending order. Lines are separated by \n, \r, or \r\n on
	/// Windows. If the string contains one line, then it will be returned unchanged.
	/// </returns>
	public static string SortLinesDesc(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);
		lines = lines.OrderByDescending(line => line).ToArray();
		return string.Join(Environment.NewLine, lines);
	}

	/// <summary>
	/// Normalizes vertical whitespace such that if there is more than a single empty line separating
	/// two paragraphs, they are reduced to one empty line.
	/// </summary>
	/// <param name="text">The text to be normalized.</param>
	public static string NormalizeVerticalWhitespace(string text)
	{
		ArgumentNullException.ThrowIfNull(text);

		// Regex matches \n or \r\n strings 2 or more times
		string normalizedText = Regex.Replace(text, @"(\r?\n){2,}", "\n\n");
		return normalizedText;
	}

	/// <summary>
	/// Joins lines together by normalizing them
	/// and replacing new line characters with a single space.
	/// </summary>
	/// <param name="text">Text with lines to be joined.</param>
	public static string JoinLines(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string normalizedText = NormalizeVerticalWhitespace(text);

		// Regex matches \n or \r\n strings 1 or more times
		string joinedLines = Regex.Replace(normalizedText, @"(\r?\n)+", " ");
		return joinedLines;
	}

	/// <summary>
	/// Normalizes horizontal whitespace such that if there is more than a single space character or 
	/// tab character between two words, they are replaced by a single space character.
	/// </summary>
	/// <param name="text">The text to be normalized.</param>
	/// <returns></returns>
	public static string NormalizeHorizontalWhitespace(string text)
	{
		ArgumentNullException.ThrowIfNull(text);

		// Regex matches 1 or more characters of spaces, tabs, or tabs and spaces	
		string normalizedText = Regex.Replace(text, "[ \t]+", " ");
		return normalizedText;
	}

	/// <summary>
	/// Splits words on every space character after normalizing horizontal whitespace.
	/// </summary>
	/// <param name="text">Text with words to be split.</param>
	public static string SplitWords(string text)
	{
		ArgumentNullException.ThrowIfNull(text);
		string normalizedText = NormalizeHorizontalWhitespace(text);
		string splitLines = Regex.Replace(normalizedText, " ", "\n");
		return splitLines;
	}

	/// <summary>
	/// Inserts the second argument string at the beginning of the lines of the first argument.
	/// </summary>
	/// <param name="text">Text to format.</param>
	/// <param name="insertion">Text to insert.</param>
	public static string InsertTextAtLineStarts(string text, string insertion)
	{
		ArgumentNullException.ThrowIfNull(text);
		ArgumentNullException.ThrowIfNull(insertion);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);

		// Regex matches 1 or more digit characters
		Match digitMatch = Regex.Match(insertion, @"\d+");
		if (digitMatch.Success && int.TryParse(digitMatch.Value, out int number))
		{
			string[] insertionParts = insertion.Split(digitMatch.Value);
			List<string> newString = new();
			for (int i = 0; i < lines.Length; i++)
			{
				newString.Add($"{insertionParts[0]}{number + i}{insertionParts[1]}{lines[i]}");
			}
			return string.Join("\n", newString);
		}
		else return string.Join("\n", lines.Select(line => insertion + line));
	}

	/// <summary>
	/// Inserts the second argument string at the end of the lines of the first argument.
	/// </summary>
	/// <param name="text">Text to format.</param>
	/// <param name="insertion">Text to insert.</param>
	public static string InsertTextAtLineEnds(string text, string insertion)
	{
		ArgumentNullException.ThrowIfNull(text);
		ArgumentNullException.ThrowIfNull(insertion);
		string[] lines = text.Split(_lineDelimiters, StringSplitOptions.None);
		return string.Join("\n", lines.Select(line => line + insertion));
	}
}