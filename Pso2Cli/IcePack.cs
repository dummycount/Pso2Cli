using System.CommandLine;
using Zamboni.IceFileFormats;

namespace Pso2Cli;

internal static class IcePack
{
	public static Command Command()
	{
		var folderArg = new Argument<DirectoryInfo>(name: "directory", description: "Directory to pack")
			.ExistingOnly();

		var destOption = new Option<FileInfo>(
			aliases: ["--out", "-o"],
			description: "Output file");

		var group1Option = new Option<string[]>(
			aliases: ["--group1", "-1"],
			description: "File extensions and/or file names to include in group 1, e.g. \".acb,.snd\"")
		{
			Arity = ArgumentArity.ZeroOrMore
		};

		var ignoreOption = new Option<string[]>(
			aliases: ["--ignore", "-i"],
			description: "File extensions and/or file names to ignore, e.g. \".mp4,.png\"")
		{
			Arity = ArgumentArity.ZeroOrMore
		};

		var compressOption = new Option<bool>(
			aliases: ["--compress", "c"],
			description: "Compress archive");

		var encryptOption = new Option<bool>(
			aliases: ["--encrypt", "-e"],
			description: "Encrypt archive");

		var command = new Command(name: "pack", description: "Pack files into an ICE archive")
		{
			folderArg,
			destOption,
			group1Option,
			ignoreOption,
			compressOption,
			encryptOption,
		};

		command.SetHandler(Handler, folderArg, destOption, group1Option, ignoreOption, compressOption, encryptOption);

		return command;
	}

	private static async Task Handler(DirectoryInfo folder, FileInfo dest, string[]? group1Files, string[]? ignoreFiles, bool compress, bool encrypt)
	{
		var group1Patterns = ParseFilePatterns(group1Files ?? []);
		var ignorePatterns = ParseFilePatterns(ignoreFiles ?? []);
		dest ??= new FileInfo(folder.FullName.RemoveSuffix(Ice.ExtractedSuffix));

		if (dest.FullName == folder.FullName)
		{
			dest = new FileInfo(dest.FullName + ".ice");
		}

		var (group1, group2) = GetInputFiles(folder, group1Patterns: group1Patterns, ignorePatterns: ignorePatterns);
		var group1Data = GetFileData(group1);
		var group2Data = GetFileData(group2);

		var header = new IceHeaderStructures.IceArchiveHeader();
		var archive = new IceV4File(header.GetBytes(), await group1Data, await group2Data);

		await File.WriteAllBytesAsync(dest.FullName, archive.getRawData(compress: compress, forceUnencrypted: !encrypt));
	}

	private enum Group
	{
		One,
		Two
	}

	private static (IEnumerable<FileInfo> group1, IEnumerable<FileInfo> group2) GetInputFiles(DirectoryInfo folder, IEnumerable<string> group1Patterns, IEnumerable<string> ignorePatterns) 
	{
		var group1 = new List<FileInfo>();
		var group2 = new List<FileInfo>();

		foreach (var file in folder.EnumerateFiles("*.*", SearchOption.AllDirectories))
		{
			if (IsPatternMatch(file, ignorePatterns))
			{
				continue;
			}

			switch (GetFileGroup(file, folder, group1Patterns))
			{
				case Group.One: group1.Add(file); break;
				case Group.Two: group2.Add(file); break;
				default: throw new NotImplementedException("Unhandled group");
			}
		}

		return (group1, group2);
	}

	private static IEnumerable<string> ParseFilePatterns(string[] args)
	{
		foreach (var arg in args)
		{
			foreach (var pattern in arg.Split(","))
			{
				yield return pattern.Trim();
			}
		}
	}

	private static bool IsPatternMatch(FileInfo file, IEnumerable<string> patterns)
	{
		return patterns.Any(pattern => file.Name == pattern || file.Extension == pattern);
	}

	private static Group GetFileGroup(FileInfo file, DirectoryInfo folder, IEnumerable<string> group1Patterns)
	{
		var subdirs = Path.GetRelativePath(folder.FullName, file.FullName).Split('/', '\\');

		if (subdirs.Contains("group1", StringComparer.OrdinalIgnoreCase))
		{
			return Group.One;
		}

		if (subdirs.Contains("group2", StringComparer.OrdinalIgnoreCase))
		{
			return Group.Two;
		}

		return IsPatternMatch(file, group1Patterns) ? Group.One : Group.Two;
	}

	private static async Task<byte[]> GetFileData(FileInfo file)
	{
		var data = await File.ReadAllBytesAsync(file.FullName);
		var header = new IceHeaderStructures.IceFileHeader(file.FullName, (uint)data.Length).GetBytes();

		return header.Concat(data).ToArray();
	}

	private static async Task<byte[][]> GetFileData(IEnumerable<FileInfo> files)
	{
		return await Task.WhenAll(files.Select(GetFileData));
	}
}
