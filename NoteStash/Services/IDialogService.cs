using Microsoft.Win32;
using NoteStash.Views;
using System.Collections.ObjectModel;

namespace NoteStash.Services;

/// <summary>
/// Interface for opening dialogs, windows, and other applications.
/// </summary>
public interface IDialogService
{
	/// <summary>
	/// Shows a <see cref="OpenFileDialog"/> and returns the file path of the selected file, an empty string if 
	/// the dialog was opened but no selection was made, or null if the dialog was not successfully opened.
	/// </summary>
	/// <param name="filter">Determines what types of files are displayed.</param>
	/// <param name="defaultExt">The default extension.</param>
	/// <param name="initDir">Determines the initial directory when the dialog is opened.</param>
	/// <param name="title">Determines the title of the dialog.</param>
	/// <returns>File path of the selected file, an empty string if the dialog was opened
	/// but no selection was made, or null if the dialog was not successfully opened.</returns>
	public string? ShowOpen(string filter = "Text documents|*.txt|All files|*.*", string defaultExt = "txt",
		string initDir = "", string title = "Open file");

	/// <summary>
	/// Shows a <see cref="SaveFileDialog"/> and returns the file path of the selected file, an empty string if the
	/// dialog was opened but no selection was made, or null if the dialog was not successfully opened.
	/// </summary>
	/// <param name="filter">Determines what types of files are displayed.</param>
	/// <param name="defaultExt">The default extension.</param>
	/// <param name="initDir">Determines the initial directory when the dialog is opened.</param>
	/// <param name="title">Determines the title of the dialog.</param>
	/// <returns>File path of the selected file, an empty string if the dialog was opened
	/// but no selection was made, or null if the dialog was not successfully opened.</returns>
	public string? ShowSaveAs(string filter = "Text documents|*.txt|All files|*.*", string defaultExt = "txt",
		string initDir = "", string title = "Save file");

	/// <summary>
	/// Shows the <see cref="InputDialogView"/> and returns a boolean indicating whether to save or discard
	/// the file.
	/// </summary>
	/// <param name="fileName">Name of the file that you wish to be saved.</param>
	/// <returns>Boolean indicating whether to save or discard the file.</returns>
	public bool? ShowSave(string fileName = "Untitled");

	/// <summary>
	/// Launches the settings window and blocks thread until that window is closed.
	/// </summary>
	public void ShowSettings();

	/// <summary>
	/// Launches a new instance of the app. Does not return anything.
	/// </summary>
	/// <remarks>Launches another instance of the application.</remarks>
	public void ShowNewWindow();

	/// <summary>
	/// Shows the <see cref="InputDialogView"/>. Returns the input of the user.
	/// </summary>
	/// <param name="title">A title for the dialog.</param>
	/// <param name="prompt">A prompt for the user.</param>
	/// <param name="isGetFileName">Whether the input is for a file name. Default value is false.</param>
	/// <param name="options">The options to select from. If not null, a combox is shown instead of a textbox.</param>
	/// <returns>A string containing the user's input.</returns>
	public string? GetInput(string title, string prompt, bool isGetFileName = false, ObservableCollection<string>? options = null);

	/// <summary>
	/// Opens an explorer window to its argument if it exists. Otherwise, does not open the explorer.
	/// </summary>
	/// <param name="dir">Full path to directory.</param>
	public void GotoDir(string dir);

	/// <summary>
	/// Opens an explorer window to its argument's directory if it exists and selects the argument's file.
	/// Otherwise does not open the explorer.
	/// </summary>
	/// <param name="filePath">Full path to file.</param>
	public void ShowInDir(string filePath);

	/// <summary>
	/// Opens the default explorer and searchs its argument using Google.
	/// </summary>
	/// <param name="text">Text to search for in web.</param>
	public void SearchInWeb(string text);

	/// <summary>
	/// Set the current theme.
	/// </summary>
	/// <param name="view">A window for which to set the theme.</param>
	/// <param name="themeName">Name of the theme.</param>
	public void SetTheme(string themeName);
}