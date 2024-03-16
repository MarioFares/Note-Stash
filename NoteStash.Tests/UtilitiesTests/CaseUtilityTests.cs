using NoteStash.Utilities;
using System;
using Xunit;

namespace NoteStash.Tests.UtilitiesTests;

/// <summary>
/// Tests for the <see cref="CaseUtility"/> class. UpperCase and LowerCase are not thoroughly tested because they
/// only rely on the built-in corresponding functions.
/// </summary>
public class CaseUtilityTests
{
	#region SentenceCase

	[Fact]
	public void SentenceCase_EmptyString_ReturnsEmptyString()
	{
		string actual = CaseUtility.SentenceCase(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void SentenceCase_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => CaseUtility.SentenceCase(null!));

	[Theory]
	[InlineData("t", "T")]
	[InlineData("te", "Te")]
	[InlineData("I test", "I test")]
	[InlineData("test", "Test")]
	[InlineData("TEST", "Test")]
	[InlineData("tEsT", "Test")]
	[InlineData("Test Test", "Test test")]
	[InlineData("test TEST", "Test test")]
	[InlineData("tEst TeST", "Test test")]
	[InlineData("tEst TeST test", "Test test test")]
	[InlineData("tEst TeST test TEST", "Test test test test")]
	public void SentenceCase_SimpleStrings_ReturnsSentenceCase(string input, string expected) =>
		Assert.Equal(expected, CaseUtility.SentenceCase(input));

	[Theory]
	[InlineData("123", "123")]
	[InlineData("tesT123", "Test123")]
	[InlineData("!tEst123", "!test123")]
	[InlineData("!te@st123", "!te@st123")]
	[InlineData("1234567890!@#$%^&*()", "1234567890!@#$%^&*()")]
	[InlineData("{}()[]\"'", "{}()[]\"'")]
	[InlineData(".,/<>?|\\", ". ,/<>?|\\")]
	public void SentenceCase_NonalphaCharacters_ReturnsSameCharactersExceptPeriod(string input, string expected) =>
		Assert.Equal(expected, CaseUtility.SentenceCase(input));

	[Theory]
	[InlineData("tEst\n123", "Test\n123")]
	[InlineData("tEst\ntest", "Test\ntest")]
	[InlineData("TeSt\n\ttest\t    test", "Test\n\ttest\t test")]
	[InlineData("tesT\r\n\ttest\t    test", "Test\r\n\ttest\t test")]
	[InlineData("Test.\n test123 \n\n\n", "Test. \n test123")]
	[InlineData("Test.\n test123 \n\n\n. What to do?", "Test. \n test123 \n\n\n. What to do?")]
	public void SentenceCase_MultilineStrings_ReturnsTrimmedNewlinePreserved(string input, string expected) =>
		Assert.Equal(expected, CaseUtility.SentenceCase(input));

	[Theory]
	[InlineData("  I am.    ", "I am.")]
	[InlineData("  I am.    i.", "I am. I.")]
	[InlineData("this   is   a   test.    ", "This is a test.")]
	[InlineData("    this   is   a   test.    ", "This is a test.")]
	[InlineData("This is a test.", "This is a test.")]
	[InlineData("this is a test.", "This is a test.")]
	[InlineData("this is a test", "This is a test")]
	[InlineData("this is   a   test.", "This is a test.")]
	[InlineData("this is   a   test. this is a test.", "This is a test. This is a test.")]
	[InlineData("this is   a   test.   this is a test.", "This is a test. This is a test.")]
	[InlineData("this   is   a   test.   this is   a test.", "This is a test. This is a test.")]
	[InlineData("this   is   a   test.   this is   a test. it is.", "This is a test. This is a test. It is.")]
	[InlineData("  this   is   a   test.   this is   a test. it is.   ", "This is a test. This is a test. It is.")]
	public void SentenceCase_SingleDelimitedStrings_ReturnsTrimmedAndProperSpaces(string input, string expected) =>
		Assert.Equal(expected, CaseUtility.SentenceCase(input));

	#endregion

	#region RandomCase

	[Fact]
	public void RandomCase_EmptyString_ReturnsEmptyString()
	{
		string actual = CaseUtility.RandomCase(string.Empty);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void RandomCase_NullString_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => CaseUtility.RandomCase(null!));

	#endregion
}