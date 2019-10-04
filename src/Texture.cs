using System;
using System.Linq;
using System.IO;
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

        public byte[] imageToByteArray(string path)
        {
            var image = new System.Drawing.Bitmap(path);
            var pixelSize = image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 3 : 4;

            pixelFormat = pixelSize == 3 ? PixelFormat.Bgr : PixelFormat.Bgra;
            Size = new Point(image.Width, image.Height);

            var buffer = new byte[image.Width * image.Height * pixelSize];
            using(var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(BitmapHeaderLength, SeekOrigin.Begin);
                ms.Read(buffer, 0, image.Width * image.Height * pixelSize);

                InvertRows(buffer, image.Height, image.Width * pixelSize);
                return buffer;
            }
        }

        public void LoadTexture(string path, bool mipMap = false)
        {
            var buffer = imageToByteArray(path);

            TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Size.X, Size.Y, 0, pixelFormat, PixelType.UnsignedByte, buffer);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, mipMap ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear);

            if (mipMap) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void LoadTexture(string[] path, bool mipMap = false)
        {
            var buffers = path.Select(p => imageToByteArray(p)).ToList();

            TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, TextureId);

            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 8, SizedInternalFormat.Rgba32f, Size.X, Size.Y, buffers.Count());

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, mipMap ? (int)TextureMinFilter.LinearMipmapLinear : (int)TextureMinFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            for (var i = 0; i < buffers.Count(); i ++)
            {
                GL.TexSubImage3D<byte>(TextureTarget.Texture2DArray, 0, 0, 0, i, Size.X, Size.Y, 1, pixelFormat, PixelType.UnsignedByte, buffers[i]);
            }

            if (mipMap) GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}