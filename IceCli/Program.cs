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
			var rootCommand = new RootCommand("Edit PSO2 ICE archives");

			rootCommand.AddCommand(ListFiles.GetCommand());
			rootCommand.AddCommand(Pack.GetCommand());
			rootCommand.AddCommand(Unpack.GetCommand());

			var parser = Utility.GetParser(rootCommand);
			return parser.Invoke(args);
		}
	}
}
