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
	internal static class ConvertToFbx
	{
		public static Command GetCommand()
		{
			var inputArg = new Argument<FileInfo>(name: "file", description: "Model to convert")
				.ExistingOnly();

			var destArg = new Argument<FileInfo>(name: "dest", description: "Converted .fbx file (<file>.fbx)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var skeletonArg = new Option<FileInfo>(new string[] { "--skeleton", "-s" }, description: ".aqn skeleton for the model [default: <model>.aqn]")
				.ExistingOnly();

			var motionArg = new Option<FileInfo[]>(new string[] { "--motion", "-m" }, description: ".aqm motion files to include")
				.ExistingOnly();

			var metaArg = new Option<bool>("--no-metadata", description: "Do not include metadata in the exported model");

			var command = new Command(name: "fbx", description: "Convert models to FBX format. Supported formats: .aqp")
			{
				inputArg,
				destArg,
				skeletonArg,
				motionArg,
				metaArg,
			};

			command.SetHandler(Convert, inputArg, destArg, skeletonArg, motionArg, metaArg);

			return command;
		}

		private static void Convert(FileInfo inputFile, FileInfo destFile, FileInfo skeletonFile, FileInfo[] motionFiles, bool noMetadata)
		{
			destFile = destFile ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, ".fbx"));
			skeletonFile = skeletonFile ?? new FileInfo(Path.ChangeExtension(inputFile.FullName, ".aqn"));

			var format = Path.GetExtension(inputFile.FullName).ToLower();
			switch (format)
			{
				case ".aqp":
					Directory.CreateDirectory(destFile.DirectoryName);
					Fbx.ConvertFromAqua(
						fbxFile: destFile,
						modelFile: inputFile,
						skeletonFile: skeletonFile,
						motionFiles: motionFiles,
						includeMetadata: !noMetadata);
					break;

				default:
					throw new ArgumentException($"Unsupported format: {format}");
			}

		}
	}
}
