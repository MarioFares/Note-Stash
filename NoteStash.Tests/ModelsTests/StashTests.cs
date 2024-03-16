using NoteStash.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NoteStash.Tests.ModelsTests;

/// <summary>
/// Tests for the <see cref="Stash"/>.
/// </summary>
public class StashTests
{
	#region Constructor

	[Theory]
	[InlineData("stash11")]
	[InlineData("./stash12")]
	[InlineData("./stash13/")]
	[InlineData("../stash14/")]
	public void Constructor_ValidNonexistingPath_DirectoryIsCreated(string input)
	{
		string path = input;
		Assert.False(Directory.Exists(path));

		Stash _ = new(path);

		Assert.True(Directory.Exists(path));
		Directory.Delete(path, true);
	}

	[Fact]
	public void Constructor_NullPath_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => new Stash(null!));

	[Fact]
	public void Constructor_InvalidPath_ThrowsIOException() =>
		Assert.Throws<IOException>(() => new Stash("<>"));

	#endregion

	#region SourcePath

	[Fact]
	public void SourcePath_ValidRelativePath_ReturnsFullPath()
	{
		const string path = "./stash2/";
		Stash stash = new(path);
		string expected = Path.GetFullPath(path);

		string actual = stash.SourcePath;

		Assert.Equal(expected, actual);
		Directory.Delete(path, true);
	}

	#endregion

	#region RefreshStashAsync

	[Fact]
	public async Task RefreshStashAsync_PathWithFiles_ReturnsFiles()
	{
		const string path = "./stash31/";
		Assert.False(Directory.Exists(path));
		Directory.CreateDirectory(path);
		File.Create(path + "file1.txt").Close();
		File.Create(path + "file2.txt").Close();
		Stash stash = new(path);
		await stash.RefreshStashAsync();
		List<string> expected = new()
		{
			"file1.txt",
			"file2.txt"
		};

		HashSet<string> actual = stash.StashedFiles;

		Assert.Equal(expected, actual);
		Directory.Delete(path, true);
	}

	[Fact]
	public async Task RefreshStashAsync_PathWithoutFiles_ReturnsEmptyList()
	{
		const string path = "./stash32/";
		Assert.False(Directory.Exists(path));
		Directory.CreateDirectory(path);
		Stash stash = new(path);
		await stash.RefreshStashAsync();
		HashSet<string> expected = new();

		HashSet<string> actual = stash.StashedFiles;

		Assert.Equal(expected, actual);
		Directory.Delete(path, true);
	}

	[Fact]
	public async Task RefreshStashAsync_DirectoryDoesNotExist_ThrowsDirectoryNotFoundException()
	{
		const string path = "./stash33/";
		Assert.False(Directory.Exists(path));

		Stash stash = new(path);

		Directory.Delete(path);
		await Assert.ThrowsAsync<DirectoryNotFoundException>(() => stash.RefreshStashAsync());
	}

	#endregion

	#region StashFileAsync

	[Fact]
	public async Task StashFileAsync_ValidFileName_CreatesFileAndAddsToStashedFilesList()
	{
		const string path = "./stash41/";
		Assert.False(Directory.Exists(path));
		Stash stash = new(path);

		Assert.False(File.Exists(path + "file1.txt"));
		await stash.StashFileAsync("file1.txt", "hello");
		List<string> expected = new() { "file1.txt" };

		Assert.True(File.Exists(path + "file1.txt"));
		Assert.Equal(expected, stash.StashedFiles);
		Directory.Delete(path, true);
	}

	[Fact]
	public void StashFileAsync_InvalidFileName_ThrowsIOException()
	{
		const string path = "./stash42/";
		Assert.False(Directory.Exists(path));

		Stash stash = new(path);

		Assert.ThrowsAsync<IOException>(() => stash.StashFileAsync("<>.txt", ""));
		Directory.Delete(path, true);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null!)]
	public async void StashFileAsync_NullOrEmptyArgument_ReturnsFalseAndModifiesNothing(string input)
	{
		const string path = "./stash43/";
		Assert.False(Directory.Exists(path));
		Directory.CreateDirectory(path);

		Stash stash = new(path);

		Assert.False(await stash.StashFileAsync(input, ""));
		Directory.Delete(path, true);
	}

	#endregion

	#region UnstashFileAsync

	[Fact]
	public async Task UnstashFileAsync_ValidFileName_DeletesFileAndRemocvesFromStashedFilesList()
	{
		const string path = "./stash6/";
		Assert.False(Directory.Exists(path));
		Stash stash = new(path);

		Assert.False(File.Exists(path + "file1.txt"));
		await stash.StashFileAsync("file1.txt", "hello");
		await stash.UnstashFileAsync("file1.txt");
		HashSet<string> expected = new();

		Assert.False(File.Exists(path + "file1.txt"));
		Assert.Equal(expected, stash.StashedFiles);
		Directory.Delete(path, true);
	}

	#endregion
}