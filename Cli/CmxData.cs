using AquaModelLibrary;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class CmxData
	{
		public static Command GetCommand()
		{
			var binDirOption = new Option<DirectoryInfo>(
				new string[] { "--bin", "-b" },
				description: "Path to pso2_bin folder")
				.ExistingOnly();

			var colorChannels = new Command(name: "colorchannels", description: "List color channels for all parts")
			{
				binDirOption,
			};
			colorChannels.SetHandler(GetColorChannels, binDirOption);

			var command = new Command(name: "cmx", description: "Inspect CMX data");
			command.AddCommand(colorChannels);

			return command;
		}

		private static void GetColorChannels(DirectoryInfo binDir)
		{
			binDir = binDir ?? Pso2Path.GetPso2BinDirectory();
			if (binDir == null)
			{
				throw new Exception("Couldn't find PSO2 data directory");
			}

			var cmx = CharacterMakingIndexMethods.ExtractCMX(binDir.FullName);

			var parts = new Dictionary<string, SortedDictionary<int, int[]>>
			{
				{"basewear", GetColorChannels(cmx.baseWearDict)}, 
				{"outerwear", GetColorChannels(cmx.outerDict)},
				{"costume", GetColorChannels(cmx.costumeDict)},
				{"castarm", GetColorChannels(cmx.carmDict)},
				{"castleg", GetColorChannels(cmx.clegDict)},
				{"hair", GetColorChannels(cmx.hairDict)},
			};

			Console.WriteLine(JsonSerializer.Serialize(parts));
		}

		private static SortedDictionary<int, int[]> GetColorChannels(Dictionary<int, CharacterMakingIndex.BODYObject> parts)
		{
			return new SortedDictionary<int, int[]>(
				parts.ToDictionary(kp => kp.Key, kp => new int[]
				{
					kp.Value.bodyRitem.int_0,
					kp.Value.bodyRitem.int_4,
					kp.Value.bodyRitem.int_8,
					kp.Value.bodyRitem.int_C,
				})
			);
		}

		private static SortedDictionary<int, int[]> GetColorChannels(Dictionary<int, CharacterMakingIndex.HAIRObject> parts)
		{
			return new SortedDictionary<int, int[]>(
				parts.Where(kp => kp.Key >= 100000).ToDictionary(kp => kp.Key, kp =>
				{
					var (r, g) = SplitInt32(kp.Value.hair.unkInt16);
					var (b, a) = SplitInt32(kp.Value.hair.unkInt17);

					return new int[] { r, g, b, a };
				})
			);
		}

		private static (int, int) SplitInt32(int value)
		{
			var uvalue = (uint)value;
			var lo = (int)(uvalue & 0xFFFF);
			var hi = (int)(uvalue >> 16);

			return (lo, hi);
		}
	}
}
