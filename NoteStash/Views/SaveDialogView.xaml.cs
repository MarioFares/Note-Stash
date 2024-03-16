using NoteStash.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NoteStash.Views;

/// <summary>
/// Interaction logic for SaveDialogView.xaml
/// </summary>
public partial class SaveDialogView
{
	/// <summary>
	/// Initializes dialog window and sets focus on save button.
	/// </summary>
	public SaveDialogView()
	{
		InitializeComponent();
		SaveButton.Focus();
	}

	private void OnButtonClick(object? sender, RoutedEventArgs e)
	{
		SaveDialogViewModel? viewModel = DataContext as SaveDialogViewModel;
		if (viewModel == null) return;

		sender = sender as Button;
		if (sender == null) return;

		if (sender.Equals(SaveButton))
		{
			viewModel.ToSave = true;
		}
		else if (sender.Equals(NoSaveButton))
		{
			viewModel.ToSave = false;
		}
		else
		{
			viewModel.ToSave = null;
		}

		Close();
	}

	private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DragMove();
}