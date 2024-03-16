using ICSharpCode.AvalonEdit;
using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace NoteStash.Behaviors;

/// <summary>
/// Behavior for auto closing characters like ( [ { ". 
/// </summary>
public class AutocloseTextBehavior : Behavior<TextEditor>
{
	private readonly Dictionary<string, string> _charPairs = new()
	{
		{ "[", "]" },
		{ "{", "}" },
		{ "(", ")" },
		{ "\"", "\"" },
	};

	/// <summary>
	/// Connects the <see cref="UIElement.PreviewTextInput"/> event to a handler.
	/// </summary>
	protected override void OnAttached()
	{
		base.OnAttached();
		TextEditor textBox = AssociatedObject;
		textBox.PreviewTextInput += OnPreviewTextInput;
	}

	/// <summary>
	/// Disconnects the <see cref="UIElement.PreviewTextInput"/> event to a handler.
	/// </summary>
	protected override void OnDetaching()
	{
		base.OnDetaching();
		TextEditor textBox = AssociatedObject;
		textBox.PreviewTextInput -= OnPreviewTextInput;
	}

	private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
	{
		TextEditor editor = AssociatedObject;
		string cinput = e.Text;
		if (!_charPairs.ContainsKey(cinput)) return;

		if (editor.SelectedText.Length > 0)
		{
			editor.SelectedText = cinput + editor.SelectedText + _charPairs[cinput];
		}
		else
		{
			editor.Document.Insert(editor.CaretOffset, cinput + _charPairs[cinput]);
			--editor.CaretOffset;
		}

		e.Handled = true;
	}
}