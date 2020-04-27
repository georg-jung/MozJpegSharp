using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;

namespace MozJpegSharp.Tests
{
    public class MozJpegTests
    {
        [Theory]
        [CombinatorialData]
        public void RecompressSpan(
            [CombinatorialValues(
                TJSubsamplingOption.Gray,
                TJSubsamplingOption.Chrominance411,
                TJSubsamplingOption.Chrominance420,
                TJSubsamplingOption.Chrominance440,
                TJSubsamplingOption.Chrominance422,
                TJSubsamplingOption.Chrominance444)]
            TJSubsamplingOption subsampling,
            [CombinatorialValues(1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100)]
            int quality)
        {
            foreach (var data in TestUtils.GetTestImagesData("*.jpg"))
            {
                var compressed = MozJpeg.Recompress(data.Item2.AsSpan(), quality, subsampling);
                var fileName = Path.GetFileName(data.Item1);
                Trace.WriteLine($"{fileName}; old#: {data.Item2.Length}; new#: {compressed.Length}; Ration: {compressed.Length / data.Item2.Length}; Subsampling: {subsampling}; Quality: {quality}");
            }
        }
    }
}
