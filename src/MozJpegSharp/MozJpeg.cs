using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MozJpegSharp
{
    public static class MozJpeg
    {
        public static byte[] Recompress(string filePath, int quality = 70,
            TJSubsamplingOption subSampling = TJSubsamplingOption.Chrominance420,
            TJFlags flags = TJFlags.None)
        {
            var jpegBytes = System.IO.File.ReadAllBytes(filePath);
            return Recompress(jpegBytes.AsSpan(), quality, subSampling, flags);
        }

        public static byte[] Recompress(ReadOnlySpan<byte> jpegBytes, int quality = 70, TJSubsamplingOption subsampling = TJSubsamplingOption.Chrominance420,
            TJFlags flags = TJFlags.None)
        {
            var pixelFormat = TJPixelFormat.BGRA;

            using var decr = new TJDecompressor();
            var raw = decr.Decompress(jpegBytes, pixelFormat, flags, out var width, out var height, out var stride);
            decr.Dispose();

            using var compr = new TJCompressor();
            var compressed = compr.Compress(raw, stride, width, height, pixelFormat, subsampling, quality, flags);

            return compressed;
        }
    }
}
