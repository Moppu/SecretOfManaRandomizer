using System;
using System.IO;

namespace SoMRandomizer.util.img
{
    /// <summary>
    /// 24-bit BMP reader for processing imported images while avoiding including System.Drawing.
    /// </summary>
    public class Bitmap
    {
        private byte[] data = Array.Empty<byte>();
        private int width = 0;
        private int height = 0;
        private int stride = 0;
        private int bpp = 0;

        public Bitmap(MemoryStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var fileHeader = new byte[14];
            if (stream.Read(fileHeader, 0, fileHeader.Length) != fileHeader.Length)
            {
                throw new ArgumentException("Invalid file header");
            }

            if (fileHeader[0] != 'B' || fileHeader[1] != 'M')
            {
                throw new ArgumentException("Unsupported image format. Only bmp supported.");
            }

            var imagePtr = readU32(fileHeader, 10);

            var dibHeader = new byte[12];
            if (stream.Read(dibHeader, 0, dibHeader.Length) != dibHeader.Length)
            {
                throw new ArgumentException("Invalid DIB header");
            }
            var dibLen = readU32(dibHeader, 0);
            if (dibLen < dibHeader.Length)
            {
                throw new ArgumentException("Unsupported DIB header");
            }
            UInt32 w; 
            UInt32 h;
            UInt32 bpp;
            if (dibLen > dibHeader.Length)
            {
                var dibRest = new byte[dibLen - dibHeader.Length];
                if (stream.Read(dibRest, 0, dibRest.Length) != dibRest.Length)
                {
                    throw new ArgumentException("Invalid extended DIB header (" + dibLen + ")");
                }
                var compression = readU32(dibRest, 4);
                if (compression != 0)
                {
                    throw new ArgumentException("Unsupported bmp compression " + compression);
                }
                w = readU32(dibHeader, 4);
                h = readU32(dibHeader, 8);
                bpp = readU16(dibRest, 2);
            }
            else
            {
                w = readU16(dibHeader, 4);
                h = readU16(dibHeader, 6);
                bpp = readU16(dibHeader, 10);
            }

            if (w < 1 || w > int.MaxValue || h < 1)
            {
                throw new ArgumentException("Invalid image size");
            }
            if (h > Int32.MaxValue)
            {
                throw new ArgumentException("Only bottom-up bitmaps implemented");
            }
            if (bpp != 24)
            {
                throw new ArgumentException("Unsupported bpp " + bpp + ", only 24 bpp is supported.");
            }

            stream.Seek(imagePtr - fileHeader.Length - dibLen, SeekOrigin.Current);
            var stride = (bpp * w + 31) / 32 * 4;

            var data = new byte[h * stride];
            if (stream.Read(data, 0, data.Length) != data.Length)
            {
                throw new ArgumentException("Invalid pixel data");
            }
            
            this.stride = (int)stride;
            width = (int)w;
            height = (int)h;
            this.bpp = (int)bpp;
            this.data = data;
        }

        // ReSharper disable once InconsistentNaming
        public Color GetPixel(int x, int y)
        {
            if (y >= height)
            {
                throw new ArgumentException("y " + y + " is out of bounds");
            }
            if (x >= width)
            {
                throw new ArgumentException("x " + x + " is out of bounds");
            }

            var yb = height - y - 1; // bitmap is bottom up by default and only that is implemented
            var off = yb * stride + x * bpp / 8;  // only full bytes implemented
            return Color.FromArgb(data[off+2], data[off+1], data[off+0]); // only 24bit B,G,R implemented
        }

        private static UInt32 readU32(byte[] bytes, int offset)
        {
            UInt32 res = 0;
            res += bytes[offset + 3];
            res <<= 8;
            res += bytes[offset + 2];
            res <<= 8;
            res += bytes[offset + 1];
            res <<= 8;
            res += bytes[offset + 0];
            return res;
        }

        private static UInt16 readU16(byte[] bytes, int offset)
        {
            UInt16 res = 0;
            res += bytes[offset + 1];
            res <<= 8;
            res += bytes[offset + 0];
            return res;
        }
    }
}