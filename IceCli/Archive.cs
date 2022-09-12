using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zamboni;

namespace Pso2Cli
{
	internal class Archive
	{
		public static IceFile LoadIceFile(FileInfo file)
		{
			using (var stream = file.OpenRead())
			{
				return IceFile.LoadIceFile(stream);
			}
		}
	}
}
