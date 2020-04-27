using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using SkiaSharp;

namespace MozJpegSharp.SkiaSharp
{
    public static class Extensions
    {
        public static byte[] Compress(this TJCompressor @this, SKBitmap srcImage, TJSubsamplingOption subSamp, int quality, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            _ = srcImage ?? throw new ArgumentNullException(nameof(srcImage));

            var tjPixelFormat = TJSkiaUtils.ConvertPixelFormat(srcImage.ColorType);

            var width = srcImage.Width;
            var height = srcImage.Height;

            var srcPtr = srcImage.GetPixels();
            var stride = srcImage.RowBytes;

            return @this.Compress(srcPtr, stride, width, height, tjPixelFormat, subSamp, quality, flags);
        }

        /// <summary>
        /// Decompress a JPEG image.
        /// </summary>
        /// <param name="jpegBuf">Pointer to a buffer containing the JPEG image to decompress. This buffer is not modified.</param>
        /// <param name="jpegBufSize">Size of the JPEG image (in bytes).</param>
        /// <param name="destPixelFormat">Pixel format of the destination image (see <see cref="SKColorType"/> "Pixel formats".)</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>Decompressed image of specified format.</returns>
        /// <exception cref="TJException">Throws if underlying decompress function failed.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">Convertion to the requested pixel format can not be performed.</exception>
        public static SKBitmap Decompress(this TJDecompressor @this, IntPtr jpegBuf, ulong jpegBufSize, SKColorType destPixelFormat, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            
            var targetFormat = TJSkiaUtils.ConvertPixelFormat(destPixelFormat);

            @this.GetImageInfo(jpegBuf, jpegBufSize, targetFormat, out var width, out var height, out var stride, out var outBufSize);

            var info = new SKImageInfo(width, height, destPixelFormat);
            if (info.RowBytes != stride)
                throw new NotSupportedException($"Skia expected the RowBytes/stride to be {info.RowBytes} for the given parameters but MozJPEG returns {stride}. Those values need to be equal.");
            if (info.BytesSize != outBufSize)
                throw new NotSupportedException($"Skia expected the BytesSize/number of bytes required to store the bitmap data to be {info.BytesSize} for the given parameters but MozJPEG returns {outBufSize}. Those values need to be equal.");

            var dst = new SKBitmap(info);
            var dstPtr = dst.GetPixels();

            @this.Decompress(jpegBuf, jpegBufSize, dstPtr, outBufSize, targetFormat, flags, out _, out _, out _);
            return dst;
        }

        /// <summary>
        /// Decompress a JPEG image.
        /// </summary>
        /// <param name="jpegBuf">A buffer containing the JPEG image to decompress. This buffer is not modified.</param>
        /// <param name="destPixelFormat">Pixel format of the destination image (see <see cref="SKColorType"/> "Pixel formats".)</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>Decompressed image of specified format.</returns>
        /// <exception cref="TJException">Throws if underlying decompress function failed.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">Convertion to the requested pixel format can not be performed.</exception>
        public static SKBitmap Decompress(this TJDecompressor @this, byte[] jpegBuf, SKColorType destPixelFormat, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            _ = jpegBuf ?? throw new ArgumentNullException(nameof(jpegBuf));

            // see https://stackoverflow.com/questions/537573/how-to-get-intptr-from-byte-in-c-sharp
            var pinnedArray = GCHandle.Alloc(jpegBuf, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                var jpegBufSize = (ulong)jpegBuf.Length;
                return @this.Decompress(pointer, jpegBufSize, destPixelFormat, flags);
            }
            finally
            {
                pinnedArray.Free();
            }
        }

        /// <summary>
        /// Decompress a JPEG image.
        /// </summary>
        /// <param name="jpegBuf">A buffer containing the JPEG image to decompress. This buffer is not modified.</param>
        /// <param name="destPixelFormat">Pixel format of the destination image (see <see cref="SKColorType"/> "Pixel formats".)</param>
        /// <param name="flags">The bitwise OR of one or more of the <see cref="TJFlags"/> "flags".</param>
        /// <returns>Decompressed image of specified format.</returns>
        /// <exception cref="TJException">Throws if underlying decompress function failed.</exception>
        /// <exception cref="ObjectDisposedException">Object is disposed and can not be used anymore.</exception>
        /// <exception cref="NotSupportedException">Convertion to the requested pixel format can not be performed.</exception>
        public static SKBitmap Decompress(this TJDecompressor @this, ReadOnlySpan<byte> jpegBuf, SKColorType destPixelFormat, TJFlags flags)
        {
            _ = @this ?? throw new ArgumentNullException(nameof(@this));
            
            // see https://stackoverflow.com/questions/537573/how-to-get-intptr-from-byte-in-c-sharp
            var pinnedArray = GCHandle.Alloc(jpegBuf.GetPinnableReference(), GCHandleType.Pinned);
            try
            {
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                var jpegBufSize = (ulong)jpegBuf.Length;
                return @this.Decompress(pointer, jpegBufSize, destPixelFormat, flags);
            }
            finally
            {
                pinnedArray.Free();
            }
        }

    }
}
