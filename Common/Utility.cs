using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	public class Utility
	{
		public static IEnumerable<T> Chain<T>(IEnumerable<IEnumerable<T>> enumerables) => enumerables.SelectMany(x => x);

		public static IEnumerable<T> Chain<T>(params IEnumerable<T>[] enumerables) => Chain(enumerables.AsEnumerable());

		public static Parser GetParser(Command rootCommand)
		{
			return new CommandLineBuilder(rootCommand)
				.UseVersionOption()
				.UseHelp()
				.UseEnvironmentVariableDirective()
				.UseParseDirective()
				.UseSuggestDirective()
				.RegisterWithDotnetSuggest()
				.UseTypoCorrections()
				.UseParseErrorReporting()
				.CancelOnProcessTermination()
#if !DEBUG
				.UseExceptionHandler()
#endif
				.Build();
		}
	}
}
