using ICSharpCode.AvalonEdit.Search;
using NoteStash.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Point = System.Windows.Point;
using Settings = NoteStash.Models.Settings;
using Size = System.Windows.Size;

namespace NoteStash.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView
{
	private readonly Settings _settings;

	/// <summary>
	/// Initializes a new instance of the <see cref="MainView"/> and sets up the menu items of
	/// the edit menu.
	/// </summary>
	public MainView(Settings settings)
	{
		InitializeComponent();
		InputBox.Focus();
		SizeChanged += OnSizeChanged;

		SetupSearchPanel();
		_settings = settings;
	}

	private void SetupSearchPanel()
	{
		SearchPanel searchPanel = SearchPanel.Install(InputBox);
		searchPanel.OverridesDefaultStyle = true;
		searchPanel.Style = FindResource("SearchPanelStyle") as Style;
		searchPanel.SetResourceReference(SearchPanel.MarkerBrushProperty, "TextEditor.FindBrush");
		FindMenuItem.Click += (_, _) =>
		{
			searchPanel.Open();
			Dispatcher.BeginInvoke(DispatcherPriority.Input, searchPanel.Reactivate);
		};
	}

	#region Maximize & Fullscreen

	private void MaximizeHelper(double maxHeight, double maxWidth)
	{
		if (WindowState == WindowState.Normal)
		{
			MaxHeight = maxHeight;
			MaxWidth = maxWidth;
			WindowState = WindowState.Maximized;
			MaximizeIcon.Style = (Style)FindResource("IconChromeRestore");
		}
		else
		{
			WindowState = WindowState.Normal;
			MaximizeIcon.Style = (Style)FindResource("IconChromeMaximize");
		}
	}

	private void Fullscreen(object sender, RoutedEventArgs e) =>
		MaximizeHelper(double.PositiveInfinity, double.PositiveInfinity);

	protected override void MaximizeWindow(object sender, RoutedEventArgs e) =>
		MaximizeHelper(SystemParameters.MaximizedPrimaryScreenHeight - 9,
			SystemParameters.MaximizedPrimaryScreenWidth - 9);

	#endregion

	#region Tray Icon

	private void NotifyIcon_DoubleClick(object? sender, EventArgs eventArgs) => Show();

	private void MinimizeToTray(object sender, RoutedEventArgs e) => Hide();

	#endregion
	
	#region FileNameTextBox

	private void FileNameTextBox_GotFocus(object sender, RoutedEventArgs e)
	{
		// Set this first because does not select when false
		e.Handled = true;
		string text = FileNameTextBox.Text;
		FileNameTextBox.Text = Path.GetFileNameWithoutExtension(text);
		FileNameTextBox.SelectAll();
	}

	private void FileNameTextBox_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Return) InputBox.Focus();
	}

	#endregion

	#region File Drop

	private void InputBox_Drop(object sender, DragEventArgs e)
	{
		if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

		string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop)!;
		MainViewModel? viewModel = DataContext as MainViewModel;
		viewModel?.LoadFileAsync(filePaths[0]);
	}

	#endregion

	#region On Close

	private void CloseHandler(object sender, CancelEventArgs e)
	{
		bool minimizeToTray = _settings.ExitMinimizeToTray;
		if (minimizeToTray)
		{
			e.Cancel = true;
			MinimizeToTray(null!, null!);
		}
		else
		{
			Shutdown(null!, e);
		}
	}

	private async void Shutdown(object sender, CancelEventArgs e)
	{
		MainViewModel viewModel = DataContext as MainViewModel ?? throw new InvalidOperationException();
		if (await viewModel.PromptSaveAsync() == null)
		{
			e.Cancel = true;
			InputBox.Focus();
			return;
		}

		viewModel.InsertRecentFile(viewModel.DocumentPath, string.Empty);
		App.Current.Shutdown();
	}

	private void Shutdown(object? sender, EventArgs e) => Shutdown(sender!, new CancelEventArgs());

	#endregion

	#region File & Text Popups

	private CustomPopupPlacement[] OnCustomPopupPlacement(Size popupSize, Size targetSize, Point offset)
	{
		return new CustomPopupPlacement[]
		{
			new()
			{
				Point = new Point(targetSize.Width - popupSize.Width, -popupSize.Height - 3),
				PrimaryAxis = PopupPrimaryAxis.Horizontal
			},
			new()
			{
				Point = new Point(0, -popupSize.Height - 3),
				PrimaryAxis = PopupPrimaryAxis.Vertical
			}
		};
	}

	private void OnFileInfoPopupOpen(object sender, EventArgs e)
	{
		if (DataContext is MainViewModel viewModel) viewModel.UpdateFileInfo();
	}

	private void OnTextInfoPopupOpen(object sender, EventArgs e)
	{
		if (DataContext is MainViewModel viewModel) viewModel.UpdateTextInfo();
	}

	#endregion

	#region Unicode Keybinding Generation

	// Assign key bindings to generated unicode items: Ctrl+1, Ctrl+2, ..., Ctrl+9
	private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
		{
			if (e.Key >= Key.D1 && e.Key <= Key.D9)
			{
				int index = e.Key - Key.D1; // Calculate the index based on the pressed number key
				if (index < UnicodeItemsControl.Items.Count)
				{
					if (UnicodeItemsControl.ItemContainerGenerator.ContainerFromIndex(index) is FrameworkElement itemContainer)
					{
						Button button = FindVisualChild<Button>(itemContainer)!;
						button?.Command.Execute(button.Content ?? string.Empty);
					}
				}
			}
		}
	}

	// Return visual child of type T
	// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-find-datatemplate-generated-elements?view=netframeworkdesktop-4.8
	private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
	{
		int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int i = 0; i < childrenCount; i++)
		{
			var child = VisualTreeHelper.GetChild(parent, i);
			if (child != null && child is T)
			{
				return (T)child;
			}
			else
			{
				T childOfChild = FindVisualChild<T>(child);
				if (childOfChild != null) return childOfChild;
			}
		}

		return null;
	}

	#endregion

	#region CommandPalette

	private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (ActionPalette.IsOpen && !ActionPalette.IsMouseOver)
		{
			ActionPalette.IsOpen = false;
			e.Handled = true;
		}
	}

	private void ActionPalette_Closed(object sender, EventArgs e) => InputBox.Focus();

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
		ActionPalette.Width = Width - 200;
		ActionPalette.Height = Height - 200;
	}

	#endregion
}