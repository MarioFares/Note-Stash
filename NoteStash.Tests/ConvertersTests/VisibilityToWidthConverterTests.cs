using NoteStash.Converters;
using System.Windows;
using Xunit;

namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="VisibilityToWidthConverter"/>. ConvertBack not tested as it should not be needed.
/// </summary>
public class VisibilityToWidthConverterTests
{
	[Fact]
	public void Convert_Visibile_ReturnsStar()
	{
		GridLength expected = new(1, GridUnitType.Star);
		VisibilityToWidthConverter converter = new();

		GridLength actual = (GridLength)converter.Convert(Visibility.Visible, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Convert_Collapsed_ReturnsAuto()
	{
		GridLength expected = GridLength.Auto;
		VisibilityToWidthConverter converter = new();

		GridLength actual = (GridLength)converter.Convert(Visibility.Collapsed, null!, null!, null!);

		Assert.Equal(expected, actual);
	}
}