using Pso2Cli;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dds2Png
{
	internal class Program
	{
		static int Main(string[] args)
		{
			var ddsArg = new Argument<FileInfo>(name: "dds", description: ".dds image to convert")
				.ExistingOnly();

			var pngArg = new Argument<FileInfo>(name: "png", description: "Converted .png file (<dds>.png)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var rootCommand = new RootCommand("Convert DDS images to PNG format")
			{
				ddsArg,
				pngArg,
			};

			rootCommand.SetHandler((ddsFile, pngFile) =>
			{
				pngFile = pngFile ?? new FileInfo(Path.ChangeExtension(ddsFile.FullName, ".png"));
				Dds.ConvertToPng(ddsFile, pngFile);

			}, ddsArg, pngArg);

			var parser = Utility.GetParser(rootCommand);
			return parser.Invoke(args);
		}
	}
}
