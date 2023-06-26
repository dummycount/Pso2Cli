using Pso2Cli;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cli
{
	internal class Program
	{
		static int Main(string[] args)
		{
			var rootCommand = new RootCommand("PSO2 utilities");

			rootCommand.AddCommand(ConvertToAqp.GetCommand());
			rootCommand.AddCommand(ConvertToFbx.GetCommand());
			rootCommand.AddCommand(ConvertToPng.GetCommand());
			rootCommand.AddCommand(FileLists.GetCommand());

			var parser = Utility.GetParser(rootCommand);
			return parser.Invoke(args);
		}
	}
}
