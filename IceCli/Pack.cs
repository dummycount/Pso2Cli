using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zamboni.IceFileFormats;

namespace Pso2Cli
{
	internal class Pack
	{
		private static readonly List<string> ArchiveFolderSuffixes = new List<string>
		{
			".extracted",
			"_ext",
		};

		private static readonly List<string> DefaultGroup1Patterns = new List<string>
		{
			"*.acb",
			"*.bti",
			"*.cml",
			"*.cmp",
			"*.crbp",
			"*.crc",
			"*.dds",
			"*.emi",
			"*.evt",
			"*.fcl",
			"*.fig",
			"*.flte",
			"*.ikn",
			"*.ini",
			"*.lat",
			"*.light",
			"*.lua",
			"*.lve",
			"*.mus",
			"*.pha",
			"*.phi",
			"*.pgd",
			"*.prm",
			"*.rgn",
			"*.sib",
			"*.skit",
			"*.snd",
			"*.trm",
			"*.trn",
			"*.txl",
			"*.wdsn",
			"*.wtr",
			"oxyresource.crc",
		};

		public static Command GetCommand()
		{
			var directoryArg = new Argument<DirectoryInfo>(name: "folder", description: "The folder to archive")
				.ExistingOnly();

			var destArg = new Argument<FileInfo>(name: "dest", description: "The file to create")
			{
				Arity = ArgumentArity.ZeroOrOne,
			};

			var group1Option = new Option<FileInfo>(new string[] { "--group1", "-g" }, description: "File containing list of glob patterns matching group1 files")
				.ExistingOnly();

			var compressOption = new Option<bool>(new string[] { "--compress", "-c" }, description: "Compress archive");

			var unencryptedOption = new Option<bool>(new string[] { "--unencrypted", "-u" }, description: "Do not encrypt archive");

			var command = new Command("pack", "Create an ICE archive from a folder of files")
			{
				directoryArg,
				destArg,
				group1Option,
				compressOption,
				unencryptedOption,
			};

			command.SetHandler(PackArchive, directoryArg, destArg, group1Option, compressOption, unencryptedOption);

			return command;
		}

		private static void PackArchive(DirectoryInfo folder, FileInfo dest, FileInfo group1Patterns = null, bool compress = false, bool unencrypted = false)
		{
			folder = folder ?? throw new ArgumentNullException(nameof(folder));
			dest = dest ?? new FileInfo(Path.Combine(folder.Parent.FullName, GetDefaultFileName(folder)));

			var (group1Files, group2Files) = ReadFiles(folder, ReadGroup1PatternsFile(group1Patterns));

			var archive = new IceV4File(GetHeader(), group1Files, group2Files);
			File.WriteAllBytes(dest.FullName, archive.getRawData(compress, unencrypted));
		}

		private static byte[] GetHeader()
		{
			return (new IceHeaderStructures.IceArchiveHeader()).GetBytes();
		}

		private static List<string> ReadGroup1PatternsFile(FileInfo patternsFile)
		{
			if (patternsFile == null)
			{
				return DefaultGroup1Patterns;
			}

			var result = new List<string>();

			using (var stream = patternsFile.OpenText())
			{
				string line;
				while ((line = stream.ReadLine()) != null) {
					line = line.Trim();
					if (line != "" && !line.StartsWith("//") && !line.StartsWith(";") && !line.StartsWith("#"))
					{
						result.Add(line);
					}
				}
			}

			return result;
		}

		private static string GetDefaultFileName(DirectoryInfo folder)
		{
			var name = folder.Name;
			var suffix = ArchiveFolderSuffixes.Find(s => name.EndsWith(s));
			if (suffix != null)
			{
				return name.RemoveSuffix(suffix);
			}

			return name + ".ice";
		}

		private static (byte[][], byte[][]) ReadFiles(DirectoryInfo folder, List<string> group1Patterns)
		{
			var group1Files = new List<byte[]>();
			var group2Files = new List<byte[]>();

			foreach (var file in folder.EnumerateFiles("*.*", SearchOption.AllDirectories))
			{
				var data = GetFileData(file);

				if (IsGroup1(file, folder, group1Patterns))
				{
					group1Files.Add(data);
				}
				else
				{
					group2Files.Add(data);
				}
			}

			return (group1Files.ToArray(), group2Files.ToArray());
		}

		private static byte[] GetFileData(FileInfo file)
		{
			var data = File.ReadAllBytes(file.FullName);
			var header = (new IceHeaderStructures.IceFileHeader(file.FullName, (uint)data.Length)).GetBytes();

			return header.Concat(data).ToArray();
		}

		private static bool IsGroup1(FileInfo file, DirectoryInfo folder, List<string> group1Patterns)
		{
			var subdirs = GetRelativePath(file, folder).Split('/', '\\');

			if (subdirs.Contains("group1", StringComparer.OrdinalIgnoreCase))
			{
				return true;
			}

			if (subdirs.Contains("group2", StringComparer.OrdinalIgnoreCase))
			{
				return false;
			}


			return group1Patterns.Any(pattern => file.Name.Like(pattern));
		}

		private static string GetRelativePath(FileInfo file, DirectoryInfo folder)
		{
			var filename = file.FullName;
			var dirname = folder.FullName;

			if (filename.StartsWith(dirname))
			{
				return filename.Substring(dirname.Length + 1);
			}

			return filename;
		}
	}
}
