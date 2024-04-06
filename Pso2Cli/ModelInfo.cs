using AquaModelLibrary.Data.PSO2.Aqua;
using System.Text.Json;

namespace Pso2Cli;

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

	public ModelInfo(AquaObject model)
	{
		AddObject(model);
	}

	public ModelInfo(AquaPackage package)
	{
		foreach (var model in package.models)
		{
			AddObject(model);
		}
	}

	public void AddObject(AquaObject model)
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
