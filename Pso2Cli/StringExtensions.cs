namespace Pso2Cli;

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
