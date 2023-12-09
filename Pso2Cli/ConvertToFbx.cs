using AquaModelLibrary;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	static internal class ConvertToFbx
	{
		public static Command Command()
		{
			var sourceArg = new Argument<FileInfo>(name: "file", description: "Model to convert")
				.ExistingOnly();

			var destArg = new Argument<FileInfo>(name: "dest", description: "Converted file (<file>.fbx)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var skeletonArg = new Option<FileInfo>(
				aliases: ["--skeleton", "-s"], 
				description: ".aqn skeleton for the model [default: <model>.aqn]")
				.ExistingOnly();

			var motionArg = new Option<FileInfo[]>(
				aliases: ["--motion", "-m"], 
				description: ".aqm motion files to include")
				.ExistingOnly();

			var metaArg = new Option<bool>(
				name: "--no-metadata", 
				description: "Do not include metadata in the exported model");

			var infoArg = new Option<bool>(
				aliases: ["--info", "-i"], 
				description: "Print model information (JSON format)");

			var command = new Command(name: "fbx", description: "Convert models to FBX")
			{
				sourceArg,
				destArg, 
				skeletonArg, 
				motionArg,
				metaArg,
				infoArg,
			};

			command.SetHandler(Handler, sourceArg, destArg, skeletonArg, motionArg, metaArg, infoArg);

			return command;
		}

		private static void Handler(FileInfo source, FileInfo? dest, FileInfo? skeleton, FileInfo[] motion, bool noMetadata, bool printInfo)
		{
			dest ??= new FileInfo(Path.ChangeExtension(source.FullName, ".fbx"));
			skeleton ??= new FileInfo(Path.ChangeExtension(source.FullName, ".aqn"));

			var format = Path.GetExtension(source.FullName).ToLower();
			switch (format)
			{
				case ".aqp":
					var aqua = ConvertFromAqp(
						source: source,
						dest: dest,
						skeleton: skeleton,
						motion: motion,
						includeMetadata: !noMetadata);

					if (printInfo)
					{
						PrintModelInfo(aqua);
					}
					break;

				default:
					throw new ArgumentException($"Unsupported format: {format}");
			}
		}

		private static void PrintModelInfo(AquaUtil aqua)
		{
			throw new NotImplementedException();
		}

		private static AquaUtil ConvertFromAqp(FileInfo source, FileInfo dest, FileInfo skeleton, FileInfo[] motion, bool includeMetadata)
		{
			Directory.CreateDirectory(dest.DirectoryName!);

			return Fbx.ConvertFromAqua(
				aqpFile: source,
				fbxFile: dest,
				skeletonFile: skeleton,
				motionFiles: motion,
				includeMetadata: includeMetadata);
		}
	}
}
