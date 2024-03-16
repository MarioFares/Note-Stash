using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NoteStash.Models;
using NoteStash.Services;
using NoteStash.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using Clipboard = System.Windows.Forms.Clipboard;
using FontFamily = System.Windows.Media.FontFamily;

namespace NoteStash.ViewModels;

/// <summary>
/// View model for the main page.
/// </summary>
public partial class MainViewModel : ObservableObject
{
	private readonly IDialogService _dialogService;
	private readonly ITextEditorService _textEditorService;
	private readonly Settings _settings;
	private readonly DispatcherTimer _autosaveTimer;
	private readonly List<int> _tabs;
	private readonly List<int> _autosaveIntervals;

	/// <summary>
	/// Initialize the view model by initializing the models and relevant services. Also sets up the font and
	/// autosave settings.
	/// </summary>
	/// <param name="dialogService">The Dialog Service that will enable opening windows and dialogs.</param>
	/// <param name="textEditorService">The Text Editor Service that will enable interacting with the editor.</param>
	/// <param name="settings">The settings manager for the application.</param>
	public MainViewModel(IDialogService dialogService, ITextEditorService textEditorService, Settings settings)
	{
		_dialogService = dialogService;
		_textEditorService = textEditorService;
		_settings = settings;
		_autosaveTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(AutosaveTimerInterval),
		};
		_autosaveTimer.Tick += TimerTickAsync;

		// For setter checking
		_tabs = new List<int>();
		_tabs.AddRange(new[] { 2, 4, 6, 8, 10 });
		_autosaveIntervals = new List<int>();
		_autosaveIntervals.AddRange(new[] { 500, 1000, 2000, 3000, 4000, 0 });
	}

	#region Models

	/// <summary>
	/// Gets or sets the current <see cref="TextDocument"/> in the application.
	/// </summary>
	public TextDocument Document { get; set; }

	/// <summary>
	/// Gets or sets an instance of a <see cref="Stash"/> for stashed files.
	/// </summary>
	public Stash Stash { get; set; }

	/// <summary>
	/// Gets or sets an instance of a <see cref="Stash"/> for template files.
	/// </summary>
	public Stash TemplatesStash { get; set; }

	/// <summary>
	/// Gets or sets the recent files manager for the application.
	/// </summary>
	public RecentFilesManager RecentFilesManager { get; set; }

	/// <summary>
	/// Gets or sets the unicode manager for the application.
	/// </summary>
	public UnicodeManager UnicodeManager { get; set; }

	#endregion

	#region Common Functionality: Load Prompt NonEmpty InsertRecent OpenDir

	/// <summary>
	/// Loads the file specified by the argument. Changes the current document's file path and loads the
	/// text into <see cref="InputText"/> while notifying <see cref="FileNameDisplayText"/> and
	/// <see cref="SavedText"/> of change.
	/// </summary>
	/// <param name="filePath">The path to the file to be loaded.</param>
	[RelayCommand]
	public async Task LoadFileAsync(string filePath)
	{
		if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

		// Inserts the current filePath before changing to the new one on the next line
		RecentFilesManager.Insert(Document.FilePath, filePath);
		Document.FilePath = filePath;
		await Document.LoadAsync();
		InputText = Document.Text;
		OnPropertyChanged(nameof(FileNameDisplayText));
		OnPropertyChanged(nameof(SavedText));
		OnPropertyChanged(nameof(RecentFiles));
		UpdateDocumentIsNonEmpty_CanExecuteChanged();
	}

	/// <summary>
	/// Prompts the user to save their document if and only if the document has been modified and the user has
	/// enabled the <see cref="PromptToSave"/> ability. If the user prompts yes then <see cref="SaveFileAsync"/>
	/// is executed.
	/// </summary>
	/// <returns>
	/// True if the file is to be saved, false if not, and null if no changes to the document should be made.
	/// </returns>
	public async Task<bool?> PromptSaveAsync()
	{
		if (!Document.IsModified || !PromptToSave) return false;

		bool? toSave = _dialogService.ShowSave(FileNameDisplayText);
		switch (toSave)
		{
			case true:
				await SaveFileAsync();
				return true;
			case false:
				return false;
			default:
				return null;
		}
	}

	/// <summary>
	/// Returns whether the document is not empty, i.e. there is a file that is loaded.
	/// Primarily used as the CanExecute function for several commands.
	/// </summary>
	private bool IsDocumentNonEmpty() => !Document.IsEmpty;

	private void UpdateDocumentIsNonEmpty_CanExecuteChanged()
	{
		OpenDocumentDirectoryCommand.NotifyCanExecuteChanged();
		CopyFileNameCommand.NotifyCanExecuteChanged();
		CopyFileNameWithoutExtensionCommand.NotifyCanExecuteChanged();
		CopyFilePathCommand.NotifyCanExecuteChanged();
		CopyDirectoryPathCommand.NotifyCanExecuteChanged();
	}

	private bool IsRecentFilesNonEmpty() => RecentFilesManager.RecentFiles.Count > 0;

	// Recent files functionality
	public void InsertRecentFile(string oldFilePath, string newFilePath) => RecentFilesManager.Insert(oldFilePath, newFilePath);

	[RelayCommand]
	private void OpenDirectory(string dir) => _dialogService.GotoDir(Path.GetFullPath(dir));

	#endregion

	#region File Menu

	[RelayCommand]
	[property: ExecutableAction("New File", "File > New File", "Ctrl+N")]
	private async Task NewFileAsync()
	{
		if (await PromptSaveAsync() is null) return;

		Document.New();
		InputText = Document.Text;
		OnPropertyChanged(nameof(FileNameDisplayText));
		OnPropertyChanged(nameof(SavedText));
		UpdateDocumentIsNonEmpty_CanExecuteChanged();
	}

	[RelayCommand]
	[property: ExecutableAction("New Window", "File > New Window", "Ctrl+Shift+N")]
	private void ShowNewWindow() => _dialogService.ShowNewWindow();

	[RelayCommand]
	[property: ExecutableAction("Open File", "File > Open File", "Ctrl+O")]
	private async Task OpenFileAsync()
	{
		string? filePath = _dialogService.ShowOpen();
		if (!string.IsNullOrEmpty(filePath)) await LoadFileAsync(filePath);
	}

	[RelayCommand(CanExecute = nameof(IsDocumentNonEmpty))]
	[property: ExecutableAction("Show in Folder", "File > Show in Folder")]
	private void OpenDocumentDirectory()
	{
		if (!Document.IsEmpty) _dialogService.ShowInDir(Document.FilePath);
	}

	#region Recent Files SubMenu

	/// <summary>
	/// Collection of recent files opened by the application.
	/// </summary>
	public ObservableCollection<string> RecentFiles => new(RecentFilesManager.RecentFiles);

	[RelayCommand(CanExecute = nameof(IsRecentFilesNonEmpty))]
	[property: ExecutableAction("Open Recent File", "File > Open Recent", "", nameof(RecentFiles))]
	private async Task OpenRecentFile(string filePath) => await LoadFileAsync(filePath);

	[RelayCommand(CanExecute = nameof(IsRecentFilesNonEmpty))]
	[property: ExecutableAction("Refresh Recent Files", "File > Recent Files > Refresh")]
	private void RefreshRecentFiles() => OnPropertyChanged(nameof(RecentFiles));

	[RelayCommand(CanExecute = nameof(IsRecentFilesNonEmpty))]
	[property: ExecutableAction("Clear Recent Files", "File > Recent Files > Clear")]
	private void ClearRecentFiles()
	{
		RecentFilesManager.Clear();
		OnPropertyChanged(nameof(RecentFiles));
	}

	#endregion

	[RelayCommand]
	[property: ExecutableAction("Save File", "File > Save", "Ctrl+S")]
	private async Task SaveFileAsync()
	{
		if (!Document.IsEmpty)
		{
			await Document.SaveAsync();
		}
		else
		{
			await SaveFileAsAsync();
		}

		OnPropertyChanged(nameof(DocumentModified));
		OnPropertyChanged(nameof(SavedText));
	}

	[RelayCommand]
	[property: ExecutableAction("Save File As", "File > Save As", "Ctrl+Shift+S")]
	private async Task SaveFileAsAsync(string initDir = "")
	{
		string? filePath = _dialogService.ShowSaveAs(initDir: initDir);
		if (string.IsNullOrEmpty(filePath)) return;

		Document.FilePath = filePath;
		await Document.SaveAsync();
		OnPropertyChanged(nameof(FileNameDisplayText));
		OnPropertyChanged(nameof(DocumentModified));
		UpdateDocumentIsNonEmpty_CanExecuteChanged();
	}

	[RelayCommand]
	[property: ExecutableAction("Revert Changes To Last Save", "File > Revert Changes > Last Save")]
	private void RevertChangesToLastSave() => InputText = SavedText;

	[RelayCommand]
	[property: ExecutableAction("Revert Changes To Initial Text", "File > Revert Changes > Initial Text")]
	private void RevertChangesToInitialText() => InputText = Document.InitialText;

	// This property and the method after it are strictly for use in the action palette
	public ObservableCollection<string> AutosaveIntervalStrings => new(_autosaveIntervals.OrderBy(i => i).Select(i => i / 1000.0).Select(i => i.ToString()).ToList());

	[RelayCommand]
	[property: ExecutableAction("Set Autosave", "File > Autosave", "", nameof(AutosaveIntervalStrings))]
	private void SetAutosave(string arg)
	{
		bool success = double.TryParse(arg, out double result);
		if (success) AutosaveTimerInterval = (int)(result * 1000);
	}

	private int _autosaveTimerInterval;

	/// <summary>
	/// Gets or sets the current interval in milliseconds for the autosave timer only if the value is in
	/// <see cref="_autosaveIntervals"/>.
	/// </summary>
	public int AutosaveTimerInterval
	{
		get => _autosaveTimerInterval;
		set
		{
			int time = _autosaveIntervals.Contains(value) ? value : 0;
			_autosaveTimer.Interval = TimeSpan.FromMilliseconds(time);
			SetProperty(ref _autosaveTimerInterval, time);
		}
	}

	/// <summary>
	/// Gets or sets whether the user should be prompted to save before changing documents.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Prompt to Save", "File > Prompt to Save")]
	private bool _promptToSave;

	#endregion

	#region Edit Menu

	[RelayCommand(CanExecute = nameof(IsDocumentNonEmpty))]
	[property: ExecutableAction("Copy File Name", "Edit > Copy to Clipboard > File Name")]
	private void CopyFileNameWithoutExtension() => Clipboard.SetText(Path.GetFileNameWithoutExtension(FileNameDisplayText));

	[RelayCommand(CanExecute = nameof(IsDocumentNonEmpty))]
	[property: ExecutableAction("Copy File Name w/ Extension", "Edit > Copy to Clipboard > File Name w/ Ext")]
	private void CopyFileName() => Clipboard.SetText(FileNameDisplayText);

	[RelayCommand(CanExecute = nameof(IsDocumentNonEmpty))]
	[property: ExecutableAction("Copy File Path", "Edit > Copy to Clipboard > File Path")]
	private void CopyFilePath() => Clipboard.SetText(DocumentPath);

	[RelayCommand(CanExecute = nameof(IsDocumentNonEmpty))]
	[property: ExecutableAction("Copy Folder Path", "Edit > Copy to Clipboard > Folder Path")]
	private void CopyDirectoryPath() => Clipboard.SetText(Path.GetDirectoryName(DocumentPath));

	[RelayCommand(CanExecute = nameof(InsertLastUnicodeCharacter_CanExecute))]
	[property: ExecutableAction("Insert Last Unicode Character", "Insert > Last Unicode Character", "Ctrl+U")]
	private void InsertLastUnicodeCharacter()
	{
		if (!string.IsNullOrEmpty(LastUnicodeChar)) _textEditorService.Insert(LastUnicodeChar);
	}
	private bool InsertLastUnicodeCharacter_CanExecute() => !string.IsNullOrEmpty(LastUnicodeChar);

	[RelayCommand]
	[property: ExecutableAction("Clear All Text", "Edit > Clear All", "Ctrl+Shift+Del")]
	private void ClearAllText() => InputText = string.Empty;

	#region Case SubMenu

	[RelayCommand]
	[property: ExecutableAction("Uppercase", "Edit > Case > UPPERCASE")]
	private void UpperCase() => _textEditorService.UpperCase();

	[RelayCommand]
	[property: ExecutableAction("Lowercase", "Edit > Case > lowercase")]
	private void LowerCase() => _textEditorService.LowerCase();

	[RelayCommand]
	[property: ExecutableAction("Title Case", "Edit > Case > Title Case")]
	private void TitleCase() => _textEditorService.TitleCase();

	[RelayCommand]
	[property: ExecutableAction("Sentence Case", "Edit > Case > Sentence case")]
	private void SentenceCase() => _textEditorService.SentenceCase();

	[RelayCommand]
	[property: ExecutableAction("Invert Case", "Edit > Case > iNVERT cASE")]
	private void InvertCase() => _textEditorService.InvertCase();

	[RelayCommand]
	[property: ExecutableAction("Random Case", "Edit > Case > rAndOm CasE")]
	private void RandomCase() => _textEditorService.RandomCase();

	#endregion

	#region Line SubMenu

	[RelayCommand]
	[property: ExecutableAction("Select Line", "Edit > Line > Select", "Ctrl+L")]
	private void SelectCurrentLine() => _textEditorService.SelectCurrentLine();

	[RelayCommand]
	[property: ExecutableAction("Duplicate Line", "Edit > Line > Duplicate", "Ctrl+D")]
	private void DuplicateCurrentLine() => _textEditorService.DuplicateCurrentLine();

	[RelayCommand]
	[property: ExecutableAction("Delete Line", "Edit > Line > Delete", "Ctrl+Shift+Backspace")]
	private void DeleteCurrentLine() => _textEditorService.DeleteCurrentLine();

	[RelayCommand]
	[property: ExecutableAction("Move Line Up", "Edit > Line > Move Up", "Alt+Up")]
	private void MoveLineUp() => _textEditorService.MoveLineUp();

	[RelayCommand]
	[property: ExecutableAction("Move Line Down", "Edit > Line > Move Down", "Alt+Down")]
	private void MoveLineDown() => _textEditorService.MoveLineDown();

	#endregion

	#region Format SubMenu

	[RelayCommand]
	[property: ExecutableAction("Trim", "Edit > Format > Trim")]
	private void Trim() => _textEditorService.Trim();

	[RelayCommand]
	[property: ExecutableAction("Trim Start", "Edit > Format > Trim Start")]
	private void TrimStart() => _textEditorService.TrimStart();

	[RelayCommand]
	[property: ExecutableAction("Trim End", "Edit > Format > Trim End")]
	private void TrimEnd() => _textEditorService.TrimEnd();

	[RelayCommand]
	[property: ExecutableAction("Trim Lines", "Edit > Format > Trim Lines")]
	private void TrimLines() => _textEditorService.TrimLines();

	[RelayCommand]
	[property: ExecutableAction("Trim Line Starts", "Edit > Format > Trim Line Starts")]
	private void TrimLinesStart() => _textEditorService.TrimLinesStart();

	[RelayCommand]
	[property: ExecutableAction("Trim Line Ends", "Edit > Format > Trim Line Ends")]
	private void TrimLinesEnd() => _textEditorService.TrimLinesEnd();

	[RelayCommand]
	[property: ExecutableAction("Convert Spaces to Tabs", "Edit > Format > Convert Spaces to Tabs")]
	private void ConvertSpacesToTabs() => _textEditorService.ConvertSpacesToTabs();

	[RelayCommand]
	[property: ExecutableAction("Convert Leading Spaces to Tabs", "Edit > Format > Convert Leading Spaces to Tabs")]
	private void ConvertLeadingSpacesToTabs() => _textEditorService.ConvertLeadingSpacesToTabs();

	[RelayCommand]
	[property: ExecutableAction("Convert Tabs to Spaces", "Edit > Format > Convert Tabs to Spaces")]
	private void ConvertTabsToSpaces() => _textEditorService.ConvertTabsToSpaces();

	[RelayCommand]
	[property: ExecutableAction("Convert Leading Tabs to Spaces", "Edit > Format > Convert Leading Tabs to Spaces")]
	private void ConvertLeadingTabsToSpaces() => _textEditorService.ConvertLeadingTabsToSpaces();

	[RelayCommand]
	[property: ExecutableAction("Normalize Vertical Whitespace", "Edit > Format > Normalize Vertical Whitespace")]
	private void NormalizeVerticalWhitespace() => _textEditorService.NormalizeVerticalWhitespace();

	[RelayCommand]
	[property: ExecutableAction("Join Lines", "Edit > Format > Join Lines")]
	private void JoinLines() => _textEditorService.JoinLines();

	[RelayCommand]
	[property: ExecutableAction("Normalize Horizontal Whitespace", "Edit > Format > Normalize Horizontal Whitespace")]
	private void NormalizeHorizontalWhitespace() => _textEditorService.NormalizeHorizontalWhitespace();

	[RelayCommand]
	[property: ExecutableAction("Split Words", "Edit > Format > Split Words")]
	private void SplitWords() => _textEditorService.SplitWords();

	#endregion

	#region Sort SubMenu

	[RelayCommand]
	[property: ExecutableAction("Sort Words: Ascending", "Edit > Sort > Words Asc")]
	private void SortWordsAsc() => _textEditorService.SortWordsAsc();

	[RelayCommand]
	[property: ExecutableAction("Sort Words: Descending", "Edit > Sort > Words Desc")]
	private void SortWordsDesc() => _textEditorService.SortWordsDesc();

	[RelayCommand]
	[property: ExecutableAction("Sort Lines: Ascending", "Edit > Sort > Lines Asc")]
	private void SortLinesAsc() => _textEditorService.SortLinesAsc();

	[RelayCommand]
	[property: ExecutableAction("Sort Lines: Descending", "Edit > Sort > Lines Desc")]
	private void SortLinesDesc() => _textEditorService.SortLinesDesc();

	#endregion

	#region Tabs SubMenu

	/// <summary>
	/// Gets or sets whether to convert the tab key to spaces
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Convert Tab To Spaces", "Edit > Tabs")]
	private bool _convertTabToSpaces;

	private int _tabSize;

	/// <summary>
	/// Gets or sets the current tab size of the editor only if it is in <see cref="_tabs"/>.
	/// </summary>
	public int TabSize
	{
		get => _tabSize;
		set => SetProperty(ref _tabSize, _tabs.Contains(value) ? value : 2);
	}

	#endregion

	#endregion

	#region View Menu

	#region Zoom SubMenu

	private int _defaultFontSize;

	/// <summary>
	/// Gets or sets the default font size for the main text editor. Used primarily for restoring zoom.
	/// </summary>
	public int DefaultFontSize
	{
		get => _defaultFontSize;
		set
		{
			if (value > 0) SetProperty(ref _defaultFontSize, value);
		}
	}

	[RelayCommand]
	[property: ExecutableAction("Zoom In", "View > Zoom > Zoom In", "Ctrl+=")]
	private void ZoomIn() => FontSize += 2;

	[RelayCommand]
	[property: ExecutableAction("Zoom Out", "View > Zoom > Zoom Out", "Ctrl+-")]
	private void ZoomOut() => FontSize -= 2;

	[RelayCommand]
	[property: ExecutableAction("Default Zoom", "View > Zoom > Default Zoom", "Ctrl+0")]
	private void RestoreDefaultZoom() => FontSize = DefaultFontSize;

	#endregion

	#region Editor SubMenu

	[ObservableProperty]
	[property: ExecutableAction("Show Spaces", "View > Editor > Show Spaces")]
	private bool _showSpaces;

	[ObservableProperty]
	[property: ExecutableAction("Show Tabs", "View > Editor > Show Tabs")]
	private bool _showTabs;

	[ObservableProperty]
	[property: ExecutableAction("Show End of Line", "View > Editor > Show End of Line")]
	private bool _showEndOfLine;

	#endregion

	/// <summary>
	/// Gets or sets whether the state bar is visible.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("State Bar", "View > State Bar")]
	private bool _stateBar;

	/// <summary>
	/// Gets or sets whether the unicode bar is visible.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Unicode Bar", "View > Unicode Bar")]
	private bool _unicodeBar;

	/// <summary>
	/// Gets or sets whether line numbers are enabled.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Line Numbers", "View > Line Numbers")]
	private bool _lineNumbers;

	/// <summary>
	/// Gets or sets whether the diff editor is visible. The diff editor displays the <see cref="SavedText"/>.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Diff", "View > Diff")]
	private bool _diff;

	/// <summary>
	/// Gets or sets whether the status bar is visible.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Status Bar", "View > Status Bar")]
	private bool _statusBar;

	/// <summary>
	/// Gets or sets whether text wrapping is enabled.
	/// </summary>
	[ObservableProperty]
	[property: ExecutableAction("Text Wrap", "View > Wrap")]
	private bool _textWrap;

	#endregion

	#region Insert Menu

	[RelayCommand]
	[property: ExecutableAction("Insert File As Text", "Insert > File As Text", "Ctrl+Shift+O")]
	private void InsertFileAsText()
	{
		string? filePath = _dialogService.ShowOpen();
		if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

		string fileText = File.ReadAllText(filePath);
		_textEditorService.Insert(fileText);
	}

	[RelayCommand]
	private void InsertDateTime(string arg)
	{
		string time = DateTime.Now.ToString(arg);
		_textEditorService.Insert(time);
	}

	[RelayCommand]
	[property: ExecutableAction("Insert Line Before", "Insert > Line Before", "Ctrl+Shift+Return")]
	private void InsertLineBefore() => _textEditorService.InsertLineBefore();

	[RelayCommand]
	[property: ExecutableAction("Insert Line After", "Insert > Line After", "Ctrl+Return")]
	private void InsertLineAfter() => _textEditorService.InsertLineAfter();

	[RelayCommand]
	[property: ExecutableAction("Insert Text At Line Starts", "Insert > Text At Line Starts")]
	private void InsertTextAtLineStarts()
	{
		string? insertion = _dialogService.GetInput("Insert", "Provide text to insert at line starts:", false);
		if (insertion == null) return;

		_textEditorService.InsertTextAtLineStarts(insertion);
	}

	[RelayCommand]
	[property: ExecutableAction("Insert Text At Line Ends", "Insert > Text At Line Ends")]
	private void InsertTextAtLineEnds()
	{
		string? insertion = _dialogService.GetInput("Insert", "Provide text to insert at line ends:", false);
		if (insertion == null) return;

		_textEditorService.InsertTextAtLineEnds(insertion);
	}

	#endregion

	#region Unicode Menu

	[RelayCommand]
	[property: ExecutableAction("Select Next Unicode Group", "Unicode > Next Group", "Ctrl+.")]
	private void SelectNextUnicodeGroup()
	{
		int currentIndex = AvailableUnicodeNames.IndexOf(SelectedUnicodeGroup);
		int nextIndex = (currentIndex + 1) % AvailableUnicodeNames.Count;
		SelectedUnicodeGroup = AvailableUnicodeNames[nextIndex];
	}

	[RelayCommand]
	[property: ExecutableAction("Select Previous Unicode Group", "Unicode > Previous Group", "Ctrl+,")]
	private void SelectPreviousUnicodeGroup()
	{
		int currentIndex = AvailableUnicodeNames.IndexOf(SelectedUnicodeGroup);
		if (currentIndex == -1)
		{
			SelectedUnicodeGroup = AvailableUnicodeNames[^1];
		}
		else
		{
			int previousIndex = (currentIndex - 1 + AvailableUnicodeNames.Count) % AvailableUnicodeNames.Count;
			SelectedUnicodeGroup = AvailableUnicodeNames[previousIndex];
		}
	}

	[RelayCommand]
	[property: ExecutableAction("Edit Unicode Source File", "Unicode > Edit Source")]
	private async Task EditUnicodeSource() => await LoadFileAsync(UnicodeManager.SourcePath);

	[RelayCommand]
	[property: ExecutableAction("Refresh Unicode Groups", "Unicode > Refresh")]
	private async Task RefreshUnicode()
	{
		await UnicodeManager.RefreshGroupsAsync();
		OnPropertyChanged(nameof(AvailableUnicodeNames));
		OnPropertyChanged(nameof(AvailableUnicodeChars));
	}

	#endregion

	#region Stash Menu

	/// <summary>
	/// List of stash files in <see cref="Stash.SourcePath"/>.
	/// </summary>
	public ObservableCollection<string> StashedFiles => new(Stash.StashedFiles);

	[RelayCommand]
	[property: ExecutableAction("Stash File", "Stash > Stash")]
	private async Task StashFileAsync()
	{
		string? fileName = _dialogService.GetInput("Stash", "Provide a name for your file:", true);
		if (fileName == null) return;

		await Stash.StashFileAsync(fileName, InputText);
		await LoadFileAsync(Stash.SourcePath + Stash.LastStashedFile);
		await RefreshStashAsync();
	}

	[RelayCommand]
	[property: ExecutableAction("Open Stashed File", "Stash > Open", "", nameof(StashedFiles))]
	private async Task LoadStashedFileAsync(string fileName)
	{
		if (await PromptSaveAsync() == null) return;

		await LoadFileAsync(Stash.SourcePath + fileName);
	}

	[RelayCommand]
	[property: ExecutableAction("Refresh Stash", "Stash > Refresh")]
	private async Task RefreshStashAsync()
	{
		await Stash.RefreshStashAsync();
		OnPropertyChanged(nameof(StashedFiles));
	}

	[RelayCommand]
	[property: ExecutableAction("Open Stash Folder", "Stash > Open Stash")]
	private void OpenStash() => OpenDirectory(Stash.SourcePath);

	[RelayCommand]
	[property: ExecutableAction("Unstash File", "Stash > Unstash", "", nameof(StashedFiles))]
	private async Task UnstashFileAsync(string fileName)
	{
		await Stash.UnstashFileAsync(fileName);
		await RefreshStashAsync();
	}

	#endregion

	#region Templates Menu

	/// <summary>
	/// List of template files in <see cref="Stash.SourcePath"/>.
	/// </summary>
	public ObservableCollection<string> Templates => new(TemplatesStash.StashedFiles);

	[RelayCommand]
	[property: ExecutableAction("New Template", "Template > New")]
	private async Task SaveAsTemplateAsync()
	{
		string? result = _dialogService.GetInput("Templates", "Provide a name for your template:", true);
		if (result == null) return;

		await TemplatesStash.StashFileAsync(result, InputText);
		await RefreshTemplatesAsync();
	}

	[RelayCommand]
	[property: ExecutableAction("Edit Template", "Template > Edit", "", nameof(Templates))]
	private async Task EditTemplateAsync(string fileName) => await LoadFileAsync(TemplatesStash.SourcePath + fileName);

	[RelayCommand]
	[property: ExecutableAction("Load Template", "Template > Load", "", nameof(Templates))]
	private async Task LoadTemplateAsync(string fileName)
	{
		if (await PromptSaveAsync() == null)
		{
			return;
		}

		InputText = await Task.Run(() => File.ReadAllText(TemplatesStash.SourcePath + fileName));
	}

	[RelayCommand]
	[property: ExecutableAction("Append Template", "Template > Append", "", nameof(Templates))]
	private async Task AppendTemplateAsync(string fileName)
	{
		string content = await Task.Run(() => File.ReadAllText(TemplatesStash.SourcePath + fileName));
		InputText += "\n" + content;
	}

	[RelayCommand]
	[property: ExecutableAction("Refresh Templates", "Template > Refresh")]
	private async Task RefreshTemplatesAsync()
	{
		await TemplatesStash.RefreshStashAsync();
		OnPropertyChanged(nameof(Templates));
	}

	[RelayCommand]
	[property: ExecutableAction("Open Templates Folder", "Template > Open Templates")]
	private void OpenTemplates() => OpenDirectory(TemplatesStash.SourcePath);

	[RelayCommand]
	[property: ExecutableAction("Delete Template", "Template > Delete", "", nameof(Templates))]
	private async Task DeleteTemplateAsync(string fileName)
	{
		await TemplatesStash.UnstashFileAsync(fileName);
		await RefreshTemplatesAsync();
	}

	#endregion

	#region Options Menu

	[RelayCommand]
	[property: ExecutableAction("Settings", "OptionsSource > Settings")]
	private void ShowSettings()
	{
		_dialogService.ShowSettings();
		LoadSettings();
	}

	#endregion

	#region CommandPalette

	public ActionPaletteViewModel ActionPaletteViewModel
	{
		get
		{
			ActionPaletteViewModel viewModel = App.Current.Services.GetRequiredService<ActionPaletteViewModel>();
			viewModel.Actions = GetActions();
			return viewModel;
		}
	}

	private List<ExecutableAction> GetActions()
	{
		List<ExecutableAction> actions =
			GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Select(property =>
			{
				if (property.GetCustomAttribute<ExecutableActionAttribute>() != null)
				{
					// Get IRelayCommands
					if (typeof(IRelayCommand).IsAssignableFrom(property.PropertyType))
					{
						return new ExecutableAction(
						   command: (IRelayCommand)property.GetValue(this)!,
						   description: property.GetCustomAttribute<ExecutableActionAttribute>()?.Description!,
						   source: property.GetCustomAttribute<ExecutableActionAttribute>()?.Source!,
						   inputGesture: property.GetCustomAttribute<ExecutableActionAttribute>()?.InputGesture!,
						   optionsGetter: () => (ObservableCollection<string>)
												GetType()
												.GetProperty(property.GetCustomAttribute<ExecutableActionAttribute>()?.OptionsSource!)
												?.GetValue(this)!
					   );
					}
					// Get toggleable boolean properties
					else if (property.PropertyType == typeof(bool))
					{
						return new ExecutableAction(
						   isEnabledGetter: () => (bool)property.GetValue(this)!,
						   isEnabledSetter: newValue => property.SetValue(this, newValue),
						   description: property.GetCustomAttribute<ExecutableActionAttribute>()?.Description!,
						   source: property.GetCustomAttribute<ExecutableActionAttribute>()?.Source!
					   );
					}
				}

				return null;
			})
			.Where(action => action != null)
			.ToList()!;

		return actions;
	}

	#endregion

	#region Editor

	/// <summary>
	/// Gets or sets the text that the user has input. A file's contents is loaded into this property and upon
	/// save the contents of the property are persisted to disk. Notifies the <see cref="DocumentModified"/>
	/// property.
	/// </summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(DocumentModified))]
	private string _inputText;
	partial void OnInputTextChanged(string value) => Document.NewText = value;

	/// <summary>
	/// Gets the text that is in the file on disk (i.e. persistent text).
	/// </summary>
	public string SavedText => Document.Text;

	/// <summary>
	/// Gets or sets the selected text in the main editor.
	/// </summary>
	[ObservableProperty]
	private string _selectedText;

	private int _fontSize;

	/// <summary>
	/// Gets or set the current font size of the main text editor.
	/// </summary>
	public int FontSize
	{
		get => _fontSize;
		set
		{
			if (value > 0) SetProperty(ref _fontSize, value);
		}
	}

	/// <summary>
	/// Gets or sets the current font family of the main text editor.
	/// </summary>
	[ObservableProperty]
	private FontFamily _fontFamily;

	private bool _isTyping;

	/// <summary>
	/// Gets or sets whether the user is currently typing. When the autosave timer is enabled, the timer starts
	/// if the user is not typing and stops if the user is typing.
	/// </summary>
	public bool IsTyping
	{
		get => _isTyping;
		set
		{
			_autosaveTimer.IsEnabled = !value;
			SetProperty(ref _isTyping, value);
		}
	}

	#region Editor Context Menu

	[RelayCommand]
	[property: ExecutableAction("Search Text in Web", "Editor Context Menu > Search in Web")]
	private void SearchInWeb() => _dialogService.SearchInWeb(SelectedText); 

	#endregion

	#endregion

	#region Unicode Combobox Data & Commands

	/// <summary>
	/// Gets or sets the last unicode character inserted.
	/// </summary>
	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(InsertLastUnicodeCharacterCommand))]
	private string _lastUnicodeChar;

	/// <summary>
	/// The currently selected unicode group to display.
	/// </summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(AvailableUnicodeChars))]
	private string _selectedUnicodeGroup;

	/// <summary>
	/// List of available unicode names.
	/// </summary>
	public List<string> AvailableUnicodeNames => UnicodeManager.Names;

	/// <summary>
	/// List of available unicode characters based on
	/// <see cref="SelectedUnicodeGroup"/>.
	/// </summary>
	public List<string> AvailableUnicodeChars => UnicodeManager.GetUnicodeCharsBasedOnName(SelectedUnicodeGroup);

	[RelayCommand]
	private void InsertUnicode(string text)
	{
		_textEditorService.Insert(text);
		LastUnicodeChar = text;
	}

	#endregion

	#region Text Statistics

	/// <summary>
	/// Gets the current character count including spaces of the <see cref="InputText"/>.
	/// </summary>
	public int CharCount => TextCountUtility.CharCount(InputText);

	/// <summary>
	/// Gets the current character count excluding spaces of the <see cref="InputText"/>.
	/// </summary>
	public int CharNoSpacesCount => TextCountUtility.CharNoSpaceCount(InputText);

	/// <summary>
	/// Gets the current word count including spaces of the <see cref="InputText"/>.
	/// </summary>
	public int WordCount => TextCountUtility.WordCount(InputText);

	/// <summary>
	/// Gets the current line count including spaces of the <see cref="InputText"/>.
	/// </summary>
	public int LineCount => TextCountUtility.LineCount(InputText);

	/// <summary>
	/// Gets the current paragraph count including spaces of the <see cref="InputText"/>.
	/// </summary>
	public int ParagraphCount => TextCountUtility.ParagraphCount(InputText);

	/// <summary>
	/// Updates all the text count properties like word and character count.
	/// </summary>
	public void UpdateTextInfo()
	{
		OnPropertyChanged(nameof(CharCount));
		OnPropertyChanged(nameof(CharNoSpacesCount));
		OnPropertyChanged(nameof(WordCount));
		OnPropertyChanged(nameof(LineCount));
		OnPropertyChanged(nameof(ParagraphCount));
	}

	#endregion

	#region Document Info

	/// <summary>
	/// Gets whether the document has been modified (text that has not been saved).
	/// </summary>
	public bool DocumentModified => Document.IsModified;

	/// <summary>
	/// Gets or sets the name of the current file. When setting the name, the file is renamed on disk as well.
	/// </summary>
	/// <remarks>Extension included.</remarks>
	public string FileNameDisplayText
	{
		get => string.IsNullOrEmpty(Document.FileName) ? "Untitled" : Document.FileName;
		set => Document.Rename(value);
	}

	/// <summary>
	/// Gets a string of the current document's size in KB.
	/// </summary>
	public long DocumentSize => Document.Size;

	/// <summary>
	/// Gets the current document's file path.
	/// </summary>
	public string DocumentPath => Document.FilePath;

	/// <summary>
	/// Gets the current document's creation date.
	/// </summary>
	public DateTime DocumentDateCreated => Document.DateCreated;

	/// <summary>
	/// Gets the current document's last modification date.
	/// </summary>
	public DateTime DocumentDateModified => Document.DateModified;

	/// <summary>
	/// Gets the current document's last access date.
	/// </summary>
	public DateTime DocumentDateAccessed => Document.DateAccessed;

	/// <summary>
	/// Notifies all relevant file info properties of change.
	/// </summary>
	public void UpdateFileInfo()
	{
		OnPropertyChanged(nameof(DocumentSize));
		OnPropertyChanged(nameof(DocumentPath));
		OnPropertyChanged(nameof(DocumentDateAccessed));
		OnPropertyChanged(nameof(DocumentDateCreated));
		OnPropertyChanged(nameof(DocumentDateModified));
	}

	#endregion

	#region Timer

	private async void TimerTickAsync(object? sender, EventArgs e)
	{
		if (Document.IsEmpty || !Document.IsModified || AutosaveTimerInterval == 0)
		{
			_autosaveTimer.IsEnabled = false;

		}
		else
		{
			await SaveFileAsync();
			_autosaveTimer.IsEnabled = false;
		}
	}

	#endregion

	#region Settings

	public void LoadSettings()
	{
		FontFamily = new FontFamily(_settings.FontFamily);
		FontSize = _settings.FontSize;
		DefaultFontSize = _settings.FontSize;
		RecentFilesManager.Max = _settings.MaxRecentFiles;
		if (_settings.RecentFiles.Count == 0) RecentFilesManager.Clear();
		OnPropertyChanged(nameof(RecentFiles));
	}

	#endregion
}