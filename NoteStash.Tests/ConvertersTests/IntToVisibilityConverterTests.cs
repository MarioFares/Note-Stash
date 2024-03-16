using NoteStash.Converters;
using System.Windows;
using Xunit;

namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="IntToVisibilityConverter"/>.
/// </summary>
public class IntToVisibilityConverterTests
{
	[Fact]
	public void Convert_Zero_ReturnsCollapsed()
	{
		IntToVisibilityConverter converter = new();

		var actual = converter.Convert(0, null!, null!, null!);

		Assert.Equal(Visibility.Collapsed, actual);
	}

	/// <summary>
	/// Tests most commonly used integers in the application along with one negative edge case. It would be unwise
	/// to test every integer, but the below are the most important ints for the converter to get right.
	/// </summary>
	/// <param name="input"></param>
	[Theory]
	[InlineData(-1)]
	[InlineData(1)]
	public void Convert_NonzeroInts_ReturnsVisible(int input)
	{
		IntToVisibilityConverter converter = new();

		var actual = converter.Convert(input, null!, null!, null!);

		Assert.Equal(Visibility.Visible, actual);
	}
}