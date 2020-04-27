using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MozJpegSharp
{
    public static class Extensions
    {
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
        public static byte[] Compress(this TJCompressor @this, Bitmap srcImage, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            var pixelFormat = srcImage.PixelFormat;

            var width = srcImage.Width;
            var height = srcImage.Height;
            var srcData = srcImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, pixelFormat);

            var stride = srcData.Stride;
            var srcPtr = srcData.Scan0;

            try
            {
                return @this.Compress(srcPtr, stride, width, height, pixelFormat, subSamp, quality, flags);
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
        public static byte[] Compress(this TJCompressor @this, IntPtr srcPtr, int stride, int width, int height, PixelFormat pixelFormat,
            TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            var tjPixelFormat = TJGdiPlusUtils.ConvertPixelFormat(pixelFormat);
            return @this.Compress(srcPtr, stride, width, height, tjPixelFormat, subSamp, quality, flags);
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
        public static unsafe byte[] Compress(this TJCompressor @this, byte[] srcBuf, int stride, int width, int height, PixelFormat pixelFormat,
            TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            var tjPixelFormat = TJGdiPlusUtils.ConvertPixelFormat(pixelFormat);
            return @this.Compress(srcBuf, stride, width, height, tjPixelFormat, subSamp, quality, flags);
        }

        /// <summary>
        /// Compresses input image to the jpeg format with specified quality.
        /// </summary>
        /// <param name="srcBuf">
        /// Image buffer containing RGB, grayscale, or CMYK pixels to be compressed.
        /// This buffer is not modified.
        /// </param>
        /// <param name="destBuf">
        /// A <see cref="byte"/> array containing the compressed image.
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
        /// The size of the JPEG image (in bytes).
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
        public static unsafe int Compress(this TJCompressor @this, byte[] srcBuf, byte[] destBuf, int stride, int width, int height, PixelFormat pixelFormat, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            return @this.Compress(srcBuf.AsSpan(), destBuf.AsSpan(), stride, width, height, pixelFormat, subSamp, quality, flags).Length;
        }

        /// <summary>
        /// Compresses input image to the jpeg format with specified quality.
        /// </summary>
        /// <param name="srcBuf">
        /// Image buffer containing RGB, grayscale, or CMYK pixels to be compressed.
        /// This buffer is not modified.
        /// </param>
        /// <param name="destBuf">
        /// A <see cref="byte"/> array containing the compressed image.
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
        /// A <see cref="Span{T}"/> which is a slice of <paramref name="destBuf"/> which holds the compressed image.
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
        public static unsafe Span<byte> Compress(this TJCompressor @this, Span<byte> srcBuf, Span<byte> destBuf, int stride, int width, int height, PixelFormat pixelFormat, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            var tjPixelFormat = TJGdiPlusUtils.ConvertPixelFormat(pixelFormat);
            return @this.Compress(srcBuf, destBuf, stride, width, height, tjPixelFormat, subSamp, quality, flags);
        }

        /// <summary>
        /// Decompress a JPEG image to an RGB, grayscale, or CMYK image.
        /// </summary>
        /// <param name="jpegBuf">Pointer to a buffer containing the JPEG image to decompress. This buffer is not modified.</param>
        /// <param name="jpegBufSize">Size of the JPEG image (in bytes).</param>
        /// <param name="destPixelFormat">Pixel format of the destination image (see <see cref="PixelFormat"/> "Pixel formats".)</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>Decompressed image of specified format.</returns>
        /// <exception cref="TJException">Throws if underlying decompress function failed.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">Convertion to the requested pixel format can not be performed.</exception>
        public static unsafe Bitmap Decompress(this TJDecompressor @this, IntPtr jpegBuf, ulong jpegBufSize, PixelFormat destPixelFormat, TJFlags flags)
        {
            var targetFormat = TJGdiPlusUtils.ConvertPixelFormat(destPixelFormat);
            int width;
            int height;
            int stride;
            var buffer = @this.Decompress(jpegBuf, jpegBufSize, targetFormat, flags, out width, out height, out stride);
            Bitmap result;
            fixed (byte* bufferPtr = buffer)
            {
                result = new Bitmap(width, height, stride, destPixelFormat, (IntPtr)bufferPtr);
                if (destPixelFormat == PixelFormat.Format8bppIndexed)
                {
                    result.Palette = TJGdiPlusUtils.FixPaletteToGrayscale(result.Palette);
                }
            }

            return result;
        }

        /// <summary>
        /// Decompress a JPEG image to an RGB, grayscale, or CMYK image.
        /// </summary>
        /// <param name="jpegBuf">A buffer containing the JPEG image to decompress. This buffer is not modified.</param>
        /// <param name="destPixelFormat">Pixel format of the destination image (see <see cref="PixelFormat"/> "Pixel formats".)</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>Decompressed image of specified format.</returns>
        /// <exception cref="TJException">Throws if underlying decompress function failed.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">Convertion to the requested pixel format can not be performed.</exception>
        public static unsafe Bitmap Decompress(this TJDecompressor @this, byte[] jpegBuf, PixelFormat destPixelFormat, TJFlags flags)
        {
            var jpegBufSize = (ulong)jpegBuf.Length;
            fixed (byte* jpegPtr = jpegBuf)
            {
                return @this.Decompress((IntPtr)jpegPtr, jpegBufSize, destPixelFormat, flags);
            }
        }
    }
}
