using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class SplatMap
    {
        public const int Detail = 1024;
        public readonly int SplatCount;
        private float[][,] splats;
        public int Texture;

        public SplatMap(int textureCount)
        {
            SplatCount = textureCount;
            splats = new float[SplatCount][,];

            for(var i = 0; i < SplatCount; i++) splats[i] = new float[Detail, Detail];

            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.R8, Detail, Detail, SplatCount);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            build();
        }

        private void build()
        {
            for (var z = 0; z < Detail; z ++)
                for (var x = 0; x < Detail; x ++)
                    splats[0][z, x] = 1.0f;

            for(var i = 0; i < SplatCount; i++) toTexture(i);
        }

        public void Update(Vector2 pos, float radius, byte splatId)
        {
            Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

            for (var z1 = (int)(pos.Y - radius); z1 < pos.Y + radius; z1 ++)
                for (var x1 = (int)(pos.X - radius); x1 < pos.X + radius; x1 ++)
                {
                    if (x1 >= Detail || x1 < 0 || z1 >= Detail || z1 < 0) continue;

                    var distance = Vector2.Distance(pos, new Vector2(x1, z1));
                    if (distance > radius) continue;

                    var n = calcP(MathF.Min(1.0f, MathF.Sqrt((distance / radius > State.ToolHardness ? distance : 0.0f) / radius)));
                    var result = splats[splatId][z1, x1] + n;

                    splats[splatId][z1, x1] += result > 1.0f ? 1.0f : result;
                    average(z1, x1);
                }

            for(var i = 0; i < SplatCount; i++) toTexture(i);
        }

        private void average(int z, int x)
        {
            var split = 1.0f / splats.Sum(s => s[z, x]);
            foreach(var splat in splats) {
                splat[z, x] *= split;
            }
        }

        private void toTexture(int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexSubImage3D<float>(TextureTarget.Texture2DArray, 0, 0, 0, textureId, Detail, Detail, 1, PixelFormat.Red, PixelType.Float, splats[textureId]);
        }
    }
}