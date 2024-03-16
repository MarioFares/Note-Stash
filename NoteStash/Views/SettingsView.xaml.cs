using NoteStash.ViewModels;

namespace NoteStash.Views;

/// <summary>
/// Interaction logic for SettingsView.xaml
/// </summary>
public partial class SettingsView
{
	/// <summary>
	/// Initializes SettingsView.
	/// </summary>
	public SettingsView()
	{
		InitializeComponent();
	}

	private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		SettingsViewModel viewModel = DataContext as SettingsViewModel;
		viewModel.SaveSettings();
	}
}