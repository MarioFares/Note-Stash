using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NoteStash.Models;

/// <summary>
/// Model for a file stash in app and directory on disk.
/// </summary>
public class Stash
{
	/// <summary>
	/// Returns a new stash pointing to the path argument and creates the directory if it doesn't exist.
	/// </summary>
	/// <param name="relativeSourcePath">The relative source to the stash directory.</param>
	public Stash(string relativeSourcePath)
	{
		SourcePath = relativeSourcePath;
		Directory.CreateDirectory(SourcePath);
		RefreshStash();
	}

	private string _sourcePath;

	/// <summary>
	/// Sets the relative or full path for the source directory of the stash and gets the full path.
	/// </summary>
	/// <remarks>Full path is always returned.</remarks>
	public string SourcePath
	{
		get => Path.GetFullPath(_sourcePath);
		set => _sourcePath = value;
	}

	/// <summary>
	/// Gets the last file that was added to <see cref="StashedFiles"/> using the <see cref="StashFileAsync"/>
	/// function.
	/// </summary>
	public string? LastStashedFile { get; private set; }

	/// <summary>
	/// Gets a <see cref="HashSet{T}"/> of stashed files in <see cref="SourcePath"/>.
	/// </summary>
	public HashSet<string> StashedFiles { get; private set; }

	/// <summary>
	/// Updates <see cref="StashedFiles"/> by getting the file names in <see cref="SourcePath"/>.
	/// </summary>
	private void RefreshStash()
	{
		StashedFiles = Directory.GetFiles(SourcePath).Select(Path.GetFileName).ToHashSet()!;
	}

	/// <summary>
	/// Refreshes <see cref="StashedFiles"/> asynchronously by getting the file names in <see cref="SourcePath"/>.
	/// </summary>
	public async Task RefreshStashAsync() => await Task.Run(RefreshStash)!;

	/// <summary>
	/// Creates or writes to a file in <see cref="SourcePath"/> asynchronously as specified by the arguments.
	/// If no extension is provided, the default extension is .txt.
	/// </summary>
	/// <param name="fileName">The name of the file that is to be written to, along with its extension.</param>
	/// <param name="fileContent">String content to be written to the file.</param>
	/// <returns>
	/// True if the data has been successfully written, false otherwise. The default extension, if
	/// none is provided, is .txt.
	/// </returns>
	public async Task<bool> StashFileAsync(string fileName, string fileContent)
	{
		if (!string.IsNullOrEmpty(fileName) && !StashedFiles.Contains(fileName))
		{
			if (!Path.HasExtension(fileName)) fileName += ".txt";

			await Task.Run(() => File.WriteAllText(SourcePath + fileName, fileContent));
			StashedFiles.Add(fileName);
			LastStashedFile = fileName;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Removes the specified file from <see cref="StashedFiles"/> and from <see cref="SourcePath"/> asynchronously.
	/// </summary>
	/// <param name="fileName">The name of the file to be removed from the stash.</param>
	/// <returns>
	/// True if the file has been successfully removed, false otherwise.
	/// </returns>
	public async Task<bool> UnstashFileAsync(string fileName)
	{
		if (string.IsNullOrEmpty(fileName) || !StashedFiles.Contains(fileName)) return false;

		string path = Path.GetFullPath(SourcePath + fileName);
		await Task.Run(() => File.Delete(path));
		StashedFiles.Remove(fileName);
		return true;
	}
}