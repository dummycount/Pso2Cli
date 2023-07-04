using AquaModelLibrary;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class InfoCommand
	{
		public static Command GetCommand()
		{
			var inputArg = new Argument<FileInfo>(name: "file", description: "Item to inspect")
				.ExistingOnly();

			var command = new Command(name: "info", description: "")
			{
				inputArg,
			};

			command.SetHandler(PrintInfo, inputArg);

			return command;
		}

		private static void PrintInfo(FileInfo inputFile)
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
