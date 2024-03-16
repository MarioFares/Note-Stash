using System;
using System.IO;
using System.Threading.Tasks;

namespace NoteStash.Models;

/// <summary>
/// Model for a text document.
/// </summary>
public class TextDocument
{
	private FileInfo? _fileInfo;

	/// <summary>
	/// Returns a new and empty document.
	/// </summary>
	public TextDocument()
	{
		FilePath = string.Empty;
		Text = string.Empty;
		NewText = string.Empty;
		InitialText = string.Empty;
	}

	/// <summary>
	/// Gets text that was in the file upon initial load.
	/// </summary>
	/// <remarks>
	/// Does not reflect the actual text currently in the document. For that, see <see cref="Text"/>.
	/// </remarks>
	public string InitialText { get; private set; }

	/// <summary>
	/// Gets persistent string data found in the file as indicated by <see cref="FilePath"/>.
	/// </summary>
	/// <remarks>Can be set only through <see cref="SaveAsync"/>.</remarks>
	public string Text { get; private set; }

	/// <summary>
	/// Gets or sets the new text string data that is not persistent. This property is used along with
	/// <see cref="Text"/> to determine whether there are changes to save to the document.
	/// </summary>
	public string NewText { get; set; }

	/// <summary>
	/// Gets the name of the file along with its extension. Uses <see cref="FilePath"/> to supply the file's name.
	/// Uses <see cref="Path.GetFileName(string?)"/>.
	/// </summary>
	public string FileName => Path.GetFileName(FilePath);

	private string _filePath;

	/// <summary>
	/// Gets or sets the full path of the file. Includes everything up to the file name and its extension.
	/// </summary>
	public string FilePath
	{
		get => _filePath;
		set
		{
			_filePath = value;
			if (File.Exists(value))
			{
				_fileInfo = new FileInfo(value);
			}
		}
	}

	/// <summary>
	/// Gets the size of the file specified in <see cref="FilePath"/> using <see cref="FileInfo"/>. The unit is
	/// bytes.
	/// </summary>
	public long Size => _fileInfo?.Length ?? 0;

	/// <summary>
	/// Gets the creation time of the file specified in <see cref="FilePath"/> using <see cref="FileInfo"/>.
	/// </summary>
	public DateTime DateCreated => _fileInfo?.CreationTime ?? default;

	/// <summary>
	/// Gets the last modification time of the file specified in <see cref="FilePath"/> using <see cref="FileInfo"/>
	/// </summary>
	public DateTime DateModified => _fileInfo?.LastWriteTime ?? default;

	/// <summary>
	/// Gets the last access time of the file specified in <see cref="FilePath"/> using <see cref="FileInfo"/>
	/// </summary>
	public DateTime DateAccessed => _fileInfo?.LastAccessTime ?? default;

	/// <summary>
	/// Gets whether the document is empty, namely whether a path has been specified.
	/// </summary>
	public bool IsEmpty => string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(FilePath);

	/// <summary>
	/// Gets whether the document has been modified, or in other words, has changes that have not been
	/// flushed to disk.
	/// </summary>
	public bool IsModified => Text != NewText;

	/// <summary>
	/// Flushes any new text to disk asynchronously, making changes persistent. If the path of the file is
	/// valid but the file does not exist, the function creates the file and writes <see cref="NewText"/> to it.
	/// Fails if the document is empty.
	/// </summary>
	/// <returns>
	/// Whether changes have been persisted, namely, if the document is empty, the function
	/// will return false.
	/// </returns>
	/// <remarks>Creates the file if it does not exist.</remarks>
	public async Task<bool> SaveAsync()
	{
		if (IsEmpty) return false;

		await Task.Run(() => File.WriteAllText(FilePath, NewText));
		Text = NewText;

		// The file might just have been created when WriteAllText is called
		_fileInfo ??= new FileInfo(FilePath);
		return true;
	}

	/// <summary>
	/// Resets the documents properties to empty fields.
	/// </summary>
	public void New()
	{
		_fileInfo = null;
		Text = string.Empty;
		NewText = string.Empty;
		FilePath = string.Empty;
	}

	/// <summary>
	/// Loads text asynchronously from the <see cref="FilePath"/> into <see cref="Text"/> only.
	/// </summary>
	/// <remarks>
	/// This function is used since a path supplied does not have to exist, it may be created on save.
	/// </remarks>
	public async Task LoadAsync()
	{
		Text = await Task.Run(() => File.ReadAllText(FilePath));
		InitialText = Text;
	}

	/// <summary>
	/// Renames the current document in the filesystem to newName.
	/// </summary>
	/// <returns>
	/// Whether the file has been successfully renamed.
	/// </returns>
	/// <param name="newName">A string indicating the new desired name without the extension provided.</param>
	/// <remarks>
	/// It is critical that an extension not be provided. It is automatically determined using
	/// <see cref="FilePath"/>
	/// </remarks>
	public bool Rename(string newName)
	{
		if (string.IsNullOrEmpty(newName) || IsEmpty) return false;

		newName += Path.GetExtension(FilePath);
		string? dirPath = Path.GetDirectoryName(FilePath);
		if (dirPath == null) return false;

		string newPath = Path.Combine(dirPath, newName);
		File.Move(FilePath, newPath);
		FilePath = newPath;
		return true;
	}
}