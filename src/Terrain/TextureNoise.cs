using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TextureNoise
    {
        private const int Detail = 1024;

        public int Texture;

        public TextureNoise(int seed)
        {
            build();
        }

        private void build()
        {
            var noise = SimplexNoise.Noise.Calc2D(Detail, Detail, 0.05f);
            for (var x = 0; x < Detail; x++)
                for (var z = 0; z < Detail; z++)
                    noise[x, z] /= 255.0f;

            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexImage2D<float>(TextureTarget.Texture2D, 0, PixelInternalFormat.R16f, Detail, Detail, 0, PixelFormat.Red, PixelType.Float, noise);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}