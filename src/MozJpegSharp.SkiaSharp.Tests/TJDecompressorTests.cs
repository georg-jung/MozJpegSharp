// <copyright file="TJDecompressorTests.cs" company="Autonomic Systems, Quamotion">
// Copyright (c) Autonomic Systems. All rights reserved.
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Drawing.Imaging;
using System.IO;
using SkiaSharp;
using Xunit;

namespace MozJpegSharp.SkiaSharp.Tests
{
    public class TJDecompressorTests : IDisposable
    {
        private TJDecompressor decompressor;

        public TJDecompressorTests()
        {
            this.decompressor = new TJDecompressor();
            if (Directory.Exists(this.OutDirectory))
            {
                Directory.Delete(this.OutDirectory, true);
            }

            Directory.CreateDirectory(this.OutDirectory);
        }

        private string OutDirectory
        {
            get { return Path.Combine(TestUtils.BinPath, "decompress_images_out"); }
        }

        public void Dispose()
        {
            this.decompressor.Dispose();
        }

        [Theory]
        [CombinatorialData]
        public void DecompressByteArray(
            [CombinatorialValues(
            SKColorType.Bgra8888,
            SKColorType.Rgba8888,
            SKColorType.Rgb888x)]
            SKColorType format)
        {
            foreach (var data in TestUtils.GetTestImagesData("*.jpg"))
            {
                var result = this.decompressor.Decompress(data.Item2, format, TJFlags.None);
                Assert.NotNull(result);

                var file = Path.Combine(this.OutDirectory, $"{Path.GetFileNameWithoutExtension(data.Item1)}_{format}.bmp");
                using var s = new SKFileWStream(file);
                SKPixmap.Encode(s, result, SKEncodedImageFormat.Bmp, 100);
            }
        }

        [Theory]
        [CombinatorialData]
        public void DecompressIntPtr(
            [CombinatorialValues(
                SKColorType.Bgra8888,
                SKColorType.Rgba8888,
                SKColorType.Rgb888x)]
            SKColorType format)
        {
            foreach (var data in TestUtils.GetTestImagesData("*.jpg"))
            {
                var dataPtr = TJUtils.CopyDataToPointer(data.Item2);
                var result = this.decompressor.Decompress(dataPtr, (ulong)data.Item2.Length, format, TJFlags.None);
                Assert.NotNull(result);
                TJUtils.FreePtr(dataPtr);
            }
        }

        [Theory]
        [CombinatorialData]
        public void DecompressSpan(
            [CombinatorialValues(
            TJPixelFormat.ABGR,
            TJPixelFormat.RGB,
            TJPixelFormat.Gray)]
            TJPixelFormat format)
        {
            byte[] outBuf = ArrayPool<byte>.Shared.Rent(250 * 250 * 4);

            foreach (var data in TestUtils.GetTestImagesData("*.jpg"))
            {
                var dataSpan = data.Item2.AsSpan();
                this.decompressor.Decompress(dataSpan, outBuf.AsSpan(), format, TJFlags.None, out int width, out int height, out int stride);
            }

            ArrayPool<byte>.Shared.Return(outBuf);
        }

        [Theory]
        [CombinatorialData]
        public void DecompressArray(
            [CombinatorialValues(
            TJPixelFormat.ABGR,
            TJPixelFormat.RGB,
            TJPixelFormat.Gray)]
            TJPixelFormat format)
        {
            byte[] outBuf = ArrayPool<byte>.Shared.Rent(250 * 250 * 4);

            foreach (var data in TestUtils.GetTestImagesData("*.jpg"))
            {
                this.decompressor.Decompress(data.Item2, outBuf, format, TJFlags.None, out int width, out int height, out int stride);
            }

            ArrayPool<byte>.Shared.Return(outBuf);
        }
    }
}
