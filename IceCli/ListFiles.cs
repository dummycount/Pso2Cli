using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Zamboni;

namespace Pso2Cli
{
	internal class ListFiles
	{
		public enum Format
		{
			Text,
			Json,
		}

		public static Command GetCommand()
		{
			var fileArg = new Argument<FileInfo>(name: "file", description: "The archive to inspect")
				.ExistingOnly();

			var formatOption = new Option<Format>(new string[] { "--format", "-f" }, description: "Output format");
			formatOption.SetDefaultValue(Format.Text);

			var command = new Command("list", "List the files in an ICE archive")
			{
				fileArg,
				formatOption,
			};

				
			command.SetHandler((file, format) =>
			{
				ListArchiveFiles(file, format);
			}, fileArg, formatOption);

			return command;
		}

		private class FileList
		{
			public List<string> Group1 { get; set; } = new List<string>();
			public List<string> Group2 { get; set; } = new List<string>();
		}

		private static void ListArchiveFiles(FileInfo file, Format format)
		{
			var archive = Archive.LoadIceFile(file);

			var list = new FileList
			{
				Group1 = GetGroupFileNames(archive.groupOneFiles).ToList(),
				Group2 = GetGroupFileNames(archive.groupTwoFiles).ToList(),
			};

			switch (format)
			{
				case Format.Text:
					WriteText(list);
					break;

				case Format.Json:
					WriteJson(list);
					break;
			}
		}

		private static IEnumerable<string> GetGroupFileNames(byte[][] files)
		{
			foreach (var bytes in files)
			{
				yield return IceFile.getFileName(bytes);
			}
		}

		private static void WriteText(FileList list)
		{
			WriteTextGroup("Group 1", list.Group1);
			WriteTextGroup("Group 2", list.Group2);
		}

		private static void WriteTextGroup(string name, List<string> files)
		{
			if (files.Count == 0)
			{
				return;
			}

			Console.WriteLine($"{name} contents:");
			foreach (var file in files)
			{
				Console.Write("  ");
				Console.WriteLine(file);
			}
		}

		private class LowercaseNamingPolicy : JsonNamingPolicy
		{
			public override string ConvertName(string name)
			{
				return name.ToLower();
			}
		}

		private static void WriteJson(FileList list)
		{
			var options = new JsonSerializerOptions()
			{
				WriteIndented = true,
				PropertyNamingPolicy = new LowercaseNamingPolicy(),
			};

			Console.WriteLine(JsonSerializer.Serialize(list, options));
		}
	}
}
