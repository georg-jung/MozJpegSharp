# MozJpegSharp (.NET wrapper for mozjpeg)
[![Build Status](https://dev.azure.com/georg-jung/MozJpegSharp/_apis/build/status/georg-jung.MozJpegSharp?branchName=master)](https://dev.azure.com/georg-jung/MozJpegSharp/_build/latest?definitionId=6&branchName=master)

> This is currently under active development and not yet ready to use.

MozJPEG reduces file sizes of JPEG images while retaining quality and compatibility with the vast majority of the world's deployed decoders.

This library provides .NET wrappers for mozjpeg, allowing you to use it from any .NET language,
such as C#.

For Windows (32-bit and 64-bit) and macOS (all versions), the NuGet package includes libjpeg-turbo. For Linux distributions, you need to install mozjpeg using your package manager (if available) or compile it from source.


## Installation

Install using the package manager console:

```
Install-Package MozJpegSharp
```

#### macOS - .NET Core
Make sure you also include a reference to System.Drawing.Common:
`dotnet add package System.Drawing.Common`

## Credits

This is heavily based on [`Quamotion.TurboJpegWrapper`](https://github.com/quamotion/AS.TurboJpegWrapper). Most credits go to Quamotion. This project replaces the used libjpeg-turbo with mozjpeg to achieve higher compression rates.
