using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class SplatMap
    {
        public const int Detail = 1024;

        private byte[,] id;
        private byte[,] intensity;

        public int TextureId;
        public int TextureIntensity;


        public SplatMap()
        {
            id = new byte[Detail, Detail * 3];
            intensity = new byte[Detail, Detail * 3];

            TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba16ui, Detail, Detail);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new[] { (int)TextureMagFilter.Nearest });
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new[] { (int)TextureMinFilter.Nearest });

            TextureIntensity = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureIntensity);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba16, Detail, Detail);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new[] { (int)TextureMagFilter.Nearest });
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new[] { (int)TextureMinFilter.Nearest });

            build();
        }

        private void build()
        {
            for (var z = 0; z < Detail; z ++)
                for (var x = 0; x < Detail * 3; x += 3)
                {
                    id[z, x] = 0;
                    intensity[z, x] = 255;
                }

            toTexture();
        }

        public void Update(Vector2 pos, float radius, byte textureId)
        {
            Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

            for (var z1 = pos.Y - radius; z1 < pos.Y + radius; z1 ++)
                for (var x1 = pos.X - radius; x1 < pos.X + radius; x1 ++)
                {
                    if (x1 >= Detail || x1 < 0 || z1 >= Detail || z1 < 0) continue;

                    var amount = Vector2.Distance(pos, new Vector2(x1, z1));
                    var n = calcP(MathF.Min(1f, MathF.Sqrt((amount / radius > State.ToolHardness ? amount : 0.0f) / radius)));

                    var freeSlot = findFreeSlot((int)z1, (int)x1 * 3, textureId);
                    var result = intensity[(int)z1, (int)x1 * 3 + freeSlot] + (n * 255);

                    intensity[(int)z1, (int)x1 * 3 + freeSlot] = (byte)(result > 255 ? 255 : result);

                    var intensities = new Vector3(intensity[(int)z1, (int)x1 * 3], intensity[(int)z1, (int)x1 * 3 + 1], intensity[(int)z1, (int)x1 * 3 + 2]);
                    var avg = 255.0f / (intensities.X + intensities.Y + intensities.Z);

                    id[(int)z1, (int)x1 * 3 + freeSlot] = textureId;

                    intensity[(int)z1, (int)x1 * 3] = (byte)(intensities.X * avg);
                    intensity[(int)z1, (int)x1 * 3 + 1] = (byte)(intensities.Y * avg);
                    intensity[(int)z1, (int)x1 * 3 + 2] = (byte)(intensities.Z * avg);
                }

            toTexture();
        }

        private int findFreeSlot(int z, int x, byte textureId)
        {
            if (id[z, x] == textureId) return 0;
            if (id[z, x + 1] == textureId) return 1;
            if (id[z, x + 2] == textureId) return 2;

            var items = new [] { intensity[z, x], intensity[z, x + 1], intensity[z, x + 2] };
            return Array.IndexOf(items, items.Min());
        }

        private unsafe void toTexture()
        {
            var idArr = toOneDimensional(id);
            var intensityArr = toOneDimensional(intensity);

            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            fixed (byte* p = idArr)
            {
                var ptr = (IntPtr)p;
                GL.TextureSubImage2D(TextureId, 0, 0, 0, Detail, Detail, PixelFormat.RgbInteger, PixelType.UnsignedByte, ptr);
            }

            GL.BindTexture(TextureTarget.Texture2D, TextureIntensity);
            fixed (byte* p = intensityArr)
            {
                var ptr = (IntPtr)p;
                GL.TextureSubImage2D(TextureIntensity, 0, 0, 0, Detail, Detail, PixelFormat.Rgb, PixelType.UnsignedByte, ptr);
            }
        }

        private byte[] toOneDimensional(byte[,] input)
        {
            byte[] output = new byte[Detail * Detail * 3];
            System.Buffer.BlockCopy(input, 0, output, 0, output.Length);
            return output;
        }
    }
}