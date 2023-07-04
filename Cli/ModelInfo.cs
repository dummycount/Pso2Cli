using AquaModelLibrary;
using AquaModelLibrary.Extra;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pso2Cli
{
	class ModelInfo
	{
		public class MaterialInfo : IComparable<MaterialInfo>
		{
			public string Name { get; set; }
			public string BlendType { get; set; }
			public string SpecialType { get; set; }
			public int TwoSided { get; set; }
			public int AlphaCutoff { get; set; }
			public List<string> Textures { get; set; } = new List<string>();
			public List<string> Shaders { get; set; } = new List<string>();

			int IComparable<MaterialInfo>.CompareTo(MaterialInfo other)
			{
				return Name.CompareTo(other.Name);
			}
		}

		public SortedSet<MaterialInfo> Materials { get; set; } = new SortedSet<MaterialInfo>();

        public ModelInfo() {}

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

		public override string ToString()
		{
			return JsonSerializer.Serialize(this, new JsonSerializerOptions
			{
				PropertyNamingPolicy = new LowerCaseJsonNamingPolicy(),
				WriteIndented = true
			});
		}
	}
}
