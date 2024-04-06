using Pfim;
using System.CommandLine;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Pso2Cli;

static internal class ConvertToPng
{
	public static Command Command()
	{
		var sourceArg = new Argument<FileInfo>(name: "file", description: "Image to convert")
			.ExistingOnly();

		var destArg = new Argument<FileInfo>(name: "dest", description: "Converted file [default: <file>.png]")
		{
			Arity = ArgumentArity.ZeroOrOne
		};

		var command = new Command(name: "png", description: "Convert images to PNG")
		{
			sourceArg,
			destArg,
		};

		command.SetHandler(Handler, sourceArg, destArg);

		return command;
	}

	private static void Handler(FileInfo source, FileInfo? dest)
	{
		dest ??= new FileInfo(Path.ChangeExtension(source.FullName, ".png"));

		if (dest.FullName == source.FullName)
		{
			throw new ArgumentException("Source and destination cannot be the same file");
		}

		var sourceFormat = Path.GetExtension(source.FullName).ToLower();
		switch (sourceFormat)
		{
			case ".dds":
				ConvertDds(source, dest);
				break;

			default:
				throw new ArgumentException($"Unsupported format: {sourceFormat}");
		}
	}

	private static void ConvertDds(FileInfo source, FileInfo dest)
	{
		using var image = Pfimage.FromFile(source.FullName);

		var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
		var bitmap = new Bitmap(image.Width, image.Height, image.Stride, GetPixelFormat(image), data);

		if (image.Format == Pfim.ImageFormat.Rgb8)
		{
			SetGrayscaleColorPalette(bitmap);
		}

		Directory.CreateDirectory(dest.DirectoryName!);
		bitmap.Save(dest.FullName, ImageFormat.Png);
	}

	private static PixelFormat GetPixelFormat(IImage image)
	{
		return image.Format switch
		{
			Pfim.ImageFormat.Rgb24 => PixelFormat.Format24bppRgb,
			Pfim.ImageFormat.Rgba32 => PixelFormat.Format32bppArgb,
			Pfim.ImageFormat.R5g5b5 => PixelFormat.Format16bppRgb555,
			Pfim.ImageFormat.R5g6b5 => PixelFormat.Format16bppRgb565,
			Pfim.ImageFormat.R5g5b5a1 => PixelFormat.Format16bppArgb1555,
			Pfim.ImageFormat.Rgb8 => PixelFormat.Format8bppIndexed,
			_ => throw new NotImplementedException($"Unsupported image format: {image.Format}"),
		};
	}

	private static void SetGrayscaleColorPalette(Bitmap bitmap)
	{
		var palette = bitmap.Palette;
		for (var i = 0; i < palette.Entries.Length; i++)
		{
			palette.Entries[i] = Color.FromArgb(i, i, i);
		}
		bitmap.Palette = palette;
	}
}
