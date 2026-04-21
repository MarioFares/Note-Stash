using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteStash.Models;
using NoteStash.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NoteStash.ViewModels;

public partial class ActionPaletteViewModel : ObservableObject
{
	private readonly IDialogService _dialogService;
	private readonly IAiService _aiService;
	private readonly ITextEditorService _textEditorService;

	public ActionPaletteViewModel(IDialogService dialogService, IAiService aiService, ITextEditorService textEditorService)
	{
		_dialogService = dialogService;
		_aiService = aiService;
		_textEditorService = textEditorService;
	}

	public List<ExecutableAction> Actions { get; set; }

	[ObservableProperty] 
	private bool _isOpen;

	[ObservableProperty] 
	private int _selectedIndex = 0;

	[RelayCommand]
	private void Hide() => IsOpen = false;

	[RelayCommand]
	private void ToggleShow() => IsOpen = !IsOpen;

	[ObservableProperty]
	private string _searchText;
	partial void OnSearchTextChanged(string value)
	{
		// Check if AI mode should be activated
		IsAiMode = value?.StartsWith(">") ?? false;
		
		if (!IsAiMode)
		{
			OnPropertyChanged(nameof(FilteredActions));
			SelectedIndex = 0;
		}
	}

	[ObservableProperty]
	private bool _isAiMode;

	[ObservableProperty]
	private string _aiOutput;

	[ObservableProperty]
	private bool _isAiLoading;

	[RelayCommand]
	public void ExecuteAction(ExecutableAction action)
	{
		Hide();
		if (action.NeedsArgument)
		{
			string? arg = _dialogService.GetInput("Options", "Provide an option:", false, action.ArgumentOptions);
			if (arg == null) return;

			action.Execute(arg);
		}
		else action.Execute();
	}

	/// <summary>
	/// Gets commands filtered based on descriptions that match <see cref="SearchText"/>.
	/// </summary>
	public ObservableCollection<ExecutableAction> FilteredActions =>
			new(Actions
			.Where(action =>
				(string.IsNullOrEmpty(SearchText) || action.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
				(action.CommandCanExecute || action.Toggleable))
			.OrderBy(action =>
			{
				int index = action.Description.IndexOf(SearchText ?? string.Empty, StringComparison.OrdinalIgnoreCase);
				return index;
			})
			.ThenBy(action => action.Description));

	[RelayCommand]
	private async Task ExecuteAiAsync(string instruction)
	{
		if (string.IsNullOrWhiteSpace(instruction))
		{
			AiOutput = "";
			return;
		}

		IsAiLoading = true;
		AiOutput = "";

		try
		{
			string prompt = BuildPromptWithContext(instruction);
			AiOutput = await _aiService.GenerateContent(prompt);
		}
		catch (Exception ex)
		{
			AiOutput = $"Error: {ex.Message}";
		}
		finally
		{
			IsAiLoading = false;
		}
	}

	public async Task ProcessAiInstruction(string instruction)
	{
		await ExecuteAiAsync(instruction);
	}
	private string BuildPromptWithContext(string instruction)
	{
		string selectedText = _textEditorService.GetSelectedText();
		
		if (string.IsNullOrEmpty(selectedText))
		{
			return instruction;
		}
		
		return $"{instruction}\n\nContext:\n{selectedText}";
	}

	[RelayCommand]
	private void CopyAiOutput()
	{
		if (!string.IsNullOrEmpty(AiOutput))
		{
			System.Windows.Forms.Clipboard.SetText(AiOutput);
		}
	}
}

