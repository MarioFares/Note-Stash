using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NoteStash.Models;

/// <summary>
/// Model for a unicode manager that reads unicode groups from Json.
/// </summary>
public class UnicodeManager
{
	/// <summary>
	/// Returns a new unicode manager whose source of groups is passed as an argument.
	/// </summary>
	/// <param name="relativeSourcePath">Source of the Json file to supply the groups.</param>
	public UnicodeManager(string relativeSourcePath)
	{
		SourcePath = relativeSourcePath;
		RefreshGroups();
	}

	private string _sourcePath;

	/// <summary>
	/// Sets the relative or full path for the source file of the unicode groups and gets the full path.
	/// </summary>
	/// <remarks>Full path is always returned.</remarks>
	public string SourcePath
	{
		get => Path.GetFullPath(_sourcePath);
		set => _sourcePath = value;
	}

	/// <summary>
	/// List of available unicode groups.
	/// </summary>
	private List<UnicodeGroup> UnicodeGroups { get; set; }

	/// <summary>
	/// Gets the list of names of unicode groups.
	/// </summary>
	public List<string> Names => UnicodeGroups.Select(group => group.Name).ToList();

	/// <summary>
	/// Updates <see cref="UnicodeGroups"/> by deserializing the json in <see cref="SourcePath"/>.
	/// </summary>
	private void RefreshGroups()
	{
		string json = File.Exists(SourcePath) ? File.ReadAllText(SourcePath) : "[]";
		UnicodeGroups = JsonConvert.DeserializeObject<List<UnicodeGroup>>(json) ?? new List<UnicodeGroup>();
	}

	/// <summary>
	/// Refreshes <see cref="UnicodeGroups"/> asynchronously by deserializing the json in <see cref="SourcePath"/>.
	/// </summary>
	public async Task RefreshGroupsAsync() => await Task.Run(RefreshGroups);

	/// <summary>
	/// Returns all unicode characters in the same group whose name matches the argument.
	/// </summary>
	/// <param name="name">Name of the group to search.</param>
	public List<string> GetUnicodeCharsBasedOnName(string name) =>
		UnicodeGroups.FirstOrDefault(group => group.Name == name)?.Chars.ToList()!;
}
