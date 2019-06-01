using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class SplatMap
    {
        private const int detail = 1024;

        private byte[] id;
        private byte[] intensity;

        public int TextureId;
        public int TextureIntensity;


        public SplatMap()
        {
            id = new byte[detail * detail * 3];
            intensity = new byte[detail * detail * 3];

            TextureId = GL.GenTexture();
            TextureIntensity = GL.GenTexture();

            build();
        }

        private void build()
        {
            for (var i = 0; i < id.Length; i += 3) {
                id[i] = 0;
                intensity[i] = 255;
            }

            toTexture();
        }

        private unsafe void toTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba16, detail, detail);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, new[] { (int)TextureMinFilter.Linear });
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, new[] { (int)TextureMinFilter.Linear });

            fixed (byte* p = id)
            {
                var ptr = (IntPtr)p;
                GL.TextureSubImage2D(TextureId, 0, 0, 0, detail, detail, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
            }

            GL.BindTexture(TextureTarget.Texture2D, TextureIntensity);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba16, detail, detail);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, new[] { (int)TextureMinFilter.Linear });
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, new[] { (int)TextureMinFilter.Linear });

            fixed (byte* p = intensity)
            {
                var ptr = (IntPtr)p;
                GL.TextureSubImage2D(TextureIntensity, 0, 0, 0, detail, detail, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
            }
        }
    }
}