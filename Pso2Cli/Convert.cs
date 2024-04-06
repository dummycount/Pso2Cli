using System.CommandLine;

namespace Pso2Cli;

static internal class Convert
{
	public static Command Command()
	{
		return new Command(name: "convert", description: "Convert between file formats")
		{
			ConvertToAqp.Command(),
			ConvertToFbx.Command(),
			ConvertToPng.Command(),
		};
	}
}
