using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class StringExtensions
	{
		public static bool Like(this string str, string pattern)
		{
			pattern = Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".");

			return new Regex($"^{pattern}$", RegexOptions.IgnoreCase | RegexOptions.Singleline).IsMatch(str);
		}

		public static string RemoveSuffix(this string str, string suffix)
		{
			if (str.EndsWith(suffix))
			{
				return str.Remove(str.Length - suffix.Length);
			}

			return str;
		}
	}
}
