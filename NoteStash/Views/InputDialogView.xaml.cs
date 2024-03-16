using NoteStash.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoteStash.Views;

/// <summary>
/// Interaction logic for InputDialogView.xaml
/// </summary>
public partial class InputDialogView
{
	/// <summary>
	/// Initializes dialog window and sets up event handlers.
	/// </summary>
	public InputDialogView()
	{
		InitializeComponent();
		OkButton.Click += (_, _) => Close();
	}

	private void OnCancel(object sender, RoutedEventArgs e)
	{
		InputDialogViewModel? viewModel = DataContext as InputDialogViewModel;
		viewModel?.Cancel();
		Close();
	}

	private void OnEnter(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter) Close();
	}

	private void OnMouseDown(object sender, MouseButtonEventArgs e) => DragMove();

	private void OnControlLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is TextBox textBox)
		{
			textBox.Focus();
		}
		else if (sender is ComboBox comboBox)
		{
			comboBox.Focus();
		}
	}
}

