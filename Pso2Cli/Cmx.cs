using System.CommandLine;

namespace Pso2Cli;

internal static class Cmx
{
	public static Command Command()
	{
		return new Command(name: "cmx", description: "CMX data commands")
		{
			CmxBodyMetadata.Command(),
			CmxColorChannels.Command(),
			CmxSheets.Command(),
		};
	}
}
