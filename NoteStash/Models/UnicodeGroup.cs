using System.Collections.Generic;

namespace NoteStash.Models;

/// <summary>
/// A model for a unicode "group" to be serialized and deserialized into Json.
/// </summary>
public class UnicodeGroup
{
	public string Name { get; set; }

	public List<string> Chars { get; set; }
}
