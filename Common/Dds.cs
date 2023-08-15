using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Pso2Cli
{
	public class Dds
	{
		public static void ConvertToPng(FileInfo ddsFile, FileInfo pngFile)
		{
			using (var image = Pfimage.FromFile(ddsFile.FullName))
			{
				var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);

				var bitmap = new Bitmap(image.Width, image.Height, image.Stride, GetPixelFormat(image), data);
				UpdateColorPalette(bitmap);

				bitmap.Save(pngFile.FullName, ImageFormat.Png);
			}
		}

		public static MemoryStream ConvertToPng(Stream stream)
		{
			using (var image = Pfimage.FromStream(stream)) 
			{
				var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);

				var bitmap = new Bitmap(image.Width, image.Height, image.Stride, GetPixelFormat(image), data);
				UpdateColorPalette(bitmap);

				var result = new MemoryStream();
				bitmap.Save(result, ImageFormat.Png);
				result.Position = 0;

				return result;
			}
		}

		private static PixelFormat GetPixelFormat(IImage image)
		{
			switch (image.Format)
			{
				case Pfim.ImageFormat.Rgb24:
					return PixelFormat.Format24bppRgb;

				case Pfim.ImageFormat.Rgba32:
					return PixelFormat.Format32bppArgb;

				case Pfim.ImageFormat.R5g5b5:
					return PixelFormat.Format16bppRgb555;

				case Pfim.ImageFormat.R5g6b5:
					return PixelFormat.Format16bppRgb565;

				case Pfim.ImageFormat.R5g5b5a1:
					return PixelFormat.Format16bppArgb1555;

				case Pfim.ImageFormat.Rgb8:
					return PixelFormat.Format8bppIndexed;

				default:
					throw new NotImplementedException($"Unsupported image format: {image.Format}");
			}
		}

		private static void UpdateColorPalette(Bitmap bitmap)
		{
			switch (bitmap.PixelFormat)
			{
				case PixelFormat.Format8bppIndexed:
					var palette = bitmap.Palette;
					for (var i = 0; i < palette.Entries.Length; i++)
					{
						palette.Entries[i] = Color.FromArgb(i, i, i);
					}
					bitmap.Palette = palette;
					break;
			}
		}
	}
}
