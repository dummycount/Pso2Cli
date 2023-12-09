using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
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
}
