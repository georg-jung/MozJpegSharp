﻿// <copyright file="TJCompressor.cs" company="Autonomic Systems, Quamotion">
// Copyright (c) Autonomic Systems. All rights reserved.
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MozJpegSharp
{
    // ReSharper disable once InconsistentNaming

    /// <summary>
    /// Implements compression of RGB, CMYK, grayscale images to the jpeg format.
    /// </summary>
    public class TJCompressor : IDisposable
    {
        private readonly object @lock = new object();
        private IntPtr compressorHandle;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TJCompressor"/> class.
        /// </summary>
        /// <exception cref="TJException">
        /// Throws if internal compressor instance can not be created.
        /// </exception>
        public TJCompressor()
        {
            this.compressorHandle = TurboJpegImport.TjInitCompress();

            if (this.compressorHandle == IntPtr.Zero)
            {
                TJUtils.GetErrorAndThrow();
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TJCompressor"/> class.
        /// </summary>
        ~TJCompressor()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Compresses input image to the jpeg format with specified quality.
        /// </summary>
        /// <param name="srcImage"> Source image to be converted. </param>
        /// <param name="subSamp">
        /// The level of chrominance subsampling to be used when
        /// generating the JPEG image (see <see cref="TJSubsamplingOption"/> "Chrominance subsampling options".)
        /// </param>
        /// <param name="quality">The image quality of the generated JPEG image (1 = worst, 100 = best).</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>
        /// A <see cref="byte"/> array containing the compressed image.
        /// </returns>
        /// <remarks>Only <see cref="PixelFormat.Format24bppRgb"/>, <see cref="PixelFormat.Format32bppArgb"/>, <see cref="PixelFormat.Format8bppIndexed"/> pixel formats are supported.</remarks>
        /// <exception cref="TJException"> Throws if compress function failed. </exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">
        /// Some parameters' values are incompatible:
        /// <list type="bullet">
        /// <item><description>Subsampling not equals to <see cref="TJSubsamplingOption.Gray"/> and pixel format <see cref="TJPixelFormat.Gray"/></description></item>
        /// </list>
        /// </exception>
        public byte[] Compress(Bitmap srcImage, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("this");
            }

            var pixelFormat = srcImage.PixelFormat;

            var width = srcImage.Width;
            var height = srcImage.Height;
            var srcData = srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, pixelFormat);

            var stride = srcData.Stride;
            var srcPtr = srcData.Scan0;

            try
            {
                return this.Compress(srcPtr, stride, width, height, pixelFormat, subSamp, quality, flags);
            }
            finally
            {
                srcImage.UnlockBits(srcData);
            }
        }

        /// <summary>
        /// Compresses input image to the jpeg format with specified quality.
        /// </summary>
        /// <param name="srcPtr">
        /// Pointer to an image buffer containing RGB, grayscale, or CMYK pixels to be compressed.
        /// This buffer is not modified.
        /// </param>
        /// <param name="stride">
        /// Bytes per line in the source image.
        /// Normally, this should be <c>width * BytesPerPixel</c> if the image is unpadded,
        /// or <c>TJPAD(width * BytesPerPixel</c> if each line of the image
        /// is padded to the nearest 32-bit boundary, as is the case for Windows bitmaps.
        /// You can also be clever and use this parameter to skip lines, etc.
        /// Setting this parameter to 0 is the equivalent of setting it to
        /// <c>width * BytesPerPixel</c>.
        /// </param>
        /// <param name="width">Width (in pixels) of the source image.</param>
        /// <param name="height">Height (in pixels) of the source image.</param>
        /// <param name="pixelFormat">Pixel format of the source image (see <see cref="PixelFormat"/> "Pixel formats").</param>
        /// <param name="subSamp">
        /// The level of chrominance subsampling to be used when
        /// generating the JPEG image (see <see cref="TJSubsamplingOption"/> "Chrominance subsampling options".)
        /// </param>
        /// <param name="quality">The image quality of the generated JPEG image (1 = worst, 100 = best).</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>
        /// A <see cref="byte"/> array containing the compressed image.
        /// </returns>
        /// <exception cref="TJException"> Throws if compress function failed. </exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">
        /// Some parameters' values are incompatible:
        /// <list type="bullet">
        /// <item><description>Subsampling not equals to <see cref="TJSubsamplingOption.Gray"/> and pixel format <see cref="TJPixelFormat.Gray"/></description></item>
        /// </list>
        /// </exception>
        public byte[] Compress(IntPtr srcPtr, int stride, int width, int height, PixelFormat pixelFormat, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("this");
            }

            var tjPixelFormat = TJUtils.ConvertPixelFormat(pixelFormat);
            CheckOptionsCompatibilityAndThrow(subSamp, tjPixelFormat);

            var buf = IntPtr.Zero;
            ulong bufSize = 0;
            try
            {
                var result = TurboJpegImport.TjCompress2(
                    this.compressorHandle,
                    srcPtr,
                    width,
                    stride,
                    height,
                    (int)tjPixelFormat,
                    ref buf,
                    ref bufSize,
                    (int)subSamp,
                    quality,
                    (int)flags);

                if (result == -1)
                {
                    TJUtils.GetErrorAndThrow();
                }

                var jpegBuf = new byte[bufSize];
                Marshal.Copy(buf, jpegBuf, 0, (int)bufSize);
                return jpegBuf;
            }
            finally
            {
                TurboJpegImport.TjFree(buf);
            }
        }

        /// <summary>
        /// Compresses input image to the jpeg format with specified quality.
        /// </summary>
        /// <param name="srcBuf">
        /// Image buffer containing RGB, grayscale, or CMYK pixels to be compressed.
        /// This buffer is not modified.
        /// </param>
        /// <param name="stride">
        /// Bytes per line in the source image.
        /// Normally, this should be <c>width * BytesPerPixel</c> if the image is unpadded,
        /// or <c>TJPAD(width * BytesPerPixel</c> if each line of the image
        /// is padded to the nearest 32-bit boundary, as is the case for Windows bitmaps.
        /// You can also be clever and use this parameter to skip lines, etc.
        /// Setting this parameter to 0 is the equivalent of setting it to
        /// <c>width * BytesPerPixel</c>.
        /// </param>
        /// <param name="width">Width (in pixels) of the source image.</param>
        /// <param name="height">Height (in pixels) of the source image.</param>
        /// <param name="pixelFormat">Pixel format of the source image (see <see cref="PixelFormat"/> "Pixel formats").</param>
        /// <param name="subSamp">
        /// The level of chrominance subsampling to be used when
        /// generating the JPEG image (see <see cref="TJSubsamplingOption"/> "Chrominance subsampling options".)
        /// </param>
        /// <param name="quality">The image quality of the generated JPEG image (1 = worst, 100 = best).</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>
        /// A <see cref="byte"/> array containing the compressed image.
        /// </returns>
        /// <exception cref="TJException">
        /// Throws if compress function failed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">
        /// Some parameters' values are incompatible:
        /// <list type="bullet">
        /// <item><description>Subsampling not equals to <see cref="TJSubsamplingOption.Gray"/> and pixel format <see cref="TJPixelFormat.Gray"/></description></item>
        /// </list>
        /// </exception>
        public unsafe byte[] Compress(byte[] srcBuf, int stride, int width, int height, PixelFormat pixelFormat, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("this");
            }

            var tjPixelFormat = TJUtils.ConvertPixelFormat(pixelFormat);
            CheckOptionsCompatibilityAndThrow(subSamp, tjPixelFormat);

            var buf = IntPtr.Zero;
            ulong bufSize = 0;
            try
            {
                fixed (byte* srcBufPtr = srcBuf)
                {
                    var result = TurboJpegImport.TjCompress2(
                        this.compressorHandle,
                        (IntPtr)srcBufPtr,
                        width,
                        stride,
                        height,
                        (int)tjPixelFormat,
                        ref buf,
                        ref bufSize,
                        (int)subSamp,
                        quality,
                        (int)flags);
                    if (result == -1)
                    {
                        TJUtils.GetErrorAndThrow();
                    }
                }

                var jpegBuf = new byte[bufSize];
                Marshal.Copy(buf, jpegBuf, 0, (int)bufSize);
                return jpegBuf;
            }
            finally
            {
                TurboJpegImport.TjFree(buf);
            }
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        /// <filterpriority>2.</filterpriority>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            lock (this.@lock)
            {
                if (this.isDisposed)
                {
                    return;
                }

                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <exception cref="NotSupportedException">
        /// Some parameters' values are incompatible:
        /// <list type="bullet">
        /// <item><description>Subsampling not equals to <see cref="TJSubsamplingOption.Gray"/> and pixel format <see cref="TJPixelFormat.Gray"/></description></item>
        /// </list>
        /// </exception>
        private static void CheckOptionsCompatibilityAndThrow(TJSubsamplingOption subSamp, TJPixelFormat srcFormat)
        {
            if (srcFormat == TJPixelFormat.Gray && subSamp != TJSubsamplingOption.Gray)
            {
                throw new NotSupportedException(
                    $"Subsampling differ from {TJSubsamplingOption.Gray} for pixel format {TJPixelFormat.Gray} is not supported");
            }
        }

        protected virtual void Dispose(bool callFromUserCode)
        {
            if (callFromUserCode)
            {
                this.isDisposed = true;
            }

            // If for whathever reason, the handle was not initialized correctly (e.g. an exception
            // in the constructor), we shouldn't free it either.
            if (this.compressorHandle != IntPtr.Zero)
            {
                TurboJpegImport.TjDestroy(this.compressorHandle);

                // Set the handle to IntPtr.Zero, to prevent double execution of this method
                // (i.e. make calling Dispose twice a safe thing to do).
                this.compressorHandle = IntPtr.Zero;
            }
        }
    }
}
