
using Pso2Cli;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Convert = Pso2Cli.Convert;

var rootCommand = new RootCommand()
{
	Cmx.Command(),
	Convert.Command(),
	Ice.Command(),
};

var parser = new CommandLineBuilder(rootCommand)
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

return await parser.InvokeAsync(args);
