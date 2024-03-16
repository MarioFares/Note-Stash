using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NoteStash.Base;

/// <summary>
/// Base class for WPF Window with standard functions to minimize, maximize, and close.
/// </summary>
public class WindowBase : Window
{
	// Below fix for menus to appear right to left properly
	// https://stackoverflow.com/questions/37326546/why-are-my-menus-opening-right-to-left-instead-of-left-to-
	private static FieldInfo? _menuDropAlignmentField;

	/// <summary>
	/// Initializes the window and sets menus to open aligned to the right.
	/// </summary>
	protected WindowBase()
	{
		_menuDropAlignmentField =
			typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
		System.Diagnostics.Debug.Assert(_menuDropAlignmentField != null);

		EnsureStandardPopupAlignment();
		SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
	}

	// Fix for menus to appear right to left properly
	private static void SystemParameters_StaticPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		EnsureStandardPopupAlignment();
	}

	// Fix for menus to appear right to left properly
	private static void EnsureStandardPopupAlignment()
	{
		if (SystemParameters.MenuDropAlignment)
		{
			_menuDropAlignmentField?.SetValue(null, false);
		}
	}

	/// <summary>
	/// Move current window or maximize it if two clicks are detected.
	/// </summary>
	protected void MoveWindow(object sender, MouseButtonEventArgs e)
	{
		if (e.ClickCount >= 2)
		{
			MaximizeWindow(sender, e);
		}
		else if (WindowState == WindowState.Maximized)
		{
			Point mousePos = e.GetPosition(this);
			WindowState = WindowState.Normal;

			Left = mousePos.X - (RestoreBounds.Width * mousePos.X / ActualWidth);
			Top = mousePos.Y - (RestoreBounds.Height * mousePos.Y / ActualHeight);

			DragMove();
		}
		else
		{
			DragMove();
		}
	}

	/// <summary>
	/// Close current window.
	/// </summary>
	protected void CloseWindow(object sender, RoutedEventArgs e)
	{
		Close();
	}

	/// <summary>
	/// Maximize current window.
	/// </summary>
	protected virtual void MaximizeWindow(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
	}

	/// <summary>
	/// Minimize current window.
	/// </summary>
	protected void MinimizeWindow(object sender, RoutedEventArgs e)
	{
		WindowState = WindowState.Minimized;
	}
}