using AquaModelLibrary;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static AquaModelLibrary.CharacterMakingIndex;

namespace Pso2Cli
{
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
}
