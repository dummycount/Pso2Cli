using AquaModelLibrary.Extra;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class FileLists
	{
		public static Command GetCommand()
		{
			var binDirOption = new Option<DirectoryInfo>(
				new string[] { "--bin", "-b" },
				description: "Path to pso2_bin folder")
				.ExistingOnly();

			var outArg = new Argument<DirectoryInfo>(
				name: "out",
				description: "Output directory",
				getDefaultValue: () => new DirectoryInfo("./FileList"));

			var command = new Command(name: "filelist", description: "Generate file list spreadsheets")
			{
				binDirOption,
				outArg,
			};

			command.SetHandler(GenerateSheets, binDirOption, outArg);

			return command;
		}

		private static void GenerateSheets(DirectoryInfo binDir, DirectoryInfo outDir)
		{
			outDir = outDir ?? throw new ArgumentNullException(nameof(outDir));

			binDir = binDir ?? Pso2Path.GetPso2BinDirectory();
			if (binDir == null)
			{
				throw new Exception("Couldn't find PSO2 data directory");
			}

			Directory.CreateDirectory(outDir.FullName);
			ReferenceGenerator.OutputFileLists(binDir.FullName, outDir.FullName);
		}
	}
}
