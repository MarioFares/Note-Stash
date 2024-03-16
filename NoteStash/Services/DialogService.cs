using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using NoteStash.Models;
using NoteStash.ViewModels;
using NoteStash.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace NoteStash.Services;

/// <summary>
/// Service for opening dialogs and launching windows.
/// </summary>
public class DialogService : IDialogService
{
	private readonly Settings _settings;

	public DialogService(Settings settings)
	{
		_settings = settings;
	}

    /// <inheritdoc />
    public string? ShowOpen(string filter = "Text documents|*.txt|All files|*.*", string defaultExt = "txt",
		string initDir = "", string title = "Open file")
	{
		OpenFileDialog dialog = new()
		{
			Filter = filter,
			InitialDirectory = initDir,
			Title = title,
			DefaultExt = defaultExt
		};
		bool? result = dialog.ShowDialog();
		if (result == true)
		{
			return string.IsNullOrEmpty(dialog.FileName) ? string.Empty : dialog.FileName;
		}

		return null;
	}

	/// <inheritdoc />
	public string? ShowSaveAs(string filter = "Text documents|*.txt|All files|*.*", string defaultExt = "txt",
		string initDir = "", string title = "Save file")
	{
		SaveFileDialog dialog = new()
		{
			Filter = filter,
			InitialDirectory = initDir,
			Title = title,
			DefaultExt = defaultExt
		};
		bool? result = dialog.ShowDialog();
		if (result == true)
		{
			return string.IsNullOrEmpty(dialog.FileName) ? string.Empty : dialog.FileName;
		}

		return null;
	}

	/// <inheritdoc />
	public bool? ShowSave(string fileName = "Untitled")
	{
		SaveDialogViewModel viewModel = App.Current.Services.GetRequiredService<SaveDialogViewModel>();
		viewModel.FileName = fileName;
		SaveDialogView dialog = new()
		{
			DataContext = viewModel,
			Owner = App.Current.MainWindow
		};
		dialog.ShowDialog();
		return viewModel.ToSave;
	}

	/// <inheritdoc />
	public void ShowSettings()
	{
		SettingsViewModel viewModel = App.Current.Services.GetRequiredService<SettingsViewModel>();
		SettingsView window = new()
		{
			DataContext = viewModel,
			Owner = App.Current.MainWindow
		};
		window.ShowDialog();
	}

	/// <inheritdoc />
	public void ShowNewWindow() =>	Process.Start("notestash.exe");
	

	/// <inheritdoc />
	public string? GetInput(string title, string prompt, bool isGetFileName = false, ObservableCollection<string>? options = null)
	{
		InputDialogViewModel viewModel = App.Current.Services.GetRequiredService<InputDialogViewModel>();
		viewModel.Title = title;
		viewModel.Prompt = prompt;
		viewModel.IsGetFileName = isGetFileName;
		viewModel.Options = options;
		InputDialogView dialog = new()
		{
			DataContext = viewModel,
			Owner = App.Current.MainWindow
		};

		dialog.ShowDialog();
		return viewModel.Result;
	}

	/// <inheritdoc />
	public void GotoDir(string dir)
	{
		if (Directory.Exists(dir)) Process.Start("explorer.exe", dir);
	}

	/// <inheritdoc />
	public void ShowInDir(string filePath)
	{
		if (!File.Exists(filePath)) return;

		// Combine the arguments together
		string argument = "/select, \"" + filePath + "\"";
		Process.Start("explorer.exe", argument);
	}

	/// <inheritdoc />
	public void SearchInWeb(string text)
	{
		if (string.IsNullOrEmpty(text)) return;

		// Trim and take 2000 character to avoid Invalid Uri exception
		text = text.Trim();
		text = text.Length <= 2000 ? text : text[..2000];
		string searchQuery = @"https://www.google.com/search?q=" + text;
		Process.Start(new ProcessStartInfo("explorer.exe", "\"" + searchQuery + "\""));
	}

	/// <inheritdoc />
	public void SetTheme(string themeName)
	{
		string themePath = _settings.ThemesPath + themeName + ".xaml";
		if (File.Exists(themePath))
		{
			App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
			{
				Source = new Uri(themePath, UriKind.RelativeOrAbsolute)
			});
		}
		else
		{
			string dictSource = "pack://application:,,,/NoteStash;component/Data/Themes/Default.xaml";
			ResourceDictionary mergedDictionary = App.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.ToString() == dictSource)!;
			XmlWriterSettings writerSettings = new()
			{
				Indent = true,
				CloseOutput = true,
				ConformanceLevel = ConformanceLevel.Fragment
			};
			XmlWriter writer = XmlWriter.Create(_settings.ThemesPath + "Default.xaml", writerSettings);
			XamlWriter.Save(mergedDictionary, writer);
			writer.Close();
		}

		App.Current.MainWindow.UpdateDefaultStyle();
		App.Current.MainWindow.UpdateLayout();
	}
}