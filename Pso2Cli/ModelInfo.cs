using AquaModelLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal class ModelInfo
	{
		public class MaterialInfo : IComparable<MaterialInfo>
		{
			public string Name { get; set; } = "";
			public string BlendType { get; set; } = "";
			public string SpecialType { get; set; } = "";
			public int TwoSided { get; set; }
			public int AlphaCutoff { get; set; }
			public List<string> Textures { get; set; } = [];
			public List<string> Shaders { get; set; } = [];

			public int CompareTo(MaterialInfo? other)
			{
				return Name.CompareTo(other?.Name);
			}
		}

		public SortedSet<MaterialInfo> Materials { get; set; } = [];

		public ModelInfo() { }

		public ModelInfo(AquaUtil aqua)
		{
			foreach (var set in aqua.aquaModels)
			{
				foreach (var model in set.models)
				{
					foreach (var material in model.GetUniqueMaterials(out var unused))
					{
						var matInfo = new MaterialInfo
						{
							Name = material.matName,
							BlendType = material.blendType,
							SpecialType = material.specialType,
							TwoSided = material.twoSided,
							AlphaCutoff = material.alphaCutoff,
							Textures = new List<string>(material.texNames),
							Shaders = new List<string>(material.shaderNames),
						};

						Materials.Add(matInfo);
					}
				}
			}
		}

		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			PropertyNamingPolicy = new LowerCaseJsonNamingPolicy(),
			WriteIndented = true
		};

		public override string ToString()
		{
			return JsonSerializer.Serialize(this, JsonOptions);
		}
	}
}
