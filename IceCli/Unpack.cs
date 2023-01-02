using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zamboni;

namespace Pso2Cli
{
	internal class Unpack
	{
		public static Command GetCommand()
		{
			var fileArg = new Argument<FileInfo>(name: "file", description: "The file to extract")
				.ExistingOnly();

			var destArg = new Argument<DirectoryInfo>(name: "dest", description: "The directory to extract to (<file>.extracted)")
			{
				Arity = ArgumentArity.ZeroOrOne
			};

			var flattenArg = new Option<bool>(new string[] { "--flatten", "-f" }, description: "Do not organize into group folders");

			var fbxArg = new Option<bool>(new string[] { "--fbx", "-x" }, description: "Convert extracted models to .fbx format");
			var pngArg = new Option<bool>(new string[] { "--png", "-p" }, description: "Convert extracted textures to .png format");

			var metaArg = new Option<bool>("--no-metadata", description: "Do not include metadata in a model exported with --fbx");

			var command = new Command("unpack", "Extract an ICE archive")
			{
				fileArg,
				destArg,
				flattenArg,
				fbxArg,
				pngArg,
				metaArg,
			};

			command.SetHandler((file, dest, flatten, exportFbx, exportPng, noMetadata) =>
			{
				UnpackArchive(
					file: file,
					dest: dest,
					flatten: flatten,
					exportFbx: exportFbx,
					exportPng: exportPng,
					useMetadata: !noMetadata);

			}, fileArg, destArg, flattenArg, fbxArg, pngArg, metaArg);

			return command;
		}

		static void UnpackArchive(FileInfo file, DirectoryInfo dest, bool flatten, bool exportFbx, bool exportPng, bool useMetadata)
		{
			file = file ?? throw new ArgumentNullException(nameof(file));

			dest = dest ?? new DirectoryInfo(file.FullName + ".extracted");

			var group1 = flatten ? dest : GetGroupDirectory(dest, "group1");
			var group2 = flatten ? dest : GetGroupDirectory(dest, "group2");

			var archive = Archive.LoadIceFile(file);

			var group1Result = UnpackGroup(archive.groupOneFiles, group1);
			var group2Result = UnpackGroup(archive.groupTwoFiles, group2);

			var exportedFiles = Utility.Chain(group1Result, group2Result).ToList();

			if (exportFbx)
			{
				ConvertModelsToFbx(exportedFiles, useMetadata);
			}

			if (exportPng)
			{
				ConvertTexturesToPng(exportedFiles);
			}
		}

		private static IEnumerable<FileInfo> UnpackGroup(byte[][] files, DirectoryInfo dest)
		{
			if (files.Length == 0)
			{
				yield break;
			}

			Directory.CreateDirectory(dest.FullName);

			foreach (var bytes in files) {
				var name = IceFile.getFileName(bytes);

				byte[] file;
				if (name == "namelessFile.bin")
				{
					file = bytes;
				} 
				else
				{
					var headerSize = BitConverter.ToInt32(bytes, 0xC);
					var length = bytes.Length - headerSize;
					file = new byte[length];
					Array.ConstrainedCopy(bytes, headerSize, file, 0, length);
				}

				var path = Path.Combine(dest.FullName, name);
				File.WriteAllBytes(path, file);
				yield return new FileInfo(path);
			}
		}

		static DirectoryInfo GetGroupDirectory(DirectoryInfo dest, string name)
		{
			return new DirectoryInfo(Path.Combine(dest.FullName, name));
		}

		private static void ConvertModelsToFbx(IEnumerable<FileInfo> files, bool useMetadata)
		{
			foreach (var file in files.Where(f => f.Extension.ToLower() == ".aqp"))
			{
				var dest = new FileInfo(Path.ChangeExtension(file.FullName, ".fbx"));
				var skeleton = new FileInfo(Path.ChangeExtension(file.FullName, ".aqn"));

				Fbx.ConvertFromAqua(dest, file, skeleton.Exists ? skeleton : null);
			}
		}

		private static void ConvertTexturesToPng(IEnumerable<FileInfo> files)
		{
			foreach (var file in files.Where(f => f.Extension.ToLower() == ".dds"))
			{
				var dest = new FileInfo(Path.ChangeExtension(file.FullName, ".png"));

				Dds.ConvertToPng(file, dest);
			}
		}
	}
}
