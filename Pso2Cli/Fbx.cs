using AquaModelLibrary.Core.General;
using AquaModelLibrary.Data.PSO2.Aqua;
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
		const double BlenderScale = 100;

		/// <summary>
		/// Read an FBX file and return a PSO2 Aqua model.
		/// </summary>
		/// <param name="fbxFile"></param>
		/// <returns></returns>
		public static (AquaObject model, AquaNode skeleton) Import(FileInfo fbxFile)
		{
			AssimpModelImporter.scaleHandling = AssimpModelImporter.ScaleHandling.CustomScale;
			AssimpModelImporter.customScale = BlenderScale;

			var aqp = AssimpModelImporter.AssimpAquaConvertFull(fbxFile.FullName, scaleFactor: 1, preAssignNodeIds: false, isNGS: true, out AquaNode aqn);

			return (aqp, aqn);
		}

		/// <summary>
		/// Convert an FBX model to the PSO2 Aqua format.
		/// </summary>
		/// <param name="fbxFile">FBX file to read</param>
		/// <param name="aqpFile">AQP file to write</param>
		/// <param name="skeletonFile">Optional: AQN file to write</param>
		public static void ConvertToAqua(FileInfo fbxFile, FileInfo aqpFile, FileInfo? skeletonFile = null)
		{
			var (model, skeleton) = Import(fbxFile);

			File.WriteAllBytes(aqpFile.FullName, model.GetBytesNIFL());

			if (skeletonFile != null)
			{
				File.WriteAllBytes(skeletonFile.FullName, skeleton.GetBytesNIFL());
			}
		}

		public class MotionExport
		{
			public AquaMotion Motion { get; set; } = new AquaMotion();
			public string Name { get; set; } = "";
		}

		/// <summary>
		/// Write a PSO2 Aqua model to an FBX file.
		/// </summary>
		/// <param name="aqua"></param>
		/// <param name="fbxFile"></param>
		/// <param name="includeMetadata"></param>
		/// <param name="motions"></param>
		public static void Export(AquaObject model, AquaNode? skeleton, FileInfo fbxFile, bool includeMetadata = true, IEnumerable<MotionExport>? motions = null)
		{
			skeleton ??= AquaNode.GenerateBasicAQN();

			RemoveInvalidBones(model, skeleton);

			var aqms = motions?.Select(m => m.Motion).ToList() ?? [];
			var aqmNames = motions?.Select(m => m.Name).ToList() ?? [];

			if (model.objc.type > 0xC32)
			{
				model.splitVSETPerMesh();
			}

			model.FixHollowMatNaming();

			FbxExporterNative.ExportToFile(model, skeleton, aqms, fbxFile.FullName, aqmNames, [], includeMetadata);
		}

		/// <summary>
		/// Convert a PSO2 Aqua model to the FBX format.
		/// </summary>
		/// <param name="aqpFile">.aqp file to read</param>
		/// <param name="fbxFile">File to write</param>
		/// <param name="skeletonFile">.aqn file to read</param>
		/// <param name="motionFiles">List of .aqm files to read</param>
		/// <param name="includeMetadata"></param>
		public static AquaObject ConvertFromAqua(FileInfo aqpFile, FileInfo fbxFile, FileInfo? skeletonFile = null, bool includeMetadata = true, IEnumerable<FileInfo>? motionFiles = null)
		{
			var package = new AquaPackage(File.ReadAllBytes(aqpFile.FullName));
			var model = package.models[0];

			// var model = new AquaObject(File.ReadAllBytes(aqpFile.FullName));
			var skeleton = skeletonFile != null ? new AquaNode(File.ReadAllBytes(skeletonFile.FullName)) : null;

			var motions = motionFiles?.Select(f => new MotionExport 
			{ 
				Name = f.FullName, 
				Motion = new AquaMotion(File.ReadAllBytes(f.FullName)) 
			});

			Export(model, skeleton, fbxFile, includeMetadata, motions);

			return model;
		}

		private static void RemoveInvalidBones(AquaObject model, AquaNode skeleton)
		{
			RemoveInvalidBones(model, skeleton.nodeList.Count);
		}

		private static void RemoveInvalidBones(AquaObject model, int boneCount)
		{
			model.bonePalette = model.bonePalette.Where(index => index < boneCount).ToList();
		}
	}
}
