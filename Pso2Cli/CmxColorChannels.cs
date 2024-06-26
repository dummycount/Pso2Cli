﻿using AquaModelLibrary.Data.PSO2.Aqua;
using AquaModelLibrary.Data.PSO2.Aqua.CharacterMakingIndexData;
using AquaModelLibrary.Data.Utility;
using System.CommandLine;
using System.Text.Json;

namespace Pso2Cli;

internal static class CmxColorChannels
{
	public static Command Command()
	{
		var binDirOption = Utility.GetPso2BinDirectoryOption();

		var command = new Command(name: "colorchannels", description: "Print color channels for all parts")
		{
			binDirOption,
		};
		command.SetHandler(Handler, binDirOption);

		return command;
	}

	private static void Handler(DirectoryInfo? binDir)
	{
		binDir ??= Utility.GetPso2BinDirectory();

		unsafe
		{
			var cmx = new CharacterMakingIndex();
			ReferenceGenerator.ExtractCMX(binDir.FullName, cmx);

			var parts = new Dictionary<string, SortedDictionary<int, CharColorMapping[]>>
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
	}

	private static SortedDictionary<int, CharColorMapping[]> GetColorChannels(Dictionary<int, BODYObject> parts)
	{
		return new SortedDictionary<int, CharColorMapping[]>(
			parts.ToDictionary(kp => kp.Key, kp => new CharColorMapping[]
			{
				kp.Value.bodyMaskColorMapping.redIndex,
				kp.Value.bodyMaskColorMapping.greenIndex,
				kp.Value.bodyMaskColorMapping.blueIndex,
				kp.Value.bodyMaskColorMapping.alphaIndex,
			})
		);
	}

	private static SortedDictionary<int, CharColorMapping[]> GetColorChannels(Dictionary<int, HAIRObject> parts)
	{
		return new SortedDictionary<int, CharColorMapping[]>(
			parts.Where(kp => kp.Key >= 100000).ToDictionary(kp => kp.Key, kp =>
			{
				var (r, g) = SplitInt32(kp.Value.hair.unkInt16);
				var (b, a) = SplitInt32(kp.Value.hair.unkInt17);


				return new CharColorMapping[] {
					(CharColorMapping)r,
					(CharColorMapping)g,
					(CharColorMapping)b,
					(CharColorMapping)a
				};
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
