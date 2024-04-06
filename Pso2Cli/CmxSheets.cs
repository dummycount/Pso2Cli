using AquaModelLibrary.Data.Utility;
using System.CommandLine;

namespace Pso2Cli;

internal static class CmxSheets
{
	public static Command Command()
	{
		var binDirOption = Utility.GetPso2BinDirectoryOption(); ;

		var outArg = new Argument<DirectoryInfo>(
			name: "out",
			description: "Output directory",
			getDefaultValue: () => new DirectoryInfo("./FileList"));

		var command = new Command(name: "sheets", description: "Generate file list spreadsheets")
		{
			binDirOption,
			outArg,
		};

		command.SetHandler(Handler, binDirOption, outArg);

		return command;
	}

	private static void Handler(DirectoryInfo? binDir, DirectoryInfo outDir)
	{
		binDir ??= Utility.GetPso2BinDirectory();

		Directory.CreateDirectory(outDir.FullName);

		unsafe
		{
			ReferenceGenerator.OutputFileLists(binDir.FullName, outDir.FullName);
		}
	}
}
