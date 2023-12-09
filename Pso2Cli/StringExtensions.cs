using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	internal static class StringExtensions
	{
		public static string RemoveSuffix(this string str, string suffix)
		{
			if (str.EndsWith(suffix))
			{
				return str[..^suffix.Length];
			}

			return str;
		}
	}
}
