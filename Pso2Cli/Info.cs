using AquaModelLibrary.Data.PSO2.Aqua;
using System.CommandLine;

namespace Pso2Cli;

internal static class Info
{
	public static Command Command()
	{
		var fileArg = new Argument<FileInfo>(name: "file", description: "Item to inspect")
			.ExistingOnly();

		var command = new Command(name: "info", description: "Print file information")
		{
			fileArg,
		};

		command.SetHandler(Handler, fileArg);

		return command;
	}

	private static void Handler(FileInfo inputFile)
	{
		var format = Path.GetExtension(inputFile.FullName).ToLower();
		switch (format)
		{
			case ".aqp":
				PrintAqpInfo(inputFile);
				break;

			default:
				throw new ArgumentException($"Unsupported format: {format}");
		}
	}

	private static void PrintAqpInfo(FileInfo inputFile)
	{
		var package = new AquaPackage(File.ReadAllBytes(inputFile.FullName));

		var info = new ModelInfo(package);
		Console.WriteLine(info.ToString());
	}
}
