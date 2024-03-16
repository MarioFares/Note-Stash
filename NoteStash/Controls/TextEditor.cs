using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
using NoteStash.Utilities;
using System;
using System.Windows;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace NoteStash.Controls;

/// <summary>
/// Interaction logic for TextEditor.xaml
/// </summary>
public class TextEditor : ICSharpCode.AvalonEdit.TextEditor
{
	public TextEditor()
	{
		Padding = new Thickness(10);
		TextArea.SelectionBorder = new(Brushes.Transparent, 0);
		TextArea.SelectionCornerRadius = 3;

		// Events
		TextArea.Caret.PositionChanged += OnCaretPositionChanged;
		TextArea.SelectionChanged += OnSelectionChanged;
		TextChanged += OnTextChanged;
		PreviewDragOver += OnPreviewDragOver;
		KeyUp += OnKeyUp;
		KeyDown += OnKeyDown;

		// Override Ctrl+D so that it can be used for duplicate
		AvalonEditCommands.DeleteLine.InputGestures.Clear();
	}

	#region IndentationSize Property

	public static readonly DependencyProperty IndentationSizeProperty =
		DependencyProperty.Register(
			name: nameof(IndentationSize),
			propertyType: typeof(int),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(2, OnIndentationSizeChanged));

	/// <summary>
	/// Gets or sets the tab size in spaces. 0 is for the tab character \t whereas any positive number will be
	/// the number of spaces inserted when the tab key is pressed.
	/// </summary>
	public int IndentationSize
	{
		get => (int)GetValue(IndentationSizeProperty);
		set => SetValue(IndentationSizeProperty, value);
	}

	private static void OnIndentationSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is int indentationSize)
		{
			textEditor.TextArea.Options.IndentationSize = indentationSize < 1 ? 4 : indentationSize;
		}
	}

	#endregion

	#region ConvertTabToSpaces Property

	public static readonly DependencyProperty ConvertTabToSpacesProperty =
		DependencyProperty.Register(
			name: nameof(ConvertTabToSpaces),
			propertyType: typeof(bool),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(false, OnConvertTabToSpacesChanged));

	/// <summary>
	/// Gets or set whether to covert tab characters to spaces.
	/// </summary>
	public bool ConvertTabToSpaces
	{
		get => (bool)GetValue(ConvertTabToSpacesProperty);
		set => SetValue(ConvertTabToSpacesProperty, value);
	}

	private static void OnConvertTabToSpacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is bool convertTabToSpaces)
		{
			textEditor.TextArea.Options.ConvertTabsToSpaces = convertTabToSpaces;
		}
	}

	#endregion

	#region IsTyping Property

	public static readonly DependencyProperty IsTypingProperty =
		DependencyProperty.Register(
			name: nameof(IsTyping),
			propertyType: typeof(bool),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(false));

	/// <summary>
	/// Whether the user is currently typing.
	/// </summary>
	public bool IsTyping
	{
		get => (bool)GetValue(IsTypingProperty);
		set => SetValue(IsTypingProperty, value);
	}

	private void OnKeyDown(object sender, KeyEventArgs e) => IsTyping = true;

	private void OnKeyUp(object sender, KeyEventArgs e) => IsTyping = false;

	#endregion

	#region Line & Charcater Index

	#region LineIndex Property

	public static readonly DependencyProperty LineIndexProperty =
		DependencyProperty.Register(
			name: nameof(LineIndex),
			propertyType: typeof(int),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(1));

	/// <summary>
	/// Gets the index of the line the caret is currently on. 1-based.
	/// </summary>
	public int LineIndex
	{
		get => (int)GetValue(LineIndexProperty);
		private set
		{
			if (value < 0) value = 0;
			SetValue(LineIndexProperty, value);
		}
	}

	#endregion

	#region LineCharacterIndex Property

	public static readonly DependencyProperty LineCharacterIndexProperty =
		DependencyProperty.Register(
			name: nameof(LineCharacterIndex),
			propertyType: typeof(int),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(1));

	/// <summary>
	/// Gets the index of the character the caret is currently on relative to the start of the line. 1-based.
	/// </summary>
	public int LineCharacterIndex
	{
		get => (int)GetValue(LineCharacterIndexProperty);
		private set
		{
			if (value < 0) value = 0;
			SetValue(LineCharacterIndexProperty, value);
		}
	}

	#endregion

	private void OnCaretPositionChanged(object? sender, EventArgs eventArgs)
	{
		LineIndex = Document.GetLineByOffset(CaretOffset).LineNumber;
		LineCharacterIndex = CaretOffset - Document.GetLineByOffset(CaretOffset).Offset;

		// Account for 0-based indexing
		++LineCharacterIndex;
	}

	#endregion

	#region BindableSelectedText Property

	public static readonly DependencyProperty BindableSelectedTextProperty =
		DependencyProperty.Register(
			name: nameof(BindableSelectedText),
			propertyType: typeof(string),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(string.Empty));

	/// <summary>
	/// Gets or sets the selected text.
	/// </summary>
	/// <remarks>Used since the built-in property is not a dependency property</remarks>
	public string BindableSelectedText
	{
		get => (string)GetValue(BindableSelectedTextProperty);
		set => SetValue(BindableSelectedTextProperty, value);
	}

	private void OnSelectionChanged(object? sender, EventArgs e) => BindableSelectedText = SelectedText;

	#endregion

	#region CaretBrush Property

	public static readonly DependencyProperty CaretBrushProperty =
		DependencyProperty.Register(
			name: nameof(CaretBrush),
			propertyType: typeof(Brush),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(Brushes.White, OnCaretBrushChanged));

	/// <summary>
	/// Gets or sets the caret brush.
	/// </summary>
	public Brush CaretBrush
	{
		get => (Brush)GetValue(CaretBrushProperty);
		set => SetValue(CaretBrushProperty, value);
	}

	private static void OnCaretBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is Brush caretBrush)
		{
			// If the caretBrush is a SolidColorBrush, use it directly
			textEditor.TextArea.Caret.CaretBrush = caretBrush;
		}
	}

	#endregion

	#region SelectionBrush Property

	public static readonly DependencyProperty SelectionBrushProperty =
		DependencyProperty.Register(
			name: nameof(SelectionBrush),
			propertyType: typeof(Brush),
			ownerType: typeof(TextEditor),
			typeMetadata: new PropertyMetadata(Brushes.Blue, OnSelectionBrushChanged));

	/// <summary>
	/// Gets or sets the selection brush.
	/// </summary>
	public Brush SelectionBrush
	{
		get => (Brush)GetValue(CaretBrushProperty);
		set => SetValue(CaretBrushProperty, value);
	}

	private static void OnSelectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is Brush selectionBrush)
		{
			// If the caretBrush is a SolidColorBrush, use it directly
			textEditor.TextArea.SelectionBrush = selectionBrush;
			//textEditor.TextArea.SelectionBorder.Brush = selectionBrush;
		}
	}

	#endregion

	#region ShowLineNumbers Property

	public new static readonly DependencyProperty ShowLineNumbersProperty =
		DependencyProperty.Register(
			name: nameof(ShowLineNumbers),
			propertyType: typeof(bool),
			ownerType: typeof(TextEditor),
			typeMetadata: new FrameworkPropertyMetadata(false, OnShowLineNumbersChanged));

	/// <summary>
	/// Gets or sets whether to show line numbers.
	/// </summary>
	public new bool ShowLineNumbers
	{
		get => (bool)GetValue(ShowLineNumbersProperty);
		set => SetValue(ShowLineNumbersProperty, value);
	}

	// Modified based on the AvalonEdit open source code
	// DottedLineMargin is replaced with a solid line.
	static void OnShowLineNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TextEditor editor = (TextEditor)d;
		var leftMargins = editor.TextArea.LeftMargins;
		if ((bool)e.NewValue)
		{
			LineNumberMargin lineNumbers = new()
			{
				Margin = new Thickness(0, 0, 10, 0)
			};
			leftMargins.Insert(0, lineNumbers);
			var lineNumbersForeground = new Binding("LineNumbersForeground") { Source = editor };
			lineNumbers.SetBinding(ForegroundProperty, lineNumbersForeground);
		}
		else
		{
			for (int i = 0; i < leftMargins.Count; i++)
			{
				// Condition modified so as to remove the DottedLineMargin
				if (leftMargins[i] is LineNumberMargin)
				{
					leftMargins.RemoveAt(i);
					if (i < leftMargins.Count)
					{
						leftMargins.RemoveAt(i);
					}

					break;
				}
			}
		}
	}

	#endregion

	#region ShowSpaces Property

	public static readonly DependencyProperty ShowSpacesProperty =
		DependencyProperty.Register(
			name: nameof(ShowSpaces),
			propertyType: typeof(bool),
			ownerType: typeof(TextEditor),
			typeMetadata: new FrameworkPropertyMetadata(false, OnShowSpacesChanged));

	/// <summary>
	/// Gets or sets whether to visually show spaces in the editor.
	/// </summary>
	public bool ShowSpaces
	{
		get => (bool)GetValue(ShowSpacesProperty);
		set => SetValue(ShowSpacesProperty, value);
	}

	private static void OnShowSpacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is bool showSpaces)
		{
			textEditor.TextArea.Options.ShowSpaces = showSpaces;
		}
	}

	#endregion

	#region ShowTabs Property

	public static readonly DependencyProperty ShowTabsProperty =
		DependencyProperty.Register(
			name: nameof(ShowTabs),
			propertyType: typeof(bool),
			ownerType: typeof(TextEditor),
			typeMetadata: new FrameworkPropertyMetadata(false, OnShowTabsChanged));

	/// <summary>
	/// Gets or sets whether to visually show tabs in the editor.
	/// </summary>
	public bool ShowTabs
	{
		get => (bool)GetValue(ShowTabsProperty);
		set => SetValue(ShowTabsProperty, value);
	}

	private static void OnShowTabsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is bool showTabs)
		{
			textEditor.TextArea.Options.ShowTabs = showTabs;
		}
	}

	#endregion

	#region ShowEndOfLine Property

	public static readonly DependencyProperty ShowEndOfLineProperty =
		DependencyProperty.Register(
			name: nameof(ShowEndOfLine),
			propertyType: typeof(bool),
			ownerType: typeof(TextEditor),
			typeMetadata: new FrameworkPropertyMetadata(false, OnShowEndOfLineChanged));

	/// <summary>
	/// Gets or sets whether to visually show line ends in the editor.
	/// </summary>
	public bool ShowEndOfLine
	{
		get => (bool)GetValue(ShowEndOfLineProperty);
		set => SetValue(ShowEndOfLineProperty, value);
	}

	private static void OnShowEndOfLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is bool showEndOfLine)
		{
			textEditor.TextArea.Options.ShowEndOfLine = showEndOfLine;
		}
	}

	#endregion

	#region BindableText

	public static readonly DependencyProperty BindableTextProperty =
		DependencyProperty.Register(
			name: nameof(BindableText),
			propertyType: typeof(string),
			ownerType: typeof(TextEditor),
			typeMetadata: new FrameworkPropertyMetadata(string.Empty, OnBindableTextChanged));

	/// <summary>
	/// Gets or sets the text property <see cref="ICSharpCode.AvalonEdit.TextEditor.Text"/> of the document.
	/// </summary>
	public string BindableText
	{
		get => (string)GetValue(BindableTextProperty);
		set => SetValue(BindableTextProperty, value);
	}

	private void OnTextChanged(object? sender, EventArgs eventArgs)
	{
		if (!IsReadOnly) BindableText = Document.Text;
	}

	private static void OnBindableTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is TextEditor textEditor && e.NewValue is string newText)
		{
			var caretOffset = textEditor.CaretOffset;
			textEditor.Document.Text = newText;

			try
			{
				textEditor.CaretOffset = caretOffset;
			}
			catch (ArgumentOutOfRangeException)
			{
				textEditor.CaretOffset = 0;
			}
		}
	}

	#endregion

	#region ApplyToSelected Functions

	private void ApplyToSelected(Func<string, string> func)
	{
		string selectedText = SelectedText;

		if (string.IsNullOrEmpty(selectedText))
		{
			SelectAll();
			SelectedText = func(Text);
			TextArea.ClearSelection();
		}
		else SelectedText = func(selectedText);
	}

	private void ApplyInsertionToSelected(Func<string, string, string> func, string arg)
	{
		string selectedText = SelectedText;

		if (string.IsNullOrEmpty(selectedText))
		{
			SelectAll();
			SelectedText = func(Text, arg);
			TextArea.ClearSelection();
		}
		else SelectedText = func(selectedText, arg);
	}

	#endregion

	#region Case Functions

	/// <summary>
	/// Changes the selected text to upper case.
	/// </summary>
	public void UpperCase() => AvalonEditCommands.ConvertToUppercase.Execute(null!, TextArea);

	/// <summary>
	/// Changes the selected text to lower case.
	/// </summary>
	public void LowerCase() => AvalonEditCommands.ConvertToLowercase.Execute(null!, TextArea);

	/// <summary>
	/// Changes the selected text to sentence case.
	/// </summary>
	public void SentenceCase() => ApplyToSelected(CaseUtility.SentenceCase);

	/// <summary>
	/// Changes the selected text to title case.
	/// </summary>
	public void TitleCase() => AvalonEditCommands.ConvertToTitleCase.Execute(null!, TextArea);

	/// <summary>
	/// Changes the selected text to invert case.
	/// </summary>
	public void InvertCase() => AvalonEditCommands.InvertCase.Execute(null!, TextArea);

	/// <summary>
	/// Changes the selected text to random case.
	/// </summary>
	public void RandomCase() => ApplyToSelected(CaseUtility.RandomCase);

	#endregion

	#region Format Functions

	/// <summary>
	/// Trims the selected text.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void Trim() => ApplyToSelected(FormatUtility.Trim);

	/// <summary>
	/// Trims the start of the selected text.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void TrimStart() => ApplyToSelected(FormatUtility.TrimStart);

	/// <summary>
	/// Trims the end of the selected text.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void TrimEnd() => ApplyToSelected(FormatUtility.TrimEnd);

	/// <summary>
	/// Trims the lines of the selected text.
	/// </summary>
	/// <remarks>For multiple lines</remarks>
	public void TrimLines() => ApplyToSelected(FormatUtility.TrimLines);

	/// <summary>
	/// Trims the starts of the lines of the selected text.
	/// </summary>
	/// <remarks>For multiple lines</remarks>
	public void TrimLinesStart() => ApplyToSelected(FormatUtility.TrimLineStarts);

	/// <summary>
	/// Trims the ends of the lines of the selected text.
	/// </summary>
	/// <remarks>For multiple lines</remarks>
	public void TrimLinesEnd() => ApplyToSelected(FormatUtility.TrimLineEnds);

	/// <summary>
	/// Converts spaces to tabs in the selected text or the whole text if nothing is selected.
	/// </summary>
	public void ConvertSpacesToTabs() => AvalonEditCommands.ConvertSpacesToTabs.Execute(null!, TextArea);

	/// <summary>
	/// Converts leading spaces to tabs in the selected text or the whole text if nothing is selected.
	/// </summary>
	public void ConvertLeadingSpacesToTabs() => AvalonEditCommands.ConvertLeadingSpacesToTabs.Execute(null!, TextArea);

	/// <summary>
	/// Converts tabs to spaces in the selected text or the whole text if nothing is selected.
	/// </summary>
	public void ConvertTabsToSpaces() => AvalonEditCommands.ConvertTabsToSpaces.Execute(null!, TextArea);

	/// <summary>
	/// Converts leading tabs to spaces in the selected text or the whole text if nothing is selected.
	/// </summary>
	public void ConvertLeadingTabsToSpaces() => AvalonEditCommands.ConvertLeadingTabsToSpaces.Execute(null!, TextArea);

	/// <summary>
	/// Normalizes vertical whitespace such that if there is more than a single empty line separating
	/// two paragraphs, they are reduced to one empty line.
	/// </summary>
	public void NormalizeVerticalWhitespace() => ApplyToSelected(FormatUtility.NormalizeVerticalWhitespace);

	/// <summary>
	/// Joins lines together by normalizing them
	/// and replacing new line characters with a single space.
	/// </summary>
	public void JoinLines() => ApplyToSelected(FormatUtility.JoinLines);

	/// <summary>
	/// Normalizes horizontal whitespace such that if there is more than a single space character or 
	/// tab character between two words, they are replaced by a single space character.
	/// </summary>
	public void NormalizeHorizontalWhitespace() => ApplyToSelected(FormatUtility.NormalizeHorizontalWhitespace);

	/// <summary>
	/// Splits words on every space character after normalizing horizontal whitespace.
	/// </summary>
	public void SplitWords() => ApplyToSelected(FormatUtility.SplitWords);

	#endregion

	#region Sort Functions

	/// <summary>
	/// Sorts the words of the selected text in ascending order.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void SortWordsAsc() => ApplyToSelected(FormatUtility.SortWordsAsc);

	/// <summary>
	/// Sorts the words of the selected text in descending order.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void SortWordsDesc() => ApplyToSelected(FormatUtility.SortWordsDesc);

	/// <summary>
	/// Sorts the lines of the selected text in ascending order.
	/// </summary>
	/// <remarks>For multiple lines.</remarks>
	public void SortLinesAsc() => ApplyToSelected(FormatUtility.SortLinesAsc);

	/// <summary>
	/// Sorts the lines of the selected text in descending order.
	/// </summary>
	/// <remarks>For multiple lines.</remarks>
	public void SortLinesDesc() => ApplyToSelected(FormatUtility.SortLinesDesc);

	#endregion

	#region Line Functions

	/// <summary>
	/// Selects the entire line at the current location of the caret.
	/// </summary>
	public void SelectCurrentLine()
	{
		DocumentLine currentLine = Document.GetLineByOffset(CaretOffset);
		int lineStart = currentLine.Offset;
		int lineEnd = currentLine.EndOffset;

		Select(lineStart, lineEnd - lineStart);
	}

	/// <summary>
	/// Deletes the entire line at the current location of the caret.
	/// </summary>
	public void DeleteCurrentLine()
	{
		DocumentLine currentLine = Document.GetLineByOffset(CaretOffset);
		if (currentLine.Length > 0)
		{
			AvalonEditCommands.DeleteLine.Execute(null!, TextArea);
		}
		else
		{
			if (currentLine.PreviousLine != null)
			{
				CaretOffset = currentLine.PreviousLine.EndOffset;
			}
		}
	}

	/// <summary>
	/// Duplicates the entire line at the current location of the caret.
	/// </summary>
	public void DuplicateCurrentLine()
	{
		DocumentLine currentLine = Document.GetLineByOffset(CaretOffset);
		int lineStart = currentLine.Offset;
		int lineEnd = currentLine.EndOffset;
		int lineLength = lineEnd - lineStart;
		string lineText = Document.GetText(lineStart, lineLength);
		Document.Insert(lineEnd, Environment.NewLine + lineText);
	}

	/// <summary>
	/// Moves the current line up.
	/// </summary>
	public void MoveLineUp() => MoveLineHelper(-1);

	/// <summary>
	/// Moves the current line down.
	/// </summary>
	public void MoveLineDown() => MoveLineHelper(1);

	/// <summary>
	/// Moves the current line up or down based on its argument.
	/// </summary>
	/// <param name="direction">The direction in which to move the line, -1 for up, 1 for down.</param>
	/// <remarks>Only 1 and -1 are accepted arguments.</remarks>
	private void MoveLineHelper(int direction)
	{
		int lineIndex = Document.GetLineByOffset(CaretOffset).LineNumber - 1;
		if ((lineIndex <= 0 && direction < 0) || (lineIndex == Document.LineCount - 1 && direction > 0)) return;

		int caretLine = Document.GetLineByOffset(CaretOffset).LineNumber;
		int caretChar = CaretOffset - Document.GetLineByNumber(caretLine).Offset;

		string[] lines = Text.Split(Environment.NewLine);
		(lines[lineIndex], lines[lineIndex + direction]) = (lines[lineIndex + direction], lines[lineIndex]);

		Document.BeginUpdate();
		Document.Text = string.Join(Environment.NewLine, lines);
		CaretOffset = Document.GetLineByNumber(caretLine + direction).Offset + caretChar;
		Document.EndUpdate();
	}

	#endregion

	#region Insert Functions

	/// <summary>
	/// Inserts a new line before the current line.
	/// </summary>
	public void InsertLineBefore() => InsertLineHelper(-1);

	/// <summary>
	/// Inserts a new line after the current line.
	/// </summary>
	public void InsertLineAfter() => InsertLineHelper(1);

	/// <summary>
	/// Inserts a new line before or after the current line based on its argument.
	/// </summary>
	/// <param name="direction">The direction in which to insert the line, -1 for before, and 1 for after.</param>
	/// <remarks>-1 and 1 are the only accepted answers.</remarks>
	private void InsertLineHelper(int direction)
	{
		int caretOffset = CaretOffset;
		DocumentLine currentLine = Document.GetLineByOffset(caretOffset);
		int lineIndex = currentLine.LineNumber;
		int lineEnd = currentLine.EndOffset;
		int lineStart = currentLine.Offset;
		int colIndex = lineEnd - lineStart;

		if (direction == 1)
		{
			Document.Insert(Document.GetOffset(lineIndex, colIndex + 1), "\n");
			if (caretOffset == lineEnd) CaretOffset -= 1;
			DocumentLine nextLine = Document.GetLineByOffset(CaretOffset).NextLine;
			if (nextLine != null) CaretOffset = nextLine.Offset;
		}
		else if (direction == -1)
		{
			Document.Insert(Document.GetOffset(lineIndex, 0), "\n");
			DocumentLine previousLine = Document.GetLineByOffset(CaretOffset).PreviousLine;
			if (previousLine != null) CaretOffset = previousLine.Offset;
		}
	}

	/// <summary>
	/// Inserts text where the caret currently is.
	/// </summary>
	/// <param name="text">Text to insert.</param>
	public void Insert(string text) => Document.Insert(CaretOffset, text);

	/// <summary>
	/// Inserts text at line starts.
	/// </summary>
	/// <param name="insert">Text to insert.</param>
	public void InsertTextAtLineStarts(string insert) => ApplyInsertionToSelected(FormatUtility.InsertTextAtLineStarts, insert);

	/// <summary>
	/// Inserts text at line ends.
	/// </summary>
	/// <param name="insert">Text to insert.</param>
	public void InsertTextAtLineEnds(string insert) => ApplyInsertionToSelected(FormatUtility.InsertTextAtLineEnds, insert);

	#endregion

	private void OnPreviewDragOver(object sender, DragEventArgs e) => e.Handled = true;
}