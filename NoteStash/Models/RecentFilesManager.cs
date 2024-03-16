using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoteStash.Models;

/// <summary>
/// Model for managing recent files in the editor.
/// </summary>
public class RecentFilesManager
{
	public RecentFilesManager(int max, List<string> recentFiles)
	{
		Max = max;
		RecentFiles = recentFiles;
	}

	private int _max;

	/// <summary>
	/// Gets or sets the maximum number of files the user wants to store.
	/// The acceptable interval of this number is [1, 30].
	/// Otherwise, the value will default to 1.
	/// </summary>
	public int Max
	{
		get => _max;
		set => _max = (value <= 30 && value >= 1) ? value : 1;
	}

	private List<string> _recentFiles;

	/// <summary>
	/// Gets the list of recent files that behaves as a HashSet that maintains 
	/// insertion order and ensures all items are existing files.
	/// </summary>
	public List<string> RecentFiles
	{
		// Get is called much more than set. Updates may occur without setting
		// so the getter updates _recentFiles accordingly.
		get
		{
			_recentFiles.RemoveAll(x => !File.Exists(x));
			if (_recentFiles.Count > Max) _recentFiles = _recentFiles.Take(Max).ToList();
			return _recentFiles;
		}
		private set => _recentFiles = value ?? new();
	}

	/// <summary>
	/// Inserts the path of the file that has been replaced into <see cref="RecentFiles"/> and checks if
	/// instances of the new file path are already in the list.
	/// </summary>
	/// <param name="oldFilePath">The file path of the document that was previously loaded into the app.</param>
	/// <param name="newFilePath">The file path of the document that is to be loaded into the app.</param>
	public void Insert(string oldFilePath, string newFilePath)
	{
		// Removes all instances of oldFilePath and newFilePath
		RecentFiles.RemoveAll(c => c == newFilePath || c == oldFilePath);

		if (!string.IsNullOrEmpty(oldFilePath) && File.Exists(oldFilePath))
		{
			// Insert at the front = is the most recent
			RecentFiles.Insert(0, oldFilePath);
		}
	}

	public void Clear() => RecentFiles.Clear();
}
