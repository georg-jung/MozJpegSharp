// <copyright file="TJCompressorTests.cs" company="Autonomic Systems, Quamotion">
// Copyright (c) Autonomic Systems. All rights reserved.
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace MozJpegSharp.SkiaSharp.Tests
{
    public class TJCompressorTests : IDisposable
    {
        private TJCompressor compressor;

        public TJCompressorTests()
        {
            this.compressor = new TJCompressor();
            if (Directory.Exists(this.OutDirectory))
            {
                Directory.Delete(this.OutDirectory, true);
            }

            Directory.CreateDirectory(this.OutDirectory);
        }

        private string OutDirectory
        {
            get { return Path.Combine(TestUtils.BinPath, "compress_images_out"); }
        }

        public void Dispose()
        {
            this.compressor.Dispose();
        }

        [Theory]
        [CombinatorialData]
        public void CompressBitmap(
            [CombinatorialValues(
            TJSubsamplingOption.Gray,
            TJSubsamplingOption.Chrominance411,
            TJSubsamplingOption.Chrominance420,
            TJSubsamplingOption.Chrominance440,
            TJSubsamplingOption.Chrominance422,
            TJSubsamplingOption.Chrominance444)]
            TJSubsamplingOption options,
            [CombinatorialValues(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
            int quality)
        {
            var imageidx = 0;
            foreach (var bitmap in TestUtils.GetTestImages("*.bmp"))
            {
                try
                {
                    Trace.WriteLine($"Options: {options}; Quality: {quality}");

                    var result = this.compressor.Compress(bitmap, options, quality, TJFlags.None);

                    Assert.NotNull(result);

                    var file = Path.Combine(this.OutDirectory, $"{imageidx}_{quality}_{options}.jpg");
                    File.WriteAllBytes(file, result);
                }
                finally
                {
                    bitmap.Dispose();
                }

                imageidx++;
            }
        }

        [Theory]
        [CombinatorialData]
        public void CompressIntPtr(
            [CombinatorialValues(
            TJSubsamplingOption.Gray,
            TJSubsamplingOption.Chrominance411,
            TJSubsamplingOption.Chrominance420,
            TJSubsamplingOption.Chrominance440,
            TJSubsamplingOption.Chrominance422,
            TJSubsamplingOption.Chrominance444)]
            TJSubsamplingOption options,
            [CombinatorialValues(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
            int quality)
        {
            foreach (var bitmap in TestUtils.GetTestImages("*.bmp"))
            {
                try
                {
                    Trace.WriteLine($"Options: {options}; Quality: {quality}");
                    var result = this.compressor.Compress(bitmap.GetPixels(), bitmap.RowBytes, bitmap.Width, bitmap.Height, TJSkiaUtils.ConvertPixelFormat(bitmap.ColorType), options, quality, TJFlags.None);
                    Assert.NotNull(result);
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
        }

        [Theory]
        [CombinatorialData]
        public void CompressByteArray(
            [CombinatorialValues(
            TJSubsamplingOption.Gray,
            TJSubsamplingOption.Chrominance411,
            TJSubsamplingOption.Chrominance420,
            TJSubsamplingOption.Chrominance440,
            TJSubsamplingOption.Chrominance422,
            TJSubsamplingOption.Chrominance444)]
            TJSubsamplingOption options,
            [CombinatorialValues(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
            int quality)
        {
            foreach (var bitmap in TestUtils.GetTestImages("*.bmp"))
            {
                try
                {
                    var stride = bitmap.RowBytes;
                    var width = bitmap.Width;
                    var height = bitmap.Height;
                    var pixelFormat = TJSkiaUtils.ConvertPixelFormat(bitmap.ColorType);

                    var buf = bitmap.GetPixelSpan().ToArray();

                    Trace.WriteLine($"Options: {options}; Quality: {quality}");
                    var result = this.compressor.Compress(buf, stride, width, height, pixelFormat, options, quality, TJFlags.None);
                    Assert.NotNull(result);
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
        }

        [Theory]
        [CombinatorialData]
        public void CompressByteArrayToByteArray(
            [CombinatorialValues(
            TJSubsamplingOption.Gray,
            TJSubsamplingOption.Chrominance411,
            TJSubsamplingOption.Chrominance420,
            TJSubsamplingOption.Chrominance440,
            TJSubsamplingOption.Chrominance422,
            TJSubsamplingOption.Chrominance444)]
            TJSubsamplingOption options,
            [CombinatorialValues(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
            int quality)
        {
            foreach (var bitmap in TestUtils.GetTestImages("*.bmp"))
            {
                try
                {
                    var stride = bitmap.RowBytes;
                    var width = bitmap.Width;
                    var height = bitmap.Height;
                    var pixelFormat = TJSkiaUtils.ConvertPixelFormat(bitmap.ColorType);

                    var buf = bitmap.GetPixelSpan().ToArray();

                    Trace.WriteLine($"Options: {options}; Quality: {quality}");
                    byte[] target = new byte[this.compressor.GetBufferSize(width, height, options)];

                    this.compressor.Compress(buf, target, stride, width, height, pixelFormat, options, quality, TJFlags.None);
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
        }

        [Theory]
        [CombinatorialData]
        public unsafe void CompressSpanToSpan(
            [CombinatorialValues(
            TJSubsamplingOption.Gray,
            TJSubsamplingOption.Chrominance411,
            TJSubsamplingOption.Chrominance420,
            TJSubsamplingOption.Chrominance440,
            TJSubsamplingOption.Chrominance422,
            TJSubsamplingOption.Chrominance444)]
            TJSubsamplingOption options,
            [CombinatorialValues(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
            int quality)
        {
            foreach (var bitmap in TestUtils.GetTestImages("*.bmp"))
            {
                try
                {
                    var stride = bitmap.RowBytes;
                    var width = bitmap.Width;
                    var height = bitmap.Height;
                    var pixelFormat = TJSkiaUtils.ConvertPixelFormat(bitmap.ColorType);

                    var buf = bitmap.GetPixelSpan();

                    Trace.WriteLine($"Options: {options}; Quality: {quality}");
                    Span<byte> target = new byte[this.compressor.GetBufferSize(width, height, options)];

                    this.compressor.Compress(buf, target, stride, width, height, pixelFormat, options, quality, TJFlags.None);
                }
                finally
                {
                    bitmap.Dispose();
                }
            }
        }
    }
}
