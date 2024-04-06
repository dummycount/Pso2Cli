using System.CommandLine;

namespace Pso2Cli;

internal class Ice
{
	public const string ExtractedSuffix = ".extracted";

	public static Command Command()
	{
		return new Command(name: "ice", description: "ICE archive commands")
		{
			IceList.Command(),
			IcePack.Command(),
			IceUnpack.Command(),
		};
	}
}
