using Microsoft.Extensions.DependencyInjection;
using NoteStash.Models;
using NoteStash.Services;
using NoteStash.ViewModels;
using NoteStash.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using FontFamily = System.Windows.Media.FontFamily;

namespace NoteStash;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public sealed partial class App
{
	private readonly Settings _settings;
	private readonly MainViewModel _mainViewModel;

	public App()
	{
		InitializeComponent();
		Services = ConfigureServices();
		_settings = Services.GetRequiredService<Settings>();
		_mainViewModel = Services.GetRequiredService<MainViewModel>();
	}

	/// <summary>
	/// Gets the current <see cref="App"/> instance in use.
	/// </summary>
	public new static App Current => (App)Application.Current;

	/// <summary>
	/// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
	/// </summary>
	public IServiceProvider Services { get; }

	/// <summary>
	/// Configures the services for the application.
	/// </summary>
	private static IServiceProvider ConfigureServices()
	{
		ServiceCollection services = new();

		services.AddSingleton<MainView>();
		services.AddSingleton<MainViewModel>();
		services.AddSingleton<ActionPaletteViewModel>();
		services.AddTransient<SaveDialogViewModel>();
		services.AddTransient<InputDialogViewModel>();
		services.AddTransient<SettingsViewModel>();
		services.AddSingleton<IDialogService, DialogService>();
		services.AddSingleton<ITextEditorService, TextEditorService>(provider =>
		{
			MainView mainView = provider.GetRequiredService<MainView>();
			return new TextEditorService(mainView.InputBox);
		});
		services.AddSingleton<Settings>();

		return services.BuildServiceProvider();
	}

	/// <summary>
	/// Starts up the application and shows the main window. Sets up the models, view models, and services required
	/// by the application and injects them into the main view model. Also loads and sets up the saved settings. 
	/// </summary>
	/// <param name="e">The startup event args.</param>
	protected override async void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		MainView mainView = Services.GetRequiredService<MainView>();
		Current.MainWindow = mainView;

		// Set the current directory to executable directory otherwise might be system32
		Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

		// Models
		TextDocument document = new();
		Stash stash = new(_settings.StashPath);
		Stash templatesStash = new(_settings.TemplatesPath);
		List<string> recentFiles = _settings.RecentFiles;
		UnicodeManager unicodeManager = new(_settings.UnicodeFilePath);

		// Setup theme and show window
		IDialogService dialogService = Services.GetRequiredService<IDialogService>();
		dialogService.SetTheme(_settings.Theme);

		// Supply and setup the file argument if it exists
		if (e.Args.Length >= 1 && File.Exists(e.Args[0]))
		{
			document.FilePath = e.Args[0];
			await document.LoadAsync();
		}
		else
		{
			bool newFile = _settings.StartupNewFile;
			if (!newFile && recentFiles.Count > 0)
			{
				string mostRecentFile = recentFiles[0];
				if (File.Exists(mostRecentFile)) document.FilePath = mostRecentFile;
				recentFiles.Remove(mostRecentFile);
				await document.LoadAsync();
			}
		}

		_mainViewModel.Document = document;
		_mainViewModel.Stash = stash;
		_mainViewModel.TemplatesStash = templatesStash;
		_mainViewModel.RecentFilesManager = new(_settings.MaxRecentFiles, recentFiles);
		_mainViewModel.UnicodeManager = unicodeManager;
		_mainViewModel.InputText = document.Text;
		_mainViewModel.FontSize = _settings.FontSize;
		_mainViewModel.FontFamily = new FontFamily(_settings.FontFamily);
		_mainViewModel.DefaultFontSize = _settings.FontSize;
		_mainViewModel.AutosaveTimerInterval = _settings.Autosave;
		_mainViewModel.TextWrap = _settings.TextWrap;
		_mainViewModel.StateBar = _settings.StateBar;
		_mainViewModel.UnicodeBar = _settings.UnicodeBar;
		_mainViewModel.StatusBar = _settings.StatusBar;
		_mainViewModel.TabSize = _settings.TabSize;
		_mainViewModel.ConvertTabToSpaces = _settings.ConvertTabToSpaces;
		_mainViewModel.PromptToSave = _settings.PromptToSave;
		_mainViewModel.Diff = _settings.Diff;
		_mainViewModel.LineNumbers = _settings.LineNumbers;
		_mainViewModel.LastUnicodeChar = _settings.LastUnicodeChar;

		mainView.DataContext = _mainViewModel;
		mainView.Show();
	}

	/// <summary>
	/// Saves settings and exits the application.
	/// </summary>
	/// <param name="e">The exit event args.</param>
	protected override void OnExit(ExitEventArgs e)
	{
		_settings.FontSize = _mainViewModel.DefaultFontSize;
		_settings.FontFamily = _mainViewModel.FontFamily.ToString();
		_settings.Autosave = _mainViewModel.AutosaveTimerInterval;
		_settings.TextWrap = _mainViewModel.TextWrap;
		_settings.StateBar = _mainViewModel.StateBar;
		_settings.UnicodeBar = _mainViewModel.UnicodeBar;
		_settings.StatusBar = _mainViewModel.StatusBar;
		_settings.TabSize = _mainViewModel.TabSize;
		_settings.ConvertTabToSpaces = _mainViewModel.ConvertTabToSpaces;
		_settings.PromptToSave = _mainViewModel.PromptToSave;
		_settings.Diff = _mainViewModel.Diff;
		_settings.LineNumbers = _mainViewModel.LineNumbers;
		_settings.RecentFiles = _mainViewModel.RecentFiles.ToList();
		_settings.LastUnicodeChar = _mainViewModel.LastUnicodeChar;
		_settings.Save();
		base.OnExit(e);
	}
}