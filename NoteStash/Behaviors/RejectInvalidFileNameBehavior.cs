using Microsoft.Xaml.Behaviors;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoteStash.Behaviors;

/// <summary>
/// Behavior for rejecting text input that is invalid for file naming.
/// </summary>
public class RejectInvalidFileNameBehavior : Behavior<TextBox>
{
	/// <summary>
	/// Connects the <see cref="UIElement.PreviewTextInput"/> event to a handler.
	/// </summary>
	protected override void OnAttached()
	{
		base.OnAttached();
		TextBox textBox = AssociatedObject;
		textBox.PreviewTextInput += OnPreviewTextInput;
	}

	/// <summary>
	/// Disconnects the <see cref="UIElement.PreviewTextInput"/> event to a handler.
	/// </summary>
	protected override void OnDetaching()
	{
		base.OnDetaching();
		TextBox textBox = AssociatedObject;
		textBox.PreviewTextInput -= OnPreviewTextInput;
	}

	private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		// Works for /\:*?"<>| on windows
		if (e.Text.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
		{
			e.Handled = true;
		}
	}
}