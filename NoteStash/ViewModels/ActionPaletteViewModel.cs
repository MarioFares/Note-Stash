using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NoteStash.Models;
using NoteStash.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NoteStash.ViewModels;

public partial class ActionPaletteViewModel : ObservableObject
{
	private readonly IDialogService _dialogService;

	public ActionPaletteViewModel(IDialogService dialogService)
	{
		_dialogService = dialogService;
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
		OnPropertyChanged(nameof(FilteredActions));
		SelectedIndex = 0;
	}

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
}
