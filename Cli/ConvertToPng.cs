using AquaModelLibrary.Extra;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class ConvertToPng
	{
		public static Command GetCommand()
		{
			var inputArg = new Argument<FileInfo>(name: "file", description: "Image to convert")
				.ExistingOnly();

			var destArg = new Argument<FileInfo>(name: "dest", description: "Converted .png file [default: <file>.png]")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var command = new Command(name: "png", description: "Convert images to PNG format. Supported formats: .dds")
			{
				inputArg,
				destArg,
			};

			command.SetHandler(Convert, inputArg, destArg);

			return command;
		}

		private static void Convert(FileInfo inputFile, FileInfo destFile)
		{
			destFile = destFile ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, ".png"));

			var format = Path.GetExtension(inputFile.FullName).ToLower();
			switch (format)
			{
				case ".dds":
					Directory.CreateDirectory(destFile.DirectoryName);
					Dds.ConvertToPng(inputFile, destFile);
					break;

				default:
					throw new ArgumentException($"Unsupported format: {format}");
			}

		}
	}
}
