using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Cli
{
	public static class StringExtensions
	{
		public static string RemovePrefix(this string str, string prefix)
		{
			if (str.StartsWith(prefix))
			{
				return str.Remove(0, prefix.Length);
			}
			return str;
		}
	}
}
