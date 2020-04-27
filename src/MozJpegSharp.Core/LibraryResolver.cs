// <copyright file="LibraryResolver.cs" company="Autonomic Systems, Quamotion">
// Copyright (c) Autonomic Systems. All rights reserved.
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MozJpegSharp
{
    internal static class LibraryResolver
    {
        static LibraryResolver()
        {
#if !NETSTANDARD2_0 && !NETSTANDARD2_1 && !NET461
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
#endif
        }

        public static void EnsureRegistered()
        {
            // Dummy call to trigger the static constructor
        }

#if !NETSTANDARD2_0 && !NETSTANDARD2_1 && !NET461
        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // upstream, this contains more complex logic
            // for MozJpegSharp we include all the native libs as part
            // of the nupkg so theres nothing left to do here
            return IntPtr.Zero;
        }
#endif
    }
}