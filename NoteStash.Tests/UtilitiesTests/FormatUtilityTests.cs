using NoteStash.Utilities;
using System;
using Xunit;

namespace NoteStash.Tests.UtilitiesTests;

/// <summary>
/// Tests for the <see cref="FormatUtility"/> class. Trim, TrimStart, and TrimEnd are not tested because they rely
/// only on the built-in corresponding functions. Any function that works on lines splits the strings according
/// to the a list of line delimiters that are \n \r\n and \r. The functions then join back the strings with the
/// <see cref="Environment.NewLine"/> character. Since that character cannot be used for testing and the
/// utilities are targeting Windows for now, the tests will use '\r\n' which is the carriage return character
/// entered into text when the user presses the Enter key on Windows.
/// </summary>
public class FormatUtilityTests
{
	#region Trim TrimStart TrimEnd

	[Fact]
	public void Trim_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.Trim(null!));

	[Fact]
	public void TrimStart_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.TrimStart(null!));

	[Fact]
	public void TrimEnd_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.TrimEnd(null!));

	#endregion

	#region TrimLines

	[Fact]
	public void TrimLines_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.TrimLines(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void TrimLines_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.TrimLines(null!));

	[Theory]
	[InlineData("", "")]
	[InlineData(" ", "")]
	[InlineData("  ", "")]
	[InlineData("  t ", "t")]
	[InlineData("  test ", "test")]
	[InlineData("  test test ", "test test")]
	[InlineData("  test\ttest ", "test\ttest")]
	[InlineData("  test  test ", "test  test")]
	public void TrimLines_SingleLineStrings_ReturnsTrimmedString(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLines(input));

	[Theory]
	[InlineData("\t", "")]
	[InlineData("\v", "")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r\n")]
	[InlineData("\n", "\r\n")]
	public void TrimLines_EscapeSequences_ReturnsEscapeSequencesExceptTabsAndNewLines(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLines(input));

	[Theory]
	[InlineData("\r\n", "\r\n")]
	[InlineData("test\r\ntest", "test\r\ntest")]
	[InlineData(" test \r\n test ", "test\r\ntest")]
	[InlineData("test\r\n     test      ", "test\r\ntest")]
	[InlineData(" test    \r\n     test      ", "test\r\ntest")]
	[InlineData("test\t\r\n     test      \t", "test\r\ntest")]
	[InlineData("\ttest\t\r\n     test      \t", "test\r\ntest")]
	[InlineData(" test \r\n test \r\n test \r\n test", "test\r\ntest\r\ntest\r\ntest")]
	public void TrimLines_MultilineStrings_ReturnsTrimmedString(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLines(input));

	#endregion

	#region TrimLinesStart

	[Fact]
	public void TrimLinesStart_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.TrimLineStarts(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void TrimLinesStart_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.TrimLineStarts(null!));

	[Theory]
	[InlineData("", "")]
	[InlineData(" ", "")]
	[InlineData("  ", "")]
	[InlineData("  t ", "t ")]
	[InlineData("  test ", "test ")]
	[InlineData("  test test ", "test test ")]
	[InlineData("  test\ttest ", "test\ttest ")]
	[InlineData("  test  test ", "test  test ")]
	public void TrimLinesStart_SingleLineStrings_ReturnsTrimmedStringStart(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLineStarts(input));

	[Theory]
	[InlineData("\t", "")]
	[InlineData("\v", "")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r\n")]
	[InlineData("\n", "\r\n")]
	public void TrimLinesStart_EscapeSequences_ReturnsEscapeSequencesExceptTabsAndNewLines(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLineStarts(input));

	[Theory]
	[InlineData("\r\n", "\r\n")]
	[InlineData("test\r\ntest", "test\r\ntest")]
	[InlineData(" test \r\n test ", "test \r\ntest ")]
	[InlineData("test\r\n     test      ", "test\r\ntest      ")]
	[InlineData(" test    \r\n     test      ", "test    \r\ntest      ")]
	[InlineData("test\t\r\n     test      \t", "test\t\r\ntest      \t")]
	[InlineData("\ttest\t\r\n     test      \t", "test\t\r\ntest      \t")]
	[InlineData(" test \r\n test \r\n test \r\n test ", "test \r\ntest \r\ntest \r\ntest ")]
	public void TrimLinesStart_MultilineStrings_ReturnsTrimmedStringStart(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLineStarts(input));

	#endregion

	#region TrimLinesEnd

	[Fact]
	public void TrimLinesEnd_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.TrimLineEnds(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void TrimLinesEnd_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.TrimLineEnds(null!));

	[Theory]
	[InlineData("", "")]
	[InlineData(" ", "")]
	[InlineData("  ", "")]
	[InlineData("  t ", "  t")]
	[InlineData("  test ", "  test")]
	[InlineData("  test test ", "  test test")]
	[InlineData("  test\ttest ", "  test\ttest")]
	[InlineData("  test  test ", "  test  test")]
	public void TrimLinesEnd_SingleLineStrings_ReturnsTrimmedStringEnd(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLineEnds(input));

	[Theory]
	[InlineData("\t", "")]
	[InlineData("\v", "")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r\n")]
	[InlineData("\n", "\r\n")]
	public void TrimLinesEnd_EscapeSequences_ReturnsEscapeSequencesExceptTabsAndNewLines(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLineEnds(input));

	[Theory]
	[InlineData("\r\n", "\r\n")]
	[InlineData("test\r\ntest", "test\r\ntest")]
	[InlineData(" test \r\n test ", " test\r\n test")]
	[InlineData("test\r\n     test      ", "test\r\n     test")]
	[InlineData(" test    \r\n     test      ", " test\r\n     test")]
	[InlineData("test\t\r\n     test      \t", "test\r\n     test")]
	[InlineData("\ttest\t\r\n     test      \t", "\ttest\r\n     test")]
	[InlineData(" test \r\n test \r\n test \r\n test ", " test\r\n test\r\n test\r\n test")]
	public void TrimLinesEnd_MultilineStrings_ReturnsTrimmedStringEnd(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.TrimLineEnds(input));

	#endregion

	#region SortWordsAsc

	[Fact]
	public void SortWordsAsc_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.SortWordsAsc(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void SortWordsAsc_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.SortWordsAsc(null!));

	[Theory]
	[InlineData(" ", "")]
	[InlineData("t", "t")]
	[InlineData("T t", "t T")]
	[InlineData("Test", "Test")]
	[InlineData("Test    Test", "Test Test")]
	[InlineData("  Test    Test  ", "Test Test")]
	[InlineData("This is a Test", "a is Test This")]
	[InlineData("This is a Test!", "a is Test! This")]
	[InlineData("_ 0 1 3 2 d c b a e A C B D E", "_ 0 1 2 3 a A b B c C d D e E")]
	[InlineData("test sentence , . ! ?", ", ! ? . sentence test")]
	public void SortWordsAsc_SimpleStrings_ReturnsCorrectOrder(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsAsc(input));

	[Theory]
	[InlineData("\t", "\t")]
	[InlineData("\v", "\v")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r")]
	[InlineData("\n", "\n")]
	[InlineData("\n \r \a \b \t \v", "\a \b \t \n \v \r")]
	[InlineData("\r \n \b \a \v \t", "\b \a \t \n \v \r")]
	public void SortWordsAsc_EscapeSequences_ReturnsEscapeSequences(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsAsc(input));

	[Theory]
	[InlineData("test \t test", "\t test test")]
	[InlineData("test\t test", "test test\t")]
	[InlineData("test \t \v test", "\t \v test test")]
	public void SortWordsAsc_WordsWithEscapeSequences_ReturnsEscapeSequencesFirst(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsAsc(input));

	[Theory]
	[InlineData("test a\n test a", "a a\n test test")]
	[InlineData("test a \n test a", "\n a a test test")]
	[InlineData("test a \n test a \n \n", "\n \n \n a a test test")]
	public void SortWordsAsc_MultilineStrings_ReturnsCorrectOrder(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsAsc(input));

	#endregion

	#region SortWordsDesc

	[Fact]
	public void SortWordsDesc_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.SortWordsDesc(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void SortWordsDesc_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.SortWordsDesc(null!));

	[Theory]
	[InlineData(" ", "")]
	[InlineData("t", "t")]
	[InlineData("T t", "T t")]
	[InlineData("Test", "Test")]
	[InlineData("Test    Test", "Test Test")]
	[InlineData("  Test    Test  ", "Test Test")]
	[InlineData("This is a Test", "This Test is a")]
	[InlineData("This is a Test!", "This Test! is a")]
	[InlineData("_ 0 1 3 2 d c b a e A C B D E", "E e D d C c B b A a 3 2 1 0 _")]
	[InlineData("test sentence , . ! ?", "test sentence . ? ! ,")]
	public void SortWordsDesc_SimpleStrings_ReturnsCorrectOrder(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsDesc(input));

	// \a \b seem to have same precedence
	[Theory]
	[InlineData("\t", "\t")]
	[InlineData("\v", "\v")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r")]
	[InlineData("\n", "\n")]
	[InlineData("\n \r \a \b \t \v", "\r \v \n \t \a \b")]
	[InlineData("\r \n \b \a \v \t", "\r \v \n \t \b \a")]
	public void SortWordsDesc_EscapeSequences_ReturnsEscapeSequences(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsDesc(input));

	[Theory]
	[InlineData("test \t test", "test test \t")]
	[InlineData("test\t test", "test\t test")]
	[InlineData("test \t \v test", "test test \v \t")]
	public void SortWordsDesc_WordsWithEscapeSequences_ReturnsEscapeSequencesFirst(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsDesc(input));

	[Theory]
	[InlineData("test a\n test a", "test test a\n a")]
	[InlineData("test a \n test a", "test test a a \n")]
	[InlineData("test a \n test a \n \n", "test test a a \n \n \n")]
	public void SortWordsDesc_MultilineStrings_ReturnsCorrectOrder(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortWordsDesc(input));

	#endregion

	#region SortLinesAsc

	[Fact]
	public void SortLinesAsc_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.SortLinesAsc(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void SortLinesAsc_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.SortLinesAsc(null!));

	[Theory]
	[InlineData(" ")]
	[InlineData("t")]
	[InlineData("test")]
	[InlineData("test a")]
	[InlineData("A simple sentence \t")]
	public void SortLinesAsc_SingleLineStrings_ReturnsStringUnchanged(string input) =>
		Assert.Equal(input, FormatUtility.SortLinesAsc(input));

	[Theory]
	[InlineData("\t", "\t")]
	[InlineData("\v", "\v")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r\n")]
	[InlineData("\n", "\r\n")]
	public void SortLinesAsc_EscapeSequences_ReturnsEscapeSequencesExceptNewLine(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortLinesAsc(input));

	[Theory]
	[InlineData("a\n a", " a\r\na")]
	[InlineData("test\n a", " a\r\ntest")]
	[InlineData("wills\n of\n test\n a", " a\r\n of\r\n test\r\nwills")]
	[InlineData("The\nquick\nbrown\nfox\njumped.", "brown\r\nfox\r\njumped.\r\nquick\r\nThe")]
	[InlineData("3. test \r\n 2. test \r\n 1. test", " 1. test\r\n 2. test \r\n3. test ")]
	public void SortLinesAsc_MultilineStrings_ReturnsCorrectOrderElseUnchanged(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortLinesAsc(input));

	#endregion

	#region SortLinesDesc

	[Fact]
	public void SortLinesDesc_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.SortLinesDesc(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void SortLinesDesc_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.SortLinesDesc(null!));

	[Theory]
	[InlineData(" ")]
	[InlineData("t")]
	[InlineData("test")]
	[InlineData("test a")]
	[InlineData("A simple sentence \t")]
	public void SortLinesDesc_SingleLineStrings_ReturnsStringUnchanged(string input) =>
		Assert.Equal(input, FormatUtility.SortLinesDesc(input));

	[Theory]
	[InlineData("\t", "\t")]
	[InlineData("\v", "\v")]
	[InlineData("\a", "\a")]
	[InlineData("\b", "\b")]
	[InlineData("\r", "\r\n")]
	[InlineData("\n", "\r\n")]
	public void SortLinesDesc_EscapeSequences_ReturnsEscapeSequencesExceptNewLine(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortLinesDesc(input));

	[Theory]
	[InlineData("a\n a", "a\r\n a")]
	[InlineData("test\n a", "test\r\n a")]
	[InlineData("wills\n of\n test\n a", "wills\r\n test\r\n of\r\n a")]
	[InlineData("The\nquick\nbrown\nfox\njumped.", "The\r\nquick\r\njumped.\r\nfox\r\nbrown")]
	[InlineData("3. test \r\n 2. test \r\n 1. test", "3. test \r\n 2. test \r\n 1. test")]
	public void SortLinesDesc_MultilineStrings_ReturnsCorrectOrderElseUnchanged(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SortLinesDesc(input));

	#endregion

	#region NormalizeVerticalWhitespace

	[Fact]
	public void NormalizeVerticalWhitespace_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.NormalizeVerticalWhitespace(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void NormalizeVerticalWhitespace_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.NormalizeVerticalWhitespace(null!));

	[Theory]
	[InlineData(" ")]
	[InlineData("Test")]
	[InlineData("Text\nTest")]
	[InlineData("Text\n\nTest")]
	public void NormalizeVerticalWhitespace_NormalizedStrings_ReturnsStringsUnchanged(string input) =>
		Assert.Equal(input, FormatUtility.NormalizeVerticalWhitespace(input));

	[Theory]
	[InlineData("Test\n\n\nTest", "Test\n\nTest")]
	[InlineData("Test\n\n\n\nTest", "Test\n\nTest")]
	public void NormalizeVerticalWhitespace_StringsWithSeveralEmptyLines_ReturnsNormalizedStrings(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.NormalizeVerticalWhitespace(input));

	#endregion

	#region JoinLines

	[Fact]
	public void JoinLines_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.JoinLines(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void JoinLines_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.JoinLines(null!));

	[Theory]
	[InlineData(" ")]
	[InlineData("Test")]
	public void JoinLines_SimpleStrings_ReturnsStringsUnchanged(string input) =>
		Assert.Equal(input, FormatUtility.JoinLines(input));

	[Theory]
	[InlineData("test\ntest", "test test")]
	[InlineData("test\n\ntest", "test test")]
	[InlineData("test\n\n\n\ntest", "test test")]
	[InlineData("test\ntest\ntest", "test test test")]
	public void JoinLines_MultilineStrings_ReturnsJoinedStrings(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.JoinLines(input));

	#endregion

	#region NormalizeHorizontalWhitespace

	[Fact]
	public void NormalizeHorizontalWhitespace_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.NormalizeHorizontalWhitespace(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void NormalizeHorizontalWhitespace_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.NormalizeHorizontalWhitespace(null!));

	[Theory]
	[InlineData(" ")]
	[InlineData("Test")]
	[InlineData("Text\nTest")]
	[InlineData("Text\n\nTest")]
	public void NormalizeHorizontalWhitespace_NormalizedStrings_ReturnsStringsUnchanged(string input) =>
		Assert.Equal(input, FormatUtility.NormalizeHorizontalWhitespace(input));

	[Theory]
	[InlineData("Test  Test", "Test Test")]
	[InlineData("Test   Test", "Test Test")]
	[InlineData(" Test   Test ", " Test Test ")]
	[InlineData("   Test   Test   ", " Test Test ")]
	public void NormalizeHorizontalWhitespace_StringsWithSeveralSpaceCharacters_ReturnsNormalizedStrings(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.NormalizeHorizontalWhitespace(input));

	[Theory]
	[InlineData("Test\tTest", "Test Test")]
	[InlineData("Test\t\tTest", "Test Test")]
	[InlineData("Test \tTest", "Test Test")]
	[InlineData("\t\tTest\tTest\t\t", " Test Test ")]
	public void NormalizeHorizontalWhitespace_StringsWithSeveralTabCharacters_ReturnsNormalizedStrings(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.NormalizeHorizontalWhitespace(input));

	#endregion

	#region SplitWords

	[Fact]
	public void SplitWords_EmptyString_ReturnsEmptyString()
	{
		string actual = FormatUtility.SplitWords(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void SplitWords_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.SplitWords(null!));

	[Fact]
	public void SplitWords_OneWord_ReturnsStringsUnchanged() =>
		Assert.Equal("Test", FormatUtility.SplitWords("Test"));
	

	[Theory]
	[InlineData("test test", "test\ntest")]
	[InlineData("test test test", "test\ntest\ntest")]
	[InlineData("test      test", "test\ntest")]
	[InlineData(" test  test ", "\ntest\ntest\n")]
	public void SplitWords_MultispaceStrings_ReturnsSplitStrings(string input, string expected) =>
		Assert.Equal(expected, FormatUtility.SplitWords(input));

	#endregion

	#region InsertTextAtLineStarts

	[Fact]
	public void InsertTextAtLineStarts_EmptyStrings_ReturnsEmptyString()
	{
		string actual = FormatUtility.InsertTextAtLineStarts(string.Empty, string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void InsertTextAtLineStarts_NullStrings_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.InsertTextAtLineStarts(null!, null!));

	[Fact]
	public void InsertTextAtLineStarts_OneLine_ReturnsStringChanged()
	{
		string actual = FormatUtility.InsertTextAtLineStarts("test", "0");

		Assert.Equal("0test", actual);
	}

	[Theory]
	[InlineData("test\ntest", "$", "$test\n$test")]
	[InlineData("test\ntest", "&", "&test\n&test")]
	[InlineData("test\n test", "&", "&test\n& test")]
	[InlineData("test\ntest\n", "&", "&test\n&test\n&")]
	public void InsertTextAtLineStarts_MultiLineStrings_ReturnsProperStrings(string text, string insertion, string expected) =>
		Assert.Equal(expected, FormatUtility.InsertTextAtLineStarts(text, insertion));

	[Theory]
	[InlineData("test\ntest", "1", "1test\n2test")]
	[InlineData("test\ntest\ntest", "1", "1test\n2test\n3test")]
	[InlineData("test\ntest", "2", "2test\n3test")]
	[InlineData("test\ntest\n", "1", "1test\n2test\n3")]
	public void InsertTextAtLineStarts_MultiLineStringsWithIntPattern_ReturnsOrderedList(string text, string insertion, string expected) =>
		Assert.Equal(expected, FormatUtility.InsertTextAtLineStarts(text, insertion));

	#endregion

	#region InsertTextAtLineEnds

	[Fact]
	public void InsertTextAtLineEnds_EmptyStrings_ReturnsEmptyString()
	{
		string actual = FormatUtility.InsertTextAtLineEnds(string.Empty, string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void InsertTextAtLineEnds_NullStrings_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => FormatUtility.InsertTextAtLineEnds(null!, null!));

	[Fact]
	public void InsertTextAtLineEnds_OneLine_ReturnsStringChanged()
	{
		string actual = FormatUtility.InsertTextAtLineEnds("test", "0");

		Assert.Equal("test0", actual);
	}

	[Theory]
	[InlineData("test\ntest", "0", "test0\ntest0")]
	[InlineData("test\ntest", "1", "test1\ntest1")]
	[InlineData("test\n test", "1", "test1\n test1")]
	[InlineData("test\ntest\n", "1", "test1\ntest1\n1")]
	public void InsertTextAtLineEnds_MultiLineStrings_ReturnsProperStrings(string text, string insertion, string expected) =>
		Assert.Equal(expected, FormatUtility.InsertTextAtLineEnds(text, insertion));

	#endregion
}