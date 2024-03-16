using NoteStash.Models;
using NoteStash.ViewModels;
using System;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace NoteStash.Views;

/// <summary>
/// Interaction logic for ActionPaletteView.xaml
/// </summary>
public partial class ActionPaletteView
{
	public ActionPaletteView()
	{
		InitializeComponent();
		PlacementTarget = App.Current.MainWindow;

		// We use dispatcher because otherwise won't focus
		Dispatcher.BeginInvoke(new Action(() => SearchBox.Focus()));
	}

	private void Popup_Opened(object sender, EventArgs e)
	{
		SearchBox.Clear();
		SearchBox.Focus();
	}

	private void Popup_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		ActionPaletteViewModel viewModel = (ActionPaletteViewModel)DataContext;
		if (e.Key == Key.Enter && ActionsListBox.SelectedItem is ExecutableAction current)
		{
			viewModel.ExecuteAction(current);
		}
		else if (e.Key == Key.Down)
		{
			int currentIndex = ActionsListBox.SelectedIndex;
			if (ActionsListBox.Items.Count > currentIndex + 1)
			{
				ActionsListBox.SelectedIndex = ++currentIndex;
				ActionsListBox.ScrollIntoView(ActionsListBox.Items.GetItemAt(currentIndex));
			}
		}
		else if (e.Key == Key.Up)
		{
			int currentIndex = ActionsListBox.SelectedIndex;
			if (currentIndex > 0)
			{
				ActionsListBox.SelectedIndex = --currentIndex;
				ActionsListBox.ScrollIntoView(ActionsListBox.Items.GetItemAt(currentIndex));
			}
		}
	}
}
