using System;
using System.IO;
using Xunit;

namespace MozJpegSharp.Tests
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

        [Fact]
        public void CompressInvalidIntPtr()
        {
            Assert.Throws<TJException>(() => this.compressor.Compress(IntPtr.Zero, 0, 0, 0, TJPixelFormat.BGRA, TJSubsamplingOption.Gray, 0, TJFlags.None));
        }
    }
}
