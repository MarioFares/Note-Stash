using NoteStash.Converters;
using Xunit;


namespace NoteStash.Tests.ConvertersTests;

/// <summary>
/// Tests for <see cref="LongToSizeStringConverter"/>. There is no need to test ConvertBack since it is not used
/// and therefore not implemented.
/// </summary>
public class LongToSizeStringConverterTests
{
	[Fact]
	public void Convert_NegativeInt_ReturnsZeroBytes()
	{
		LongToSizeStringConverter converter = new();

		var actual = converter.Convert((long)-1, null!, null!, null!);

		Assert.Equal("0 B", actual);
	}

	[Theory]
	[InlineData(0, "0 B")]
	[InlineData(1023, "1023 B")]
	public void Convert_InBytesRange_ReturnsBytes(long input, string expected)
	{
		LongToSizeStringConverter converter = new();

		var actual = converter.Convert(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(1024, "1 KB")]
	[InlineData(1025, "1 KB")]
	[InlineData(2048, "2 KB")]
	[InlineData(1024 * 1024 - 1, "1023 KB")]
	public void Convert_InKiloBytesRange_ReturnsKiloBytes(long input, string expected)
	{
		LongToSizeStringConverter converter = new();

		var actual = converter.Convert(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(1024 * 1024, "1 MB")]
	[InlineData(1024 * 1024 + 1, "1 MB")]
	[InlineData(2 * 1024 * 1024, "2 MB")]
	[InlineData(1024 * 1024 * 1024 - 1, "1023 MB")]
	public void Convert_InMegaBytesRange_ReturnsMegaBytes(long input, string expected)
	{
		LongToSizeStringConverter converter = new();

		var actual = converter.Convert(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(1024 * 1024 * 1024, "1 GB")]
	[InlineData(1024 * 1024 * 1024 + 1, "1 GB")]
	public void Convert_InGigaBytesRange_ReturnsGigaBytes(long input, string expected)
	{
		LongToSizeStringConverter converter = new();

		var actual = converter.Convert(input, null!, null!, null!);

		Assert.Equal(expected, actual);
	}
}