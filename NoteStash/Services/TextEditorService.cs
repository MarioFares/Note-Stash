using TextEditor = NoteStash.Controls.TextEditor;

namespace NoteStash.Services;

/// <summary>
/// Service for interacting with <see cref="TextEditor"/>.
/// </summary>
public class TextEditorService : ITextEditorService
{
	private readonly TextEditor _textEditor;

	public TextEditorService(TextEditor textEditor)
	{
		_textEditor = textEditor;
	}

	/// <inheritdoc />
	public void UpperCase() => _textEditor.UpperCase();

	/// <inheritdoc />
	public void LowerCase() => _textEditor.LowerCase();

	/// <inheritdoc />
	public void SentenceCase() => _textEditor.SentenceCase();

	/// <inheritdoc />
	public void TitleCase() => _textEditor.TitleCase();

	/// <inheritdoc />
	public void InvertCase() => _textEditor.InvertCase();

	/// <inheritdoc />
	public void RandomCase() => _textEditor.RandomCase();

	/// <inheritdoc />
	public void Trim() => _textEditor.Trim();

	/// <inheritdoc />
	public void TrimStart() => _textEditor.TrimStart();

	/// <inheritdoc />
	public void TrimEnd() => _textEditor.TrimEnd();

	/// <inheritdoc />
	public void TrimLines() => _textEditor.TrimLines();

	/// <inheritdoc />
	public void TrimLinesStart() => _textEditor.TrimLinesStart();

	/// <inheritdoc />
	public void TrimLinesEnd() => _textEditor.TrimLinesEnd();

	/// <inheritdoc />
	public void ConvertSpacesToTabs() => _textEditor.ConvertSpacesToTabs();

	/// <inheritdoc />
	public void ConvertLeadingSpacesToTabs() => _textEditor.ConvertLeadingSpacesToTabs();

	/// <inheritdoc />
	public void ConvertTabsToSpaces() => _textEditor.ConvertTabsToSpaces();

	/// <inheritdoc />
	public void ConvertLeadingTabsToSpaces() => _textEditor.ConvertLeadingTabsToSpaces();

	/// <inheritdoc />
	public void SortWordsAsc() => _textEditor.SortWordsAsc();

	/// <inheritdoc />
	public void SortWordsDesc() => _textEditor.SortWordsDesc();

	/// <inheritdoc />
	public void SortLinesAsc() => _textEditor.SortLinesAsc();

	/// <inheritdoc />
	public void SortLinesDesc() => _textEditor.SortLinesDesc();

	/// <inheritdoc />
	public void NormalizeVerticalWhitespace() => _textEditor.NormalizeVerticalWhitespace();

	/// <inheritdoc />
	public void JoinLines() => _textEditor.JoinLines();

	/// <inheritdoc />
	public void NormalizeHorizontalWhitespace() => _textEditor.NormalizeHorizontalWhitespace();

	/// <inheritdoc />
	public void SplitWords() => _textEditor.SplitWords();

	/// <inheritdoc />
	public void SelectCurrentLine() => _textEditor.SelectCurrentLine();

	/// <inheritdoc />
	public void DeleteCurrentLine() => _textEditor.DeleteCurrentLine();

	/// <inheritdoc />
	public void DuplicateCurrentLine() => _textEditor.DuplicateCurrentLine();

	/// <inheritdoc />
	public void MoveLineUp() => _textEditor.MoveLineUp();

	/// <inheritdoc />
	public void MoveLineDown() => _textEditor.MoveLineDown();

	/// <inheritdoc />
	public void InsertLineBefore() => _textEditor.InsertLineBefore();

	/// <inheritdoc />
	public void InsertLineAfter() => _textEditor.InsertLineAfter();

	/// <inheritdoc />
	public void Insert(string text) => _textEditor.Insert(text);

	/// <inheritdoc />
	public void SetSelectedText(string text) => _textEditor.SelectedText = text;

	/// <inheritdoc />
	public void InsertTextAtLineStarts(string insertion) => _textEditor.InsertTextAtLineStarts(insertion);

	/// <inheritdoc />
	public void InsertTextAtLineEnds(string insertion) => _textEditor.InsertTextAtLineEnds(insertion);
}
