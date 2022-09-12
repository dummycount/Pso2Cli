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
				var bitmap = new Bitmap(image.Width, image.Height, image.Stride, PixelFormat.Format32bppArgb, data);
				bitmap.Save(pngFile.FullName, ImageFormat.Png);
			}
		}
	}
}
