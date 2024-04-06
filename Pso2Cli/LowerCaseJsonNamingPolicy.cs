using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pso2Cli;

internal class LowerCaseJsonNamingPolicy : JsonNamingPolicy
{
	private static readonly Regex StartRegex = new Regex(@"^[A-Z]+");
	private static readonly Regex WordRegex = new Regex(@"(?<=[^A-Z])[A-Z]+");

	public override string ConvertName(string name)
	{
		name = StartRegex.Replace(name, match => match.Value.ToLower());
		name = WordRegex.Replace(name, match => $"_{match.Value.ToLower()}");

		return name;
	}
}
