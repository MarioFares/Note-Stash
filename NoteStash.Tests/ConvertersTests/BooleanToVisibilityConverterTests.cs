using NoteStash.Converters;
using System.Windows;
using Xunit;

namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="BooleanToVisibilityConverter"/>.
/// </summary>
public class BooleanToVisibilityConverterTests
{
	[Fact]
	public void Convert_True_ReturnsVisible()
	{
		Visibility expected = Visibility.Visible;
		BooleanToVisibilityConverter converter = new();

		Visibility actual = (Visibility)converter.Convert(true, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Convert_False_ReturnsCollapsed()
	{
		Visibility expected = Visibility.Collapsed;
		BooleanToVisibilityConverter converter = new();

		Visibility actual = (Visibility)converter.Convert(false, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ConvertBack_Visible_ReturnsTrue()
	{
		BooleanToVisibilityConverter converter = new();

		bool actual = (bool)converter.ConvertBack(Visibility.Visible, null!, null!, null!);

		Assert.True(actual);
	}

	[Fact]
	public void ConvertBack_Collapsed_ReturnsFalse()
	{
		BooleanToVisibilityConverter converter = new();

		bool actual = (bool)converter.ConvertBack(Visibility.Collapsed, null!, null!, null!);

		Assert.False(actual);
	}
}