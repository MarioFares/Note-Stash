using NoteStash.Utilities;
using Xunit;

namespace NoteStash.Tests.UtilitiesTests;

/// <summary>
/// Tests for the <see cref="TextCountUtilityTests"/> class.
/// </summary>
public class TextCountUtilityTests
{
	#region CharCount

	[Fact]
	public void CharCount_EmptyString_ReturnsZero()
	{
		int actual = TextCountUtility.CharCount(string.Empty);

		Assert.Equal(0, actual);
	}

	[Fact]
	public void CharCount_NullString_ReturnsZero()
	{
		int actual = TextCountUtility.CharCount(null!);

		Assert.Equal(0, actual);
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData(" ", 1)]
	[InlineData("m", 1)]
	[InlineData("  ", 2)]
	[InlineData("mm", 2)]
	[InlineData("Test test", 9)]
	public void CharCount_SimpleStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharCount(input));

	[Theory]
	[InlineData("1234567890", 10)]
	[InlineData("!@#$%^&*()", 10)]
	[InlineData(",./<>?", 6)]
	[InlineData("[]{}\\|", 6)]
	[InlineData(";:'\"", 4)]
	[InlineData("`~", 2)]
	public void CharCount_NonalphaCharacters_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharCount(input));

	[Theory]
	[InlineData("\n", 0)]
	[InlineData("\t", 0)]
	[InlineData("\r", 0)]
	[InlineData("\r\n", 0)]
	[InlineData("\v", 0)]
	[InlineData("\a", 1)]
	[InlineData("\b", 1)]
	public void CharCount_EscapeSequences_ReturnsMostlyZero(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharCount(input));

	[Theory]
	[InlineData("test\n", 4)]
	[InlineData("test\n test", 9)]
	[InlineData("test\n test \t test", 15)]
	[InlineData("test\n test \r\n test", 15)]
	public void CharCount_MultilineStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharCount(input));

	#endregion

	#region CharNoSpaceCount

	[Fact]
	public void CharNoSpaceCount_EmptyString_ReturnsZero()
	{
		int actual = TextCountUtility.CharNoSpaceCount(string.Empty);

		Assert.Equal(0, actual);
	}

	[Fact]
	public void CharNoSpaceCount_NullString_ReturnsZero()
	{
		int actual = TextCountUtility.CharNoSpaceCount(null!);

		Assert.Equal(0, actual);
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData(" ", 0)]
	[InlineData("        ", 0)]
	[InlineData("t", 1)]
	[InlineData("t ", 1)]
	[InlineData("Test Test", 8)]
	[InlineData("Test Test Test", 12)]
	public void CharNoSpaceCount_SimpleStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharNoSpaceCount(input));

	[Theory]
	[InlineData("1234567890", 10)]
	[InlineData("!@#$%^&*()", 10)]
	[InlineData(",./<>?", 6)]
	[InlineData("[]{}\\|", 6)]
	[InlineData(";:'\"", 4)]
	[InlineData("`~", 2)]
	public void CharNoSpaceCount_NonalphaCharacters_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharNoSpaceCount(input));

	[Theory]
	[InlineData("\n", 0)]
	[InlineData("\t", 0)]
	[InlineData("\r", 0)]
	[InlineData("\r\n", 0)]
	[InlineData("\v", 0)]
	[InlineData("\a", 1)]
	[InlineData("\b", 1)]
	public void CharNoSpaceCount_EscapeSequences_ReturnsMostlyZero(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharNoSpaceCount(input));

	[Theory]
	[InlineData("test\n", 4)]
	[InlineData("test\n test", 8)]
	[InlineData("test\n test \t test", 12)]
	[InlineData("test\n test \r\n test", 12)]
	public void CharNoSpaceCount_MultilineStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.CharNoSpaceCount(input));

	#endregion

	#region WordCount

	[Fact]
	public void WordCount_EmptyString_ReturnsZero()
	{
		int actual = TextCountUtility.WordCount(string.Empty);

		Assert.Equal(0, actual);
	}

	[Fact]
	public void WordCount_NullString_ReturnsZero()
	{
		int actual = TextCountUtility.WordCount(null!);

		Assert.Equal(0, actual);
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData("test", 1)]
	[InlineData("test   ", 1)]
	[InlineData("test test", 2)]
	[InlineData("test   test", 2)]
	[InlineData("test test test", 3)]
	[InlineData("test\ttest\ttest", 3)]
	[InlineData("test\ntest\ntest", 3)]
	[InlineData("test\r\ntest\r\ntest", 3)]
	[InlineData("test\vtest\vtest", 3)]
	public void WordCount_SimpleStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.WordCount(input));

	[Theory]
	[InlineData("1234567890", 1)]
	[InlineData("!@#$%^&*()", 1)]
	[InlineData(",./<>?", 1)]
	[InlineData("[]{}\\|", 1)]
	[InlineData(";:'\"", 1)]
	[InlineData("`~", 1)]
	public void WordCount_NonalphaCharacters_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.WordCount(input));

	[Theory]
	[InlineData("\n", 0)]
	[InlineData("\t", 0)]
	[InlineData("\r", 0)]
	[InlineData("\r\n", 0)]
	[InlineData("\v", 0)]
	[InlineData("\a", 1)]
	[InlineData("\b", 1)]
	public void WordCount_EscapeSequences_ReturnsMostlyZero(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.WordCount(input));

	[Theory]
	[InlineData("test\n", 1)]
	[InlineData("test\n test", 2)]
	[InlineData("test\n test \t test", 3)]
	[InlineData("test\n test \r\n test", 3)]
	[InlineData("test\ttest \r\n test test \r\n test", 5)]
	public void WordCount_MultilineStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.WordCount(input));

	#endregion

	#region LineCount

	[Fact]
	public void LineCount_EmptyString_ReturnsZero()
	{
		int actual = TextCountUtility.LineCount(string.Empty);

		Assert.Equal(0, actual);
	}

	[Fact]
	public void LineCount_NullString_ReturnsZero()
	{
		int actual = TextCountUtility.LineCount(null!);

		Assert.Equal(0, actual);
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData(" ", 1)]
	[InlineData("m", 1)]
	[InlineData("  ", 1)]
	[InlineData("mm", 1)]
	[InlineData("Test test", 1)]
	public void LineCount_SimpleStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.LineCount(input));

	[Theory]
	[InlineData("1234567890", 1)]
	[InlineData("!@#$%^&*()", 1)]
	[InlineData(",./<>?", 1)]
	[InlineData("[]{}\\|", 1)]
	[InlineData(";:'\"", 1)]
	[InlineData("`~", 1)]
	public void LineCount_NonalphaCharacters_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.LineCount(input));

	[Theory]
	[InlineData("\n", 2)]
	[InlineData("\t", 1)]
	[InlineData("\r", 1)]
	[InlineData("\r\n", 2)]
	[InlineData("\v", 1)]
	[InlineData("\a", 1)]
	[InlineData("\b", 1)]
	public void LineCount_EscapeSequences_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.LineCount(input));

	[Theory]
	[InlineData("test\n", 2)]
	[InlineData("test\n test", 2)]
	[InlineData("test\n test \t test", 2)]
	[InlineData("test\r\n test \r\n test", 3)]
	[InlineData("test\r\n test \r\n test \r\n", 4)]
	public void LineCount_MultilineStrings_ReturnsCorrectCount(string input, int count) =>
		Assert.Equal(count, TextCountUtility.LineCount(input));

	#endregion

	#region ParagraphCount

	[Fact]
	public void ParagraphCount_EmptyString_ReturnsZero()
	{
		int actual = TextCountUtility.ParagraphCount(string.Empty);

		Assert.Equal(0, actual);
	}

	[Fact]
	public void ParagraphCount_NullString_ReturnsZero()
	{
		int actual = TextCountUtility.ParagraphCount(null!);

		Assert.Equal(0, actual);
	}

	[Theory]
	[InlineData("", 0)]
	[InlineData(" ", 1)]
	[InlineData("m", 1)]
	[InlineData("  ", 1)]
	[InlineData("mm", 1)]
	[InlineData("Test test", 1)]
	public void ParagraphCount_SimpleStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.ParagraphCount(input));

	[Theory]
	[InlineData("1234567890", 1)]
	[InlineData("!@#$%^&*()", 1)]
	[InlineData(",./<>?", 1)]
	[InlineData("[]{}\\|", 1)]
	[InlineData(";:'\"", 1)]
	[InlineData("`~", 1)]
	public void ParagraphCount_NonalphaCharacters_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.ParagraphCount(input));

	[Theory]
	[InlineData("\n", 1)]
	[InlineData("\t", 1)]
	[InlineData("\r", 1)]
	[InlineData("\r\n", 1)]
	[InlineData("\v", 1)]
	[InlineData("\a", 1)]
	[InlineData("\b", 1)]
	public void ParagraphCount_EscapeSequences_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.ParagraphCount(input));

	[Theory]
	[InlineData("test\n", 1)]
	[InlineData("test\n test", 1)]
	[InlineData("test\n test \t test", 1)]
	[InlineData("test\n test \r\n test", 1)]
	public void ParagraphCount_MultilineStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.ParagraphCount(input));

	[Theory]
	[InlineData("test\n\n", 1)]
	[InlineData("test\n\ntest", 2)]
	[InlineData("test\n\ntest\n\ntest", 3)]
	[InlineData("test \n \n test \n\n", 2)]
	[InlineData("test \n \n test \n\n ", 3)]
	public void ParagraphCount_MultiParagrahStrings_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.ParagraphCount(input));

	[Theory]
	[InlineData("test\r\n", 1)]
	[InlineData("test\r\n\r\n", 1)]
	[InlineData("test\r\n\r\n ", 2)]
	[InlineData("test\r\n\r\n t", 2)]
	[InlineData("test\r\n\r\ntest", 2)]
	[InlineData("test\r\n\r\ntest\r\n\r\ntest", 3)]
	[InlineData(" test \r\n \r\n test \r\n \r\n test ", 3)]
	public void ParagraphCount_MultiParagraphStringsWindowsNewline_ReturnsCorrectCount(string input, int expected) =>
		Assert.Equal(expected, TextCountUtility.ParagraphCount(input));

	#endregion
}