using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static partial class Utility
	{
		public static IEnumerable<T> Chain<T>(IEnumerable<IEnumerable<T>> enumerables) => enumerables.SelectMany(x => x);

		public static IEnumerable<T> Chain<T>(params IEnumerable<T>[] enumerables) => Chain(enumerables.AsEnumerable());

		public static Option<DirectoryInfo> GetPso2BinDirectoryOption()
		{
			return new Option<DirectoryInfo>(
				aliases: ["--bin", "-b"],
				description: "Path to pso2_bin folder")
				.ExistingOnly();
		}

		public static DirectoryInfo GetPso2BinDirectory()
		{
			var winStorePath = Path.Combine(GetProgramFiles(), "ModifiableWindowsApps", "pso2_bin");
			if (Directory.Exists(winStorePath))
			{
				return new DirectoryInfo(winStorePath);
			}

			foreach (var library in GetSteamLibraries())
			{
				var binDir = Path.Combine(library.FullName, "SteamApps", "common", "PHANTASYSTARONLINE2_NA_STEAM", "pso2_bin");
				if (Directory.Exists(binDir))
				{
					return new DirectoryInfo(binDir);
				}
			}

			throw new Exception("Couldn't find PSO2 data directory");
		}

		private static string GetProgramFiles()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
		}

		private static IEnumerable<DirectoryInfo> GetSteamLibraries()
		{
			var librariesFile = Path.Combine(GetProgramFiles(), "Steam", "SteamApps", "libraryfolders.vdf");

			foreach (var line in File.ReadLines(librariesFile))
			{
				var match = SteamPathRegex().Match(line);
				if (match.Success)
				{
					yield return new DirectoryInfo(match.Groups[1].Value);
				}
			}
		}

		[GeneratedRegex(@"\s*""path""\s*""([^""]+)""\s*")]
		private static partial Regex SteamPathRegex();
	}
}
