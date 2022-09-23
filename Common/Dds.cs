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

		private static PixelFormat GetPixelFormat(IImage image)
		{
			switch (image.Format)
			{
				case Pfim.ImageFormat.Rgb8:
					return PixelFormat.Format8bppIndexed;

				default:
					return PixelFormat.Format32bppArgb;
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
