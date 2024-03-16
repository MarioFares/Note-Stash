using NoteStash.Converters;
using System;
using System.Globalization;
using Xunit;

namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="DateTimeToStringConverter"/>. There is no need to test ConvertBack since it is not used
/// and therefore not implemented.
/// </summary>
public class DateTimeToStringConverterTests
{
	[Fact]
	public void Convert_Default_ReturnsEmptyString()
	{
		DateTimeToStringConverter converter = new();

		string actual = (string)converter.Convert(default(DateTime), null!, null!, null!);

		Assert.Equal(string.Empty, actual);
	}

	[Fact]
	public void Convert_ValidDateTime_ReturnsCorrectString()
	{
		DateTime input = new(2000, 3, 3, 3, 3, 3);
		string expected = input.ToString(CultureInfo.CurrentCulture);
		DateTimeToStringConverter converter = new();

		string actual = (string)converter.Convert(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}
}