using System.CommandLine;
using Zamboni;

namespace Pso2Cli;

internal class IceUnpack
{
	public static Command Command()
	{
		var sourceArg = new Argument<FileInfo>(name: "icefile", description: "File to extract")
			.ExistingOnly();

		var destOption = new Option<DirectoryInfo>(
			aliases: ["--out", "-o"], 
			description: "Output directory [default: <icefile>.extracted]");

		var groupOption = new Option<bool>(
			aliases: ["--groups", "-g"], 
			description: "Use group subdirectories");

		var command = new Command(name: "unpack", description: "Extract ICE archive")
		{
			sourceArg,
			destOption,
			groupOption,
		};

		command.SetHandler(Handler, sourceArg, destOption, groupOption);

		return command;
	}

	private static void Handler(FileInfo source, DirectoryInfo? dest, bool useGroups)
	{
		dest ??= new DirectoryInfo(source.FullName + Ice.ExtractedSuffix);

		var group1 = useGroups ? GetGroupDirectory(dest, "group1") : dest;
		var group2 = useGroups ? GetGroupDirectory(dest, "group2") : dest;

		using var stream = source.OpenRead();
		var archive = IceFile.LoadIceFile(stream);

		var group1Result = UnpackGroup(archive.groupOneFiles, group1);
		var group2Result = UnpackGroup(archive.groupTwoFiles, group2);

		var exportedFiles = Utility.Chain(group1Result, group2Result);

		foreach (var file in exportedFiles)
		{
			Console.WriteLine(Path.GetRelativePath(dest.FullName, file.FullName));
		}
	}

	private static DirectoryInfo GetGroupDirectory(DirectoryInfo dest, string name)
	{
		return new DirectoryInfo(Path.Combine(dest.FullName, name));
	}

	private static IEnumerable<FileInfo> UnpackGroup(byte[][] files, DirectoryInfo dest)
	{
		if (files.Length == 0)
		{
			yield break;
		}

		Directory.CreateDirectory(dest.FullName);

		foreach (var bytes in files)
		{
			var name = IceFile.getFileName(bytes);

			byte[] file;
			if (name == "namelessFile.bin")
			{
				file = bytes;
			}
			else
			{
				var headerSize = BitConverter.ToInt32(bytes, 0xC);
				var length = bytes.Length - headerSize;
				file = new byte[length];
				Array.ConstrainedCopy(bytes, headerSize, file, 0, length);
			}

			var path = Path.Combine(dest.FullName, name);
			File.WriteAllBytes(path, file);
			yield return new FileInfo(path);
		}
	}
}
