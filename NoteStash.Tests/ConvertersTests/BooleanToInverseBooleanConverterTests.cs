using NoteStash.Converters;
using Xunit;

namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="BooleanToInverseBooleanConverter"/>.
/// </summary>
public class BooleanToInverseBooleanConverterTests
{
	[Theory]
	[InlineData(true, false)]
	[InlineData(false, true)]
	public void Convert_TrueOrFalse_ReturnsInverse(bool input, bool expected)
	{
		BooleanToInverseBooleanConverter converter = new();

		bool actual = (bool)converter.Convert(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(true, false)]
	[InlineData(false, true)]
	public void ConvertBack_TrueOrFalse_ReturnsInverse(bool input, bool expected)
	{
		BooleanToInverseBooleanConverter converter = new();

		bool actual = (bool)converter.ConvertBack(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}
}
