using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
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
}
