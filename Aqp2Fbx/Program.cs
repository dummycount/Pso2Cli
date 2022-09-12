using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal class Program
	{
		static int Main(string[] args)
		{
			var modelArg = new Argument<FileInfo>(name: "aqpfile", description: ".aqp model to convert")
				.ExistingOnly();

			var destArg = new Argument<FileInfo>(name: "fbxfile", description: "Converted .fbx file (<aqpfile>.fbx)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var skeletonArg = new Option<FileInfo>(new string[] { "--skeleton", "-s" }, description: ".aqn skeleton for the model (<model>.aqn)")
				.ExistingOnly();

			var motionArg = new Option<FileInfo[]>(new string[] { "--motion", "-m" }, description: ".aqm motion files to include")
				.ExistingOnly();

			var metaArg = new Option<bool>("--no-metadata", description: "Do not include metadata in the exported model");

			var rootCommand = new RootCommand("Convert PSO2 AQP models to FBX format")
			{
				modelArg,
				destArg,
				skeletonArg,
				motionArg,
				metaArg,
			};

			rootCommand.SetHandler((modelFile, destFile, skeletonFile, motionFiles, noMetadata) =>
			{
				destFile = destFile ?? new FileInfo(Path.ChangeExtension(modelFile.FullName, ".fbx"));
				skeletonFile = skeletonFile ?? new FileInfo(Path.ChangeExtension(modelFile.FullName, ".aqn"));

				Directory.CreateDirectory(destFile.DirectoryName);

				Fbx.ConvertFromAqua(
					fbxFile: destFile,
					modelFile: modelFile,
					skeletonFile: skeletonFile,
					motionFiles: motionFiles,
					includeMetadata: !noMetadata);

			}, modelArg, destArg, skeletonArg, motionArg, metaArg);

			var parser = Utility.GetParser(rootCommand);
			return parser.Invoke(args);
		}
	}
}
