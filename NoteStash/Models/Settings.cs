using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace NoteStash.Models;

/// <summary>
/// Singleton for configuring application settings saved as a json file.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class Settings
{
	public Settings()
	{
		Directory.CreateDirectory(ThemesPath);
		if (File.Exists(SettingsFilePath))
		{
			string jsonString = File.ReadAllText(Path.GetFullPath(SettingsFilePath));
			JsonConvert.PopulateObject(jsonString, this);
		}
	}

	~Settings() => Save();

	// Paths
	public string SettingsFilePath { get; set; } = "./Data/settings.json";

	public string UnicodeFilePath { get; set; } = "./Data/unicode.json";

	public string StashPath { get; set; } = "./Data/Stash/";

	public string TemplatesPath { get; set; } = "./Data/Templates/";

	public string ThemesPath { get; set; } = "./Data/Themes/";

	// Serialized Settings
	[JsonProperty]
	public int FontSize { get; set; } = 13;

	[JsonProperty]
	public string FontFamily { get; set; } = "Segoe UI";

	[JsonProperty]
	public int TabSize { get; set; } = 4;

	[JsonProperty]
	public int Autosave { get; set; }

	[JsonProperty]
	public bool PromptToSave { get; set; } = true;

	[JsonProperty]
	public bool StateBar { get; set; } = true;

	[JsonProperty]
	public bool UnicodeBar { get; set; } = true;

	[JsonProperty]
	public bool LineNumbers { get; set; }

	[JsonProperty]
	public bool Diff { get; set; }

	[JsonProperty]
	public bool StatusBar { get; set; } = true;

	[JsonProperty]
	public bool TextWrap { get; set; } = true;

	[JsonProperty]
	public List<string> RecentFiles { get; set; } = new();

	[JsonProperty]
	public string Theme { get; set; } = "Default";

	[JsonProperty]
	public bool StartupNewFile { get; set; } = true;

	[JsonProperty]
	public int MaxRecentFiles { get; set; } = 9;

	[JsonProperty]
	public bool ExitMinimizeToTray { get; set; }

	[JsonProperty]
	public bool ConvertTabToSpaces { get; set; }

	[JsonProperty]
	public string LastUnicodeChar { get; set; } = string.Empty;

	public void Save()
	{
		string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
		File.WriteAllText(Path.GetFullPath(SettingsFilePath), jsonString);
	}
}
