using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace MozJpegSharp.SkiaSharp
{
    internal static class TJSkiaUtils
    {
        /// <summary>
        /// Converts pixel format from <see cref="PixelFormat"/> to <see cref="TJPixelFormat"/>.
        /// </summary>
        /// <param name="pixelFormat">Pixel format to convert.</param>
        /// <returns>Converted value of pixel format or exception if convertion is impossible.</returns>
        /// <exception cref="NotSupportedException">Convertion can not be performed.</exception>
        public static TJPixelFormat ConvertPixelFormat(SKColorType colorType)
        {
            return colorType switch
            {
                // Seems like skia converts the images to a platform specific representation when
                // opening. Seems like this representation does not depend on the existance of an alpha channel
                // in the image or if it's greyscale etc.

                // see SkiaSharp.SKImageInfo.PlatformColorType docs

                // default on windows
                SKColorType.Bgra8888 => TJPixelFormat.BGRA,

                // default on unix
                SKColorType.Rgba8888 => TJPixelFormat.RGBA,

                // does not seem to be a skia default anywhere but this mapping seems to be correct
                SKColorType.Rgb888x => TJPixelFormat.RGBX,

                _ => throw new NotSupportedException($"Provided pixel format \"{colorType}\" is not supported.")
            };
        }
    }
}
