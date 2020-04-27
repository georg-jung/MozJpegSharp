// <copyright file="TestUtils.cs" company="Autonomic Systems, Quamotion">
// Copyright (c) Autonomic Systems. All rights reserved.
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using SkiaSharp;

namespace MozJpegSharp.SkiaSharp.Tests
{
    internal static class TestUtils
    {
        public static string BinPath
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }

        public static IEnumerable<SKBitmap> GetTestImages(string searchPattern)
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var imagesDir = Path.Combine(Path.GetDirectoryName(path)!, "images");

            foreach (var file in Directory.EnumerateFiles(imagesDir, searchPattern))
            {
                SKBitmap bmp;
                try
                {
                    bmp = SKBitmap.Decode(file);
                    Debug.WriteLine($"Input file is {file}");
                }
                catch (OutOfMemoryException)
                {
                    continue;
                }
                catch (IOException)
                {
                    continue;
                }

                yield return bmp;
            }
        }

        public static IEnumerable<Tuple<string, byte[]>> GetTestImagesData(string searchPattern)
        {
            var imagesDir = Path.Combine(BinPath, "images");

            foreach (var file in Directory.EnumerateFiles(imagesDir, searchPattern))
            {
                Debug.WriteLine($"Input file is {file}");
                yield return new Tuple<string, byte[]>(file, File.ReadAllBytes(file));
            }
        }
    }
}
