using AquaModelLibrary;
using AquaModelLibrary.Native.Fbx;
using AquaModelLibrary.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	public class Fbx
	{
		/// <summary>
		/// Read an FBX file and return a PSO2 Aqua model.
		/// </summary>
		/// <param name="fbxFile"></param>
		/// <returns></returns>
		public static AquaUtil Import(FileInfo fbxFile)
		{
			var aqua = new AquaUtil();
			var modelSet = new AquaUtilData.ModelSet();
			modelSet.models.Add(ModelImporter.AssimpAquaConvertFull(fbxFile.FullName, scaleFactor: 1, preAssignNodeIds: false, isNGS: true, out var aqn));
			aqua.aquaModels.Add(modelSet);
			aqua.aquaBones.Add(aqn);

			return aqua;
		}

		/// <summary>
		/// Convert an FBX model to the PSO2 Aqua format.
		/// </summary>
		/// <param name="fbxFile">FBX file to read</param>
		/// <param name="modelFile">AQP file to write</param>
		/// <param name="skeletonFile">Optional: AQN file to write</param>
		public static void ConvertToAqua(FileInfo fbxFile, FileInfo modelFile, FileInfo skeletonFile = null)
		{
			fbxFile = fbxFile ?? throw new ArgumentNullException(nameof(fbxFile));
			modelFile = modelFile ?? throw new ArgumentNullException(nameof(modelFile));

			var aqua = Import(fbxFile);

			aqua.WriteNGSNIFLModel(modelFile.FullName, modelFile.FullName);

			if (skeletonFile != null)
			{
				AquaUtil.WriteBones(skeletonFile.FullName, aqua.aquaBones.First());
			}
		}

		public class MotionExport
		{
			public AquaMotion Motion { get; set; }
			public string Name { get; set; }
		}

		/// <summary>
		/// Write a PSO2 Aqua model to an FBX file.
		/// </summary>
		/// <param name="aqua"></param>
		/// <param name="fbxFile"></param>
		/// <param name="includeMetadata"></param>
		/// <param name="motions"></param>
		public static void Export(AquaUtil aqua, FileInfo fbxFile, bool includeMetadata = true, IEnumerable<MotionExport> motions = null)
		{
			if (aqua.aquaBones.Count == 0)
			{
				aqua.aquaBones.Add(AquaNode.GenerateBasicAQN());
			}

			RemoveInvalidBones(aqua);

			var aqms = motions?.Select(m => m.Motion).ToList() ?? new List<AquaMotion>();
			var aqmNames = motions?.Select(m => m.Name).ToList() ?? new List<string>();

			var model = aqua.aquaModels.First().models.First();
			if (model.objc.type > 0xC32)
			{
				model.splitVSETPerMesh();
			}

			FbxExporter.ExportToFile(model, aqua.aquaBones.First(), aqms, fbxFile.FullName, aqmNames, new List<Matrix4x4>(), includeMetadata);
		}

		/// <summary>
		/// Convert a PSO2 Aqua model to the FBX format.
		/// </summary>
		/// <param name="fbxFile">File to write</param>
		/// <param name="modelFile">.aqp file to read</param>
		/// <param name="skeletonFile">.aqn file to read</param>
		/// <param name="motionFiles">List of .aqm files to read</param>
		/// <param name="includeMetadata"></param>
		public static void ConvertFromAqua(FileInfo fbxFile, FileInfo modelFile, FileInfo skeletonFile = null, bool includeMetadata = true, IEnumerable < FileInfo> motionFiles = null)
		{
			var model = File.ReadAllBytes(modelFile.FullName);
			var skeleton = skeletonFile != null ? File.ReadAllBytes(skeletonFile.FullName) : null;
			ConvertFromAqua(fbxFile, model, skeleton, includeMetadata, motionFiles);
		}

		/// <summary>
		/// Convert a PSO2 Aqua model to the FBX format.
		/// </summary>
		/// <param name="fbxFile">File to write</param>
		/// <param name="modelFile">.aqp file to read</param>
		/// <param name="skeletonFile">.aqn file to read</param>
		/// <param name="motionFiles">List of .aqm files to read</param>
		/// <param name="includeMetadata"></param>
		public static void ConvertFromAqua(FileInfo fbxFile, byte[] modelFile, byte[] skeletonFile, bool includeMetadata = true, IEnumerable < FileInfo> motionFiles = null)
		{
			var aqua = new AquaUtil();
			var aqms = new List<AquaMotion>();
			var aqmFileNames = new List<string>();

			aqua.ReadModel(modelFile);

			if (skeletonFile != null)
			{
				aqua.ReadBones(skeletonFile);
			}

			var motions = motionFiles?.Select(f => new MotionExport { Name = f.FullName, Motion = ReadMotion(f) });

			Export(aqua, fbxFile, includeMetadata, motions);
		}

		private static AquaMotion ReadMotion(FileInfo file)
		{
			var aqm = new AquaUtil();
			aqm.ReadMotion(file.FullName);
			return aqm.aquaMotions.First().anims.First();
		}

		

		private static void RemoveInvalidBones(AquaUtil aqua)
		{
			foreach (var modelset in aqua.aquaModels)
			{
				foreach (var model in modelset.models)
				{
					RemoveInvalidBones(model, aqua.aquaBones[0].nodeList.Count);
				}
			}
		}

		private static void RemoveInvalidBones(AquaObject model, int boneCount)
		{
			model.bonePalette = model.bonePalette.Where(index => index < boneCount).ToList();
		}

	}
}
