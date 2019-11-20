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
        private readonly float[,] noiseLarge;
        private readonly float[,] noiseMedium;
        private readonly float[,] noiseSmall;

        public SplatMap()
        {
            size = (int)(Map.MapData.MapSize * TerrainConfig.HeightMapDetail);
            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.R8, size, size, TerrainConfig.Textures.Length);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            noiseLarge = SimplexNoise.Noise.Calc2D(size, size, 0.005f);
            noiseMedium = SimplexNoise.Noise.Calc2D(size, size, 0.1f);
            noiseSmall = SimplexNoise.Noise.Calc2D(size, size, 0.05f);

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

        public void AutoPaint(HeightMap heightMap, NormalMap normalMap)
        {
            if (normalMap.HasChanged) normalMap.UpdateNormalArray();
            var normals = normalMap.Normals;
            var textureChanges = new bool[TerrainConfig.Textures.Length];

            for (var z = 0; z < size; z ++)
                for (var x = 0; x < size; x ++)
                    autoPaintPixel(ref textureChanges, normals, heightMap, x, z);

            for(var i = 0; i < TerrainConfig.Textures.Length; i++)
                if (textureChanges[i]) ToTexture(i);
        }

        public void AutoPaint(HeightMap heightMap, NormalMap normalMap, Vector3 mousePosition)
        {
            if (normalMap.HasChanged) normalMap.UpdateNormalArray();

            var normals = normalMap.Normals;
            var textureChanges = new bool[TerrainConfig.Textures.Length];
            var pos = ((mousePosition.Xz / Map.MapData.MapSize) + new Vector2(0.5f)) * size;
            var radius = (int)(State.ToolRadius * size / Map.MapData.MapSize);

            for (var z = (int)(pos.Y - radius); z < pos.Y + radius; z ++)
                for (var x = (int)(pos.X - radius); x < pos.X + radius; x ++)
                    if (Vector2.Distance(pos, new Vector2(x, z)) < State.ToolRadius / 2.0f)
                        autoPaintPixel(ref textureChanges, normals, heightMap, x, z);

            for(var i = 0; i < TerrainConfig.Textures.Length; i++)
                if (textureChanges[i]) ToTexture(i);
        }

        private void autoPaintPixel(ref bool[] hasChanged, Vector4[,] normals, HeightMap heightMap, int x, int z)
        {
            var old = new float[TerrainConfig.Textures.Length];

            for(var i = 0; i < TerrainConfig.Textures.Length; i++) {
                old[i] = Map.MapData.SplatMap[i][z, x];
                Map.MapData.SplatMap[i][z, x] = 0.0f;
            }

            var angle = 1.0f - normals[z, x].Y;
            var v = (noiseMedium[z, x] + noiseSmall[z, x]) / 512.0f;
            var height = heightMap.Heights[z, x] * TerrainConfig.HeightMapScale;
            var c = noiseLarge[z, x] / 256.0f * 5.0f - 2.5f;

            addSplatMix(height,-30.0f, 32.0f, 7, 8, x, z, v, c, 0.5f, 1.0f);
            addSplatMix(height, 92.0f, 94.0f, 0, 1, x, z, v, c, 0.1f, 0.6f);
            addSplatMix(angle, 0.32f, 0.12f, 5, 6, x, z, v, 0.0f, 0.5f, 1.0f, 15.0f);
            addSplatMix(angle, 0.50f, 0.18f, 11, 12, x, z, v, 0.0f, 0.5f, 1.0f, 15.0f);

            var split = 1.0f / Map.MapData.SplatMap.Sum(s => s[z, x]);
            for(var i = 0; i < TerrainConfig.Textures.Length; i++) {
                Map.MapData.SplatMap[i][z, x] *= split;
                if (old[i] != Map.MapData.SplatMap[i][z, x]) hasChanged[i] = true;
            }
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