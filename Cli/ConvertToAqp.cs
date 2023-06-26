using AquaModelLibrary.Extra;
using LegacyObj;
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
	internal static class ConvertToAqp
	{
		public static Command GetCommand()
		{
			var inputArg = new Argument<FileInfo>(name: "file", description: "Model to convert")
				.ExistingOnly();

			var destArg = new Argument<FileInfo>(name: "dest", description: "Converted .aqp file (<file>.aqp)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var aqnArg = new Option<FileInfo>(new string[] { "--skeleton", "-s" }, description: "Converted .aqn file [default: <dest>.aqn]");

			var updateAqnOption = new Option<bool>(new string[] { "--update-aqn", "-u" }, description: "Overwrite existing .aqn file if --skeleton not specified");

			var command = new Command(name: "aqp", description: "Convert models to PSO2 AQP format. Supported formats: .fbx")
			{
				inputArg,
				destArg,
				aqnArg,
				updateAqnOption,
			};

			command.SetHandler(Convert, inputArg, destArg, aqnArg, updateAqnOption);

			return command;
		}

		private static void Convert(FileInfo inputFile, FileInfo destFile, FileInfo aqnFile, bool updateAqn)
		{
			destFile = destFile ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, ".aqp"));

			if (aqnFile == null)
			{
				var file = new FileInfo(Path.ChangeExtension(destFile.FullName, ".aqn"));
				if (updateAqn || !file.Exists)
				{
					aqnFile = file;
				}
			}

			var format = Path.GetExtension(inputFile.FullName).ToLower();
			switch (format)
			{
				case ".fbx":
					Directory.CreateDirectory(destFile.DirectoryName);
					Fbx.ConvertToAqua(fbxFile: inputFile, modelFile: destFile, skeletonFile: aqnFile);
					break;

				default:
					throw new ArgumentException($"Unsupported format: {format}");
			}

		}
	}
}
