using AquaModelLibrary.Data.PSO2.Aqua;
using AquaModelLibrary.Data.PSO2.Aqua.CharacterMakingIndexData;
using AquaModelLibrary.Data.Utility;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class CmxBodyMetadata
	{
		public static Command Command()
		{
			var binDirOption = Utility.GetPso2BinDirectoryOption();

			var command = new Command(name: "body", description: "Print body part metadata")
			{
				binDirOption,
			};
			command.SetHandler(Handler, binDirOption);

			return command;
		}

		private class BodyData
		{
			public string DataName { get; set; } = "";
			public List<string> TextureNames { get; set; } = [];
			public List<string> NodeNames { get; set; } = [];
			public int CostumeSoundId { get; set; }
			public int LinkedHeadId { get; set; }
			public int LinkedInnerId { get; set; }
			public int LinkedOuterId { get; set; }
			public float LegLength { get; set; }
		}

		private static void Handler(DirectoryInfo? binDir)
		{
			binDir ??= Utility.GetPso2BinDirectory();

			unsafe
			{
				var cmx = new CharacterMakingIndex();
				ReferenceGenerator.ExtractCMX(binDir.FullName, cmx);

				var parts = new Dictionary<string, SortedDictionary<int, BodyData>>
				{
					{"basewear", GetBodyData(cmx.baseWearDict)},
					{"outerwear", GetBodyData(cmx.outerDict)},
					{"costume", GetBodyData(cmx.costumeDict)},
					{"castarm", GetBodyData(cmx.carmDict)},
					{"castleg", GetBodyData(cmx.clegDict)},
				};

				Console.WriteLine(JsonSerializer.Serialize(parts));
			}
		}

		private static SortedDictionary<int, BodyData> GetBodyData(Dictionary<int, BODYObject> parts)
		{
			return new SortedDictionary<int, BodyData>(
				parts.ToDictionary(kp => kp.Key, kp =>
				{
					var body = kp.Value;

					return new BodyData
					{
						DataName = body.dataString,
						TextureNames = ToList(body.texString1, body.texString2, body.texString3, body.texString4, body.texString5, body.texString6),
						NodeNames = ToList(body.nodeString0, body.nodeString1, body.nodeString2),
						CostumeSoundId = body.body2.costumeSoundId,
						LinkedHeadId = body.body2.headId,
						LinkedInnerId = body.body2.linkedInnerId,
						LinkedOuterId = body.body2.int_3C,
						LegLength = body.body2.legLength,
					};
				}));
		}

		private static List<string> ToList(params string[] strings)
		{
			return strings.Where(s => !string.IsNullOrEmpty(s)).ToList();
		}
	}
}
