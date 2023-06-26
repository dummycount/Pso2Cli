using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pso2Cli
{
	public static class Pso2Path
	{
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

			return null;
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
				var match = Regex.Match(line, @"\s*""path""\s*""([^""]+)""\s*");
				if (match.Success)
				{
					yield return new DirectoryInfo(match.Groups[1].Value);
				}
			}
		}
	}
}
