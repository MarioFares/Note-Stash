using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace NoteStash.ViewModels;

/// <summary>
/// View model for the general input dialog.
/// </summary>
public partial class InputDialogViewModel : ObservableObject
{
	/// <summary>
	/// Gets or sets the currently selected index of the combobox if it is used.
	/// </summary>
	public int SelectedIndex { get; set; } = 0;

	/// <summary>
	/// Gets whether the input should be a valid file name.
	/// </summary>
	public bool IsGetFileName { get; set; }

	/// <summary>
	/// Gets whether there are multiple options to choose from.
	/// </summary>
	public bool IsComboBoxMode => Options != null && Options.Count > 0;

	/// <summary>
	/// Gets or sets the options to choose from.
	/// </summary>
	public ObservableCollection<string>? Options { get; set; }

	/// <summary>
	/// Gets or sets the title of the dialog.
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Gets or sets the prompt of the dialog.
	/// </summary>
	public string Prompt { get; set; }

	/// <summary>
	/// Gets or sets the nullable result of the dialog. If the value is null, then the dialog was cancelled.
	/// The user should appropriately return in the function that checks this result.
	/// </summary>
	public string? Result { get; set; }

	/// <summary>
	/// Sets <see cref="Result"/> to null.
	/// </summary>
	public void Cancel() => Result = null;
}
