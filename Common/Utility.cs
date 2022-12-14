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
			const int ErrorExitCode = 1;

			return new CommandLineBuilder(rootCommand)
				.UseDefaults()
				.UseVersionOption()
				.UseTypoCorrections()
				.UseHelp()
				.UseExceptionHandler((exception, context) =>
				{
					if (exception is OperationCanceledException)
					{
						return;
					}

					if (!Console.IsOutputRedirected)
					{
						Console.ForegroundColor = ConsoleColor.Red;
					}

					context.Console.Error.Write(context.LocalizationResources.ExceptionHandlerHeader());
					context.Console.Error.WriteLine(exception.Message);

					if (!Console.IsOutputRedirected)
					{
						Console.ResetColor();
					}
				}, ErrorExitCode)
				.Build();
		}
	}
}
