using CommunityToolkit.Mvvm.ComponentModel;

namespace NoteStash.ViewModels;

/// <summary>
/// View model for the save dialog.
/// </summary>
public class SaveDialogViewModel : ObservableObject
{
	/// <summary>
	/// Boolean whether the user wants to save a relevant document or not. If the value is null, the user is
	/// signalling that they neither want to save the document nor discard. The callee function must respond
	/// accordingly.
	/// </summary>
	public bool? ToSave { get; set; }

	/// <summary>
	/// Gets the name of the file to be saved.
	/// </summary>
	public string FileName { get; set; }
}