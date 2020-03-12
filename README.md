<p align="center">
  <a href="https://www.nuget.org/packages/MozJpegSharp/">
    <img
      alt="MozJpegSharp logo"
      src="doc/logo.svg"
      width="100"
    />
  </a>
</p>

# MozJpegSharp (.NET wrapper for MozJPEG)
[![NuGet version (MozJpegSharp)](https://img.shields.io/nuget/v/MozJpegSharp.svg?style=flat)](https://www.nuget.org/packages/MozJpegSharp/)
[![Build Status](https://dev.azure.com/georg-jung/MozJpegSharp/_apis/build/status/georg-jung.MozJpegSharp?branchName=master)](https://dev.azure.com/georg-jung/MozJpegSharp/_build/latest?definitionId=6&branchName=master)

> MozJPEG reduces file sizes of JPEG images while retaining quality and compatibility with the vast majority of the world's deployed decoders.

This library provides .NET wrappers for MozJPEG, allowing you to use it from any .NET language, such as C#.

For Windows (32-bit and 64-bit) and macOS (all versions), the NuGet package includes MozJPEG. On these platforms you just need to install the package to start producing smaller JPEGs. For Linux distributions, you need to install MozJPEG using your package manager (if available) or compile it from source.


## Installation

Install using the package manager console:

```
Install-Package MozJpegSharp
```

#### macOS - .NET Core
Make sure you also include a reference to System.Drawing.Common:
`dotnet add package System.Drawing.Common`

## Usage

```csharp
var inFile = "in.jpg";
var outFile = "out.jpg";
using var bmp = new Bitmap(file);
using var tjc = new MozJpegSharp.TJCompressor();
var compressed = tjc.Compress(bmp, MozJpegSharp.TJSubsamplingOption.Chrominance420, 75, MozJpegSharp.TJFlags.None);
File.WriteAllBytes(outFile, compressed);
```

## Credits

This is heavily based on [`Quamotion.TurboJpegWrapper`](https://github.com/quamotion/AS.TurboJpegWrapper). Most credits go to Quamotion. This project replaces the used libjpeg-turbo with mozjpeg to achieve higher compression rates.
