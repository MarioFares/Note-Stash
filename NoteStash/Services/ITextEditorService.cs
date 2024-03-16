namespace NoteStash.Services;

/// <summary>
/// Interface for interacting with a text editor.
/// </summary>
public interface ITextEditorService
{
	/// <summary>
	/// Changes the selected text to upper case.
	/// </summary>
	public void UpperCase();

	/// <summary>
	/// Changes the selected text to lower case.
	/// </summary>
	public void LowerCase();

	/// <summary>
	/// Changes the selected text to sentence case.
	/// </summary>
	public void SentenceCase();

	/// <summary>
	/// Changes the selected text to title case.
	/// </summary>
	public void TitleCase();

	/// <summary>
	/// Changes the selected text to invert case.
	/// </summary>
	public void InvertCase();

	/// <summary>
	/// Changes the selected text to random case.
	/// </summary>
	public void RandomCase();

	/// <summary>
	/// Trims the selected text.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void Trim();

	/// <summary>
	/// Trims the start of the selected text.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void TrimStart();

	/// <summary>
	/// Trims the end of the selected text.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void TrimEnd();

	/// <summary>
	/// Trims the lines of the selected text.
	/// </summary>
	/// <remarks>For multiple lines</remarks>
	public void TrimLines();

	/// <summary>
	/// Trims the starts of the lines of the selected text.
	/// </summary>
	/// <remarks>For multiple lines</remarks>
	public void TrimLinesStart();

	/// <summary>
	/// Trims the ends of the lines of the selected text.
	/// </summary>
	/// <remarks>For multiple lines</remarks>
	public void TrimLinesEnd();

	/// <summary>
	/// Converts spaces to tabs in the selected text or the whole 
	/// text if nothing is selected.
	/// </summary>
	public void ConvertSpacesToTabs();

	/// <summary>
	/// Converts leading spaces to tabs in the selected text or the 
	/// whole text if nothing is selected.
	/// </summary>
	public void ConvertLeadingSpacesToTabs();

	/// <summary>
	/// Converts tabs to spaces in the selected text or the whole 
	/// text if nothing is selected.
	/// </summary>
	public void ConvertTabsToSpaces();

	/// <summary>
	/// Converts leading tabs to spaces in the selected text or the whole 
	/// text if nothing is selected.
	/// </summary>
	public void ConvertLeadingTabsToSpaces();

	/// <summary>
	/// Sorts the words of the selected text in ascending order.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void SortWordsAsc();

	/// <summary>
	/// Sorts the words of the selected text in descending order.
	/// </summary>
	/// <remarks>For single lines.</remarks>
	public void SortWordsDesc();

	/// <summary>
	/// Sorts the lines of the selected text in ascending order.
	/// </summary>
	/// <remarks>For multiple lines.</remarks>
	public void SortLinesAsc();

	/// <summary>
	/// Sorts the lines of the selected text in descending order.
	/// </summary>
	/// <remarks>For multiple lines.</remarks>
	public void SortLinesDesc();

	/// <summary>
	/// Selects the entire line at the current location of the caret.
	/// </summary>
	public void SelectCurrentLine();

	/// <summary>
	/// Normalizes vertical whitespace such that if there is more than a 
	/// single empty line separating two paragraphs, they are reduced to 
	/// one empty line.
	/// </summary>
	public void NormalizeVerticalWhitespace();

	/// <summary>
	/// Joins lines together by normalizing them
	/// and replacing new line characters with a single space.
	/// </summary>
	public void JoinLines();

	/// <summary>
	/// Normalizes horizontal whitespace such that if there is more than a single space character or 
	/// tab character between two words, they are replaced by a single space character.
	/// </summary>
	public void NormalizeHorizontalWhitespace();

	/// <summary>
	/// Splits words on every space character after normalizing horizontal whitespace.
	/// </summary>
	public void SplitWords();

	/// <summary>
	/// Deletes the entire line at the current location of the caret.
	/// </summary>
	public void DeleteCurrentLine();

	/// <summary>
	/// Duplicates the entire line at the current location of the caret.
	/// </summary>
	public void DuplicateCurrentLine();

	/// <summary>
	/// Moves the current line up.
	/// </summary>
	public void MoveLineUp();

	/// <summary>
	/// Moves the current line down.
	/// </summary>
	public void MoveLineDown();

	/// <summary>
	/// Inserts a new line before the current line.
	/// </summary>
	public void InsertLineBefore();

	/// <summary>
	/// Inserts a new line after the current line.
	/// </summary>
	public void InsertLineAfter();

	/// <summary>
	/// Inserts text where the caret currently is.
	/// </summary>
	/// <param name="text">Text to insert.</param>
	public void Insert(string text);

	/// <summary>
	/// Sets the currently selected text.
	/// </summary>
	/// <param name="text">Text to select.</param>
	public void SetSelectedText(string text);

	/// <summary>
	/// Inserts the second argument string at the beginning of 
	/// the lines of the first argument.
	/// </summary>
	/// <param name="insertion">Text to insert.</param>
	public void InsertTextAtLineStarts(string insertion);

	/// <summary>
	/// Inserts the second argument string at the end of 
	/// the lines of the first argument.
	/// </summary>
	/// <param name="insertion">Text to insert.</param>
	public void InsertTextAtLineEnds(string insertion);
}
