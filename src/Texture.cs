using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace Larx
{
    public class Texture
    {
        private const int BitmapHeaderLength = 54;

        private PixelFormat pixelFormat;
        public Point Size { get; private set; }
        public int TextureId { get; private set; }

        private static void InvertRows(byte[] imageData, int rowCount, int bytesPerRow)
        {
            byte[] topRow = new byte[bytesPerRow];
            byte[] bottomRow = new byte[bytesPerRow];
            for (int i = 0; i < rowCount / 2; i++)
            {
                int topIndex = i * bytesPerRow;
                int bottomIndex = (rowCount - i - 1) * bytesPerRow;
                Array.Copy(imageData, topIndex, topRow, 0, bytesPerRow);
                Array.Copy(imageData, bottomIndex, bottomRow, 0, bytesPerRow);
                Array.Copy(topRow, 0, imageData, bottomIndex, bytesPerRow);
                Array.Copy(bottomRow, 0, imageData, topIndex, bytesPerRow);
            }
        }

        public unsafe void LoadTexture(string path, bool mipMap = false)
        {
            var imageStream = readFile(path);

            // Read bitmap header
            var header = new byte[BitmapHeaderLength];
            imageStream.Read(header, 0, BitmapHeaderLength);
            var start = parseHeader(header);

            // Read bitmap data
            var pixelSize = pixelFormat == PixelFormat.Bgr ? 3 : 4;
            var buffer = new byte[Size.X * Size.Y * pixelSize];
            imageStream.Seek(start, SeekOrigin.Begin);
            imageStream.Read(buffer, 0, Size.X * Size.Y * pixelSize);

            InvertRows(buffer, Size.Y, Size.X * pixelSize);
            if (pixelFormat == PixelFormat.Bgra)
                reorganizeBuffer(buffer);

            fixed (byte* p = buffer)
            {
                var ptr = (IntPtr)p;

                TextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, TextureId);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0, pixelFormat, PixelType.UnsignedByte, ptr);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new[] { (int)TextureMinFilter.Linear });
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new[] { mipMap ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear });

                if (mipMap) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        private FileStream readFile(string path)
        {
            return File.OpenRead(path);
        }

        private int parseHeader(byte[] header)
        {
            var fileType = Encoding.ASCII.GetString(header.Take(2).ToArray());
            if (fileType != "BM")
                throw new ApplicationException($"Texture has invalid file type, expected BM got {fileType}!");

            var format = BitConverter.ToInt32(header, 30);
            if (format != 0 && format != 3)
                throw new ApplicationException("Unsupported bitmap format!");

            pixelFormat = format == 0 ? PixelFormat.Bgr : PixelFormat.Bgra;
            Size = new Point(BitConverter.ToInt32(header, 18), BitConverter.ToInt32(header, 22));

            return BitConverter.ToInt32(header, 10); // Start of image data
        }

        private void reorganizeBuffer(byte[] buffer)
        {
            for (var i = 0; i < buffer.Count() - 4; i += 4)
            {
                var src = buffer.Skip(i).Take(4).ToArray();
                buffer[i] = src[1];
                buffer[i + 1] = src[2];
                buffer[i + 2] = src[3];
                buffer[i + 3] = src[0];
            }
        }
    }
}