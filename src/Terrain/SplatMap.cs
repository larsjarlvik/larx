using OpenTK.Graphics.OpenGL;
using Larx.Storage;
using OpenTK;
using Larx.Terrain.Shaders;
using System;
using System.Linq;

namespace Larx.Terrain
{
    public class SplatMap
    {
        private readonly int size;
        public readonly int Texture;

        public SplatMap()
        {
            size = (int)(Map.MapData.MapSize * TerrainConfig.HeightMapDetail);
            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.R8, size, size, TerrainConfig.Textures.Length);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            build();
            Refresh();
        }

        private void build()
        {
            for (var z = 0; z < size; z ++)
                for (var x = 0; x < size; x ++)
                    Map.MapData.SplatMap[0][z, x] = 1.0f;
        }

        public void Refresh()
        {
            for(var n = 0; n < TerrainConfig.Textures.Length; n ++)
                ToTexture(n);
        }

        public void Paint(Vector3 mousePosition, byte splatId)
        {
            var position = (mousePosition.Xz / Map.MapData.MapSize);
            position.X = (position.X + 0.5f) * size;
            position.Y = (position.Y + 0.5f) * size;

            update(position, splatId);
        }

        private float calcP(float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

        private void update(Vector2 pos, byte splatId)
        {
            var radius = (int)(State.ToolRadius * size / Map.MapData.MapSize);
            var hasChanged = new bool[TerrainConfig.Textures.Length];

            for (var z1 = (int)(pos.Y - radius); z1 < pos.Y + radius; z1 ++)
                for (var x1 = (int)(pos.X - radius); x1 < pos.X + radius; x1 ++)
                {
                    if (x1 >= size || x1 < 0 || z1 >= size || z1 < 0) continue;

                    var distance = Vector2.Distance(pos, new Vector2(x1, z1));
                    if (distance > radius) continue;

                    var n = calcP(MathF.Min(1.0f, MathF.Sqrt((distance / radius > (State.ToolHardness * 0.1f) ? distance : 0.0f) / radius)));
                    var result = Map.MapData.SplatMap[splatId][z1, x1] + n;

                    Map.MapData.SplatMap[splatId][z1, x1] = result > 1.0f ? 1.0f : result;
                    average(ref hasChanged, z1, x1);
                }

            for(var i = 0; i < TerrainConfig.Textures.Length; i++)
                if (hasChanged[i]) ToTexture(i);
        }

        private void average(ref bool[] hasChanged, int z, int x)
        {
            var split = 1.0f / Map.MapData.SplatMap.Sum(s => s[z, x]);

            for(var i = 0; i < TerrainConfig.Textures.Length; i++) {
                Map.MapData.SplatMap[i][z, x] *= split;
                if ((split > 1.02f || split < 0.98f) && Map.MapData.SplatMap[i][z, x] > 0.0f)
                    hasChanged[i] = true;
            }
        }

        public void ToTexture(int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexSubImage3D<float>(TextureTarget.Texture2DArray, 0, 0, 0, textureId, size, size, 1, PixelFormat.Red, PixelType.Float, Map.MapData.SplatMap[textureId]);
        }

        public void AutoGenerate(HeightMap heightMap, NormalMap normalMap)
        {
            var normals = normalMap.GetNormals();
            var n1 = SimplexNoise.Noise.Calc2D(size, size, 0.01f);
            var n2 = SimplexNoise.Noise.Calc2D(size, size, 0.05f);
            var n3 = SimplexNoise.Noise.Calc2D(size, size, 0.1f);
            var c1 = SimplexNoise.Noise.Calc2D(size, size, 0.005f);

            for (var z = 1; z < size; z ++)
                for (var x = 1; x < size; x ++) {
                    for(var i = 0; i < TerrainConfig.Textures.Length; i++)
                        Map.MapData.SplatMap[i][z, x] = 0.0f;

                    var angle = 1.0f - normals[z, x].Y;
                    var v = (n1[z, x] + n2[z, x]) / 512.0f;
                    var height = heightMap.Heights[z, x] * TerrainConfig.HeightMapScale;
                    var c = c1[z, x] / 256.0f * 5.0f - 2.5f;

                    addSplatMix(height,-30.0f, 32.0f, 7, 8, x, z, v, c, 0.5f, 1.0f);
                    addSplatMix(height, 62.0f, 64.0f, 0, 1, x, z, v, c, 0.1f, 0.6f);
                    addSplatMix(angle, 0.32f, 0.12f, 5, 6, x, z, v, 0.0f, 0.5f, 1.0f, 15.0f);
                    addSplatMix(angle, 0.40f, 0.08f, 11, 12, x, z, v, 0.0f, 0.5f, 1.0f, 15.0f);

                    var split = 1.0f / Map.MapData.SplatMap.Sum(s => s[z, x]);
                    for(var i = 0; i < TerrainConfig.Textures.Length; i++) {
                        Map.MapData.SplatMap[i][z, x] *= split;
                    }
                }

            Refresh();

        }

        private void addSplatMix(float height, float center, float spread, int s1, int s2, int x, int z, float v, float c, float strength, float mix = 1.0f, float multiplier = 1.0f)
        {
            var distance = MathF.Abs(height + c - center);
            var n = calcP(MathF.Min(1.0f, MathF.Sqrt((distance / spread > strength ? distance : 0.0f) / spread)));

            Map.MapData.SplatMap[s1][z, x] = v / mix * n * multiplier;
            Map.MapData.SplatMap[s2][z, x] = (1.0f - v * mix) * n * multiplier;
        }
    }
}