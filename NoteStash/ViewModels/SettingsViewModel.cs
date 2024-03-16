using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteStash.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FontFamily = System.Windows.Media.FontFamily;
using Settings = NoteStash.Models.Settings;

namespace NoteStash.ViewModels;

/// <summary>
/// View model for the settings page.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
	private readonly IDialogService _dialogService;
	private readonly Settings _settings;

	public SettingsViewModel(IDialogService dialogService, Settings settings)
	{
		_dialogService = dialogService;
		_settings = settings;
		SelectedFontFamily = new FontFamily(_settings.FontFamily);
		SelectedFontSize = _settings.FontSize;
		StartupNewFile = _settings.StartupNewFile;
		ExitMinimizeToTray = _settings.ExitMinimizeToTray;
		FontSizes = new ObservableCollection<int>(new List<int>() { 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 20, 22, 24, 36, 48, 72 });
		SelectedMaxRecentFiles = _settings.MaxRecentFiles;
		MaxRecentFiles = new ObservableCollection<int>(Enumerable.Range(1, 30).ToList());

		string[] themeFiles = Directory.GetFiles(_settings.ThemesPath);
		Themes = new ObservableCollection<string>(themeFiles.Select(e => Path.GetFileNameWithoutExtension(e)));
		SelectedTheme = _settings.Theme;
	}

	/// <summary>
	/// List of available themes in the themes directory.
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<string> _themes;

	private string _selectedTheme;

	/// <summary>
	/// Currently selected theme.
	/// </summary>
	public string SelectedTheme
	{
		get => _selectedTheme;
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				_dialogService.SetTheme(value);
				SetProperty(ref _selectedTheme, value);
			}
		}
	}

	/// <summary>
	/// Currently selected font family.
	/// </summary>
	[ObservableProperty]
	private FontFamily _selectedFontFamily;

	/// <summary>
	/// Currently selected font size
	/// </summary>
	[ObservableProperty]
	private int _selectedFontSize;

	/// <summary>
	/// List of font sizes.
	/// </summary>
	public ObservableCollection<int> FontSizes { get; }

	/// <summary>
	/// Gets or sets whether to start application with a new file or most recent file.
	/// </summary>
	public bool StartupNewFile { get; set; }

	/// <summary>
	/// Gets or sets whether to minimize application when closed or shut it down.
	/// </summary>
	public bool ExitMinimizeToTray { get; set; }

	/// <summary>
	/// Gets or sets the collection of allowed recent files counts.
	/// </summary>
	public ObservableCollection<int> MaxRecentFiles { get; }

	/// <summary>
	/// Gets or sets the maximum of recent files.
	/// </summary>
	public int SelectedMaxRecentFiles { get; set; }

	public void SaveSettings()
	{
		_settings.FontFamily = SelectedFontFamily.ToString();
		_settings.FontSize = SelectedFontSize;
		_settings.StartupNewFile = StartupNewFile;
		_settings.ExitMinimizeToTray = ExitMinimizeToTray;
		_settings.Theme = SelectedTheme;
		_settings.MaxRecentFiles = SelectedMaxRecentFiles;
		_settings.Save();
	}

	[RelayCommand]
	private void RefreshThemes()
	{
		string[] themeFiles = Directory.GetFiles(_settings.ThemesPath);
		Themes = new ObservableCollection<string>(themeFiles.Select(e => Path.GetFileNameWithoutExtension(e)));
	}

	[RelayCommand]
	private void OpenThemesFolder() => _dialogService.GotoDir(Path.GetFullPath(_settings.ThemesPath));

	[RelayCommand]
	private void ClearRecentFiles() => _settings.RecentFiles.Clear();
}