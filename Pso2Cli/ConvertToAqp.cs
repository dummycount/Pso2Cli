using System.CommandLine;

namespace Pso2Cli;

static internal class ConvertToAqp
{
	public static Command Command()
	{
		var sourceArg = new Argument<FileInfo>(name: "file", description: "Model to convert")
			.ExistingOnly();

		var destArg = new Argument<FileInfo>(name: "dest", description: "Converted file (<file>.aqp)")
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		var aqnArg = new Option<FileInfo>(
			aliases: ["--skeleton", "-s"], 
			description: "Converted .aqn file [default: <dest>.aqn]");

		var updateAqnOption = new Option<bool>(
			aliases: ["--update-aqn", "-u"], 
			description: "Overwrite existing .aqn file if --skeleton not specified");

		var scaleOption = new Option<Fbx.IScaleValue>(
			aliases: ["--scale", "-x"],
			parseArgument: (result) =>
			{
				if (!result.Tokens.Any())
				{
					return Fbx.DefaultScale;
				}

				var token = result.Tokens.Single().Value.ToLower();
				if (token == "none")
				{
					return new Fbx.NoScale();
				}
				if (token == "file")
				{
					return new Fbx.FileScale();
				}
				if (double.TryParse(token, out var scale))
				{
					return new Fbx.CustomScale(scale);
				}

				result.ErrorMessage = "Scale must be a number, \"file\" or \"none\".";
				return Fbx.DefaultScale;
			},
			description: "Scale multiplier, \"file\", or \"none\" [default: file]");

		var command = new Command(name: "aqp", description: "Convert models to AQP")
		{
			sourceArg,
			destArg,
			aqnArg,
			updateAqnOption,
			scaleOption,
		};

		command.SetHandler(Handler, sourceArg, destArg, aqnArg, updateAqnOption, scaleOption);

		return command;
	}

	private static void Handler(FileInfo source, FileInfo? dest, FileInfo? aqn, bool updateAqn, Fbx.IScaleValue scale)
	{
		dest ??= new FileInfo(Path.ChangeExtension(source.FullName, ".aqp"));

		if (aqn == null)
		{
			var file = new FileInfo(Path.ChangeExtension(dest.FullName, ".aqn"));
			if (updateAqn || !file.Exists)
			{
				aqn = file;
			}
		}

		var format = Path.GetExtension(source.FullName).ToLower();
		switch (format)
		{
			case ".fbx":
				Directory.CreateDirectory(dest.DirectoryName!);
				Fbx.ConvertToAqua(fbxFile: source, aqpFile: dest, skeletonFile: aqn, scale: scale);
				break;

			default:
				throw new ArgumentException($"Unsupported format: {format}");
		}
	}
}
