using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zamboni;

namespace Pso2Cli
{
	internal static class IceList
	{
		public static Command Command()
		{
			var fileArg = new Argument<FileInfo>(name: "icefile", description: "Directory to pack")
				.ExistingOnly();

			var command = new Command(name: "pack", description: "Pack files into an ICE archive")
			{
				fileArg
			};

			command.SetHandler(Handler, fileArg);

			return command;
		}

		private static void Handler(FileInfo file)
		{
			using var stream = file.OpenRead();
			var archive = IceFile.LoadIceFile(stream);

			foreach (var dataFile in GetGroupFileNames(archive.groupOneFiles))
			{
				Console.WriteLine($"group1/{dataFile}");
			}

			foreach (var dataFile in GetGroupFileNames(archive.groupTwoFiles))
			{
				Console.WriteLine($"group2/{dataFile}");
			}
		}

		private static IEnumerable<string> GetGroupFileNames(byte[][] files)
		{
			foreach (var bytes in files)
			{
				yield return IceFile.getFileName(bytes);
			}
		}
	}
}
