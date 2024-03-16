using Newtonsoft.Json;
using NoteStash.Models;
using System.Collections.Generic;
using Xunit;

namespace NoteStash.Tests.ModelsTests;

/// <summary>
/// Tests for <see cref="UnicodeGroup"/>.
/// </summary>
public class UnicodeGroupTests
{
	[Fact]
	public void ValidJson_IsDeserializedProperly()
	{
		string json = "{\"Name\": \"Test\", \"Chars\": [ \"m\"]}";

		UnicodeGroup actual = JsonConvert.DeserializeObject<UnicodeGroup>(json)!;

		Assert.Equal("Test", actual.Name);
		Assert.Equal(new List<string> { "m" }, actual.Chars);
	}

	[Fact]
	public void ValidObject_IsSerializedProperly()
	{
		UnicodeGroup input = new()
		{
			Name = "Test",
			Chars = new() { "m" }
		};
		string expected = "{\"Name\":\"Test\",\"Chars\":[\"m\"]}";

		string actual = JsonConvert.SerializeObject(input, Formatting.None);

		Assert.Equal(expected, actual);
	}
}
