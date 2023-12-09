using AquaModelLibrary;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
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
			var aqua = new AquaUtil();
			aqua.ReadModel(inputFile.FullName);

			var info = new ModelInfo(aqua);
			Console.WriteLine(info.ToString());
		}
	}
}
