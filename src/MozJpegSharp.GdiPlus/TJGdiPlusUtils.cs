using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace MozJpegSharp
{
    internal static class TJGdiPlusUtils
    {

        /// <summary>
        /// Converts pixel format from <see cref="PixelFormat"/> to <see cref="TJPixelFormat"/>.
        /// </summary>
        /// <param name="pixelFormat">Pixel format to convert.</param>
        /// <returns>Converted value of pixel format or exception if convertion is impossible.</returns>
        /// <exception cref="NotSupportedException">Convertion can not be performed.</exception>
        public static TJPixelFormat ConvertPixelFormat(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    return TJPixelFormat.BGRA;
                case PixelFormat.Format24bppRgb:
                    return TJPixelFormat.BGR;
                case PixelFormat.Format8bppIndexed:
                    return TJPixelFormat.Gray;
                default:
                    throw new NotSupportedException($"Provided pixel format \"{pixelFormat}\" is not supported");
            }
        }

        internal static ColorPalette FixPaletteToGrayscale(ColorPalette palette)
        {
            for (var index = 0; index < palette.Entries.Length; ++index)
            {
                palette.Entries[index] = Color.FromArgb(index, index, index);
            }

            return palette;
        }
    }
}
