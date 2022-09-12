using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.CommandLine;
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
			var fbxArg = new Argument<FileInfo>(name: "fbxfile", description: ".fbx model to convert")
				.ExistingOnly();

			var aqpArg = new Argument<FileInfo>(name: "aqpfile", description: "Converted .aqp file (<fbxfile>.aqp)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var aqnArg = new Option<FileInfo>(new string[] { "--skeleton", "-s" }, description: "Converted .aqn file (<dest>.aqn)");

			var updateAqnOption = new Option<bool>(new string[] { "--update-aqn", "-u" }, description: "Overwrite existing .aqn file if --skeleton not specified");

			var rootCommand = new RootCommand("Convert FBX models to PSO2 AQP format")
			{
				fbxArg,
				aqpArg,
				aqnArg,
				updateAqnOption,
			};

			rootCommand.SetHandler((fbxFile, aqpFile, aqnFile, updateAqn) =>
			{
				aqpFile = aqpFile ?? new FileInfo(Path.ChangeExtension(fbxFile.FullName, ".aqp"));
				
				if (aqnFile == null)
				{
					var file = new FileInfo(Path.ChangeExtension(aqpFile.FullName, ".aqn"));
					if (updateAqn || !file.Exists)
					{
						aqnFile = file;
					} 
				}

				Directory.CreateDirectory(aqpFile.DirectoryName);

				Fbx.ConvertToAqua(fbxFile: fbxFile, modelFile: aqpFile, skeletonFile: aqnFile);

			}, fbxArg, aqpArg, aqnArg, updateAqnOption);

			var parser = Utility.GetParser(rootCommand);
			return parser.Invoke(args);
		}
	}
}
