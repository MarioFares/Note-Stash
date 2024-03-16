using NoteStash.Converters;
using System;
using Xunit;

namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="MatchingIntToBooleanConverter"/>
/// </summary>
public class MatchingIntToBooleanConverterTests
{
	/// <summary>
	/// Tests for a group of MenuItems that each have a ConverterParameter specified. The `selected` variable
	/// represents whatever value is selected at a time.
	/// </summary>
	/// <param name="input"></param>
	/// <param name="expected"></param>
	[Theory]
	[InlineData("0", true)]
	[InlineData("2", false)]
	[InlineData("4", false)]
	[InlineData("6", false)]
	[InlineData("8", false)]
	[InlineData("10", false)]
	public void Convert_Zero_ReturnsAllFalseButZero(string input, bool expected)
	{
		// Represents the option/MenuItem that is initially selected
		int selected = 0;
		MatchingIntToBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(selected, null!, input, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("2", true)]
	[InlineData("4", false)]
	[InlineData("6", false)]
	[InlineData("8", false)]
	[InlineData("10", false)]
	public void Convert_Two_ReturnsAllFalseButTwo(string input, bool expected)
	{
		// Represents the option/MenuItem that is initially selected
		int selected = 2;
		MatchingIntToBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(selected, null!, input, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("2", false)]
	[InlineData("4", true)]
	[InlineData("6", false)]
	[InlineData("8", false)]
	[InlineData("10", false)]
	public void Convert_Four_ReturnsAllFalseButFour(string input, bool expected)
	{
		// Represents the option/MenuItem that is initially selected
		int selected = 4;
		MatchingIntToBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(selected, null!, input, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("2", false)]
	[InlineData("4", false)]
	[InlineData("6", true)]
	[InlineData("8", false)]
	[InlineData("10", false)]
	public void Convert_Six_ReturnsAllFalseButSix(string input, bool expected)
	{
		// Represents the option/MenuItem that is initially selected
		int selected = 6;
		MatchingIntToBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(selected, null!, input, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("2", false)]
	[InlineData("4", false)]
	[InlineData("6", false)]
	[InlineData("8", true)]
	[InlineData("10", false)]
	public void Convert_Eight_ReturnsAllFalseButEight(string input, bool expected)
	{
		// Represents the option/MenuItem that is initially selected
		int selected = 8;
		MatchingIntToBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(selected, null!, input, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("0", false)]
	[InlineData("2", false)]
	[InlineData("4", false)]
	[InlineData("6", false)]
	[InlineData("8", false)]
	[InlineData("10", true)]
	public void Convert_Ten_ReturnsAllFalseButTen(string parameter, bool expected)
	{
		// Represents the option/MenuItem that is initially selected
		int selected = 10;
		MatchingIntToBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(selected, null!, parameter, null!);

		Assert.Equal(expected, actual);
	}

	/// <summary>
	/// Tests whether all other options return 0, except for 0, which is the only option that is selected.
	/// </summary>
	/// <param name="parameter">The ConverterParameter of the MenuItem.</param>
	/// <param name="isSelected">Whether the MenuItem is selected.</param>
	/// <param name="expected">The expected return type from the ConvertBack function.</param>
	[Theory]
	[InlineData("0", true, 0)]
	[InlineData("2", false, 0)]
	[InlineData("4", false, 0)]
	[InlineData("6", false, 0)]
	[InlineData("8", false, 0)]
	[InlineData("10", false, 0)]
	public void ConvertBack_ZeroOptionTrue_ReturnsZeroForAllOtherParameters(string parameter, bool isSelected,
		int expected)
	{
		MatchingIntToBooleanConverter converter = new();

		int actual = (int)converter.ConvertBack(isSelected, typeof(int), parameter, null!);

		Assert.Equal(expected, actual);
	}

	/// <summary>
	/// Tests whether all other options return 0, except for 2, which is the only option that is selected.
	/// </summary>
	/// <param name="parameter">The ConverterParameter of the MenuItem.</param>
	/// <param name="isSelected">Whether the MenuItem is selected.</param>
	/// <param name="expected">The expected return type from the ConvertBack function.</param>
	[Theory]
	[InlineData("0", false, 0)]
	[InlineData("2", true, 2)]
	[InlineData("4", false, 0)]
	[InlineData("6", false, 0)]
	[InlineData("8", false, 0)]
	[InlineData("10", false, 0)]
	public void ConvertBack_TwoOptionTrue_ReturnsZeroForAllOtherParameters(string parameter, bool isSelected,
		int expected)
	{
		MatchingIntToBooleanConverter converter = new();

		int actual = (int)converter.ConvertBack(isSelected, typeof(int), parameter, null!);

		Assert.Equal(expected, actual);
	}

	/// <summary>
	/// Tests whether the parameter is always returned as a string. There will no be a case when all options
	/// in a group are true. This tests only the functionality of conversion from <see cref="string"/> to
	/// <see cref="int"/>.
	/// </summary>
	/// <param name="parameter">The ConverterParameter of the MenuItem.</param>
	/// <param name="isSelected">Whether the MenuItem is selected.</param>
	/// <param name="expected">The expected return type from the ConvertBack function.</param>
	[Theory]
	[InlineData("0", true, 0)]
	[InlineData("2", true, 2)]
	[InlineData("4", true, 4)]
	[InlineData("6", true, 6)]
	[InlineData("8", true, 8)]
	[InlineData("10", true, 10)]
	public void ConvertBack_AllOptionsTrue_ReturnsNumberForAllOtherParameters(string parameter, bool isSelected,
		int expected)
	{
		MatchingIntToBooleanConverter converter = new();

		int actual = (int)converter.ConvertBack(isSelected, typeof(int), parameter, null!);

		Assert.Equal(expected, actual);
	}

	/// <summary>
	/// Tests whether <see cref="FormatException"/> is thrown for non integer strings. This is possible since
	/// the developer is setting the ConverterParameter in XAML and might inadvertently set it to something
	/// that is not an integer.
	/// </summary>
	/// <param name="parameter">The ConverterParameter of the MenuItem.</param>
	[Fact]
	public void ConvertBack_NonInt_ThrowsFormatException()
	{
		MatchingIntToBooleanConverter converter = new();

		Assert.Throws<FormatException>(() =>
			(int)converter.ConvertBack(true, typeof(int), "a", null!));
	}
}