using System;
using System.Collections.Generic;
using Larx.Storage;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class HeightMap
    {
        public int Texture;
        private int size;
        public float[,] Heights;
        private readonly NormalMap normalMap;

        public HeightMap(NormalMap normalMap)
        {
            this.normalMap = normalMap;
            size = (int)(Map.MapData.MapSize * TerrainConfig.HeightMapDetail);
            Heights = new float[size, size];

            for(var x = 0; x < size; x ++)
                for(var z = 0; z < size; z ++)
                    Heights[x, z] = 1.0f / TerrainConfig.HeightMapScale;

            Update();
        }

        public void Update()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, size, size, 0, PixelFormat.Luminance, PixelType.Float, Heights);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            normalMap.Generate(Texture);
        }

        public float? GetElevationAtPoint(Vector2 pos)
        {
            var tc = getTextureCoordinate(pos);
            if (tc.X < 0.0f || tc.X >= size - 1 ||
                tc.Y < 0.0f || tc.Y >= size - 1)
                return null;

            var x = (int)tc.X;
            var z = (int)tc.Y;

            var h0 = Heights[z, x];
            var h1 = Heights[z, x + 1];
            var h2 = Heights[z + 1, x];
            var h3 = Heights[z + 1, x + 1];

            var percentU = tc.X - x;
            var percentV = tc.Y - z;

            var dU = percentU > percentV ? h1 - h0 : h3 - h2;
            var dV = percentU > percentV ? h3 - h1 : h2 - h0;
            return (h0 + (dU * percentU) + (dV * percentV)) * TerrainConfig.HeightMapScale;
        }

        private Vector2 getTextureCoordinate(Vector2 mapCoordinate)
        {
            return ((mapCoordinate + new Vector2(Map.MapData.MapSize / 2.0f)) * TerrainConfig.HeightMapDetail);
        }

        internal void ChangeElevation(Vector3 position, float offset)
        {
            var texturePos = getTextureCoordinate(position.Zx);
            var toUpdate = getTilesInArea(texturePos, State.ToolRadius);

            foreach (var i in toUpdate)
            {
                var height = Heights[(int)i.X, (int)i.Y];

                Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

                var amount = Vector2.Distance(texturePos, i);
                var elev = height + calcP(MathF.Min(1f, MathF.Sqrt((amount / State.ToolRadius > (State.ToolHardness * 0.1f) ? amount : 0.0f) / State.ToolRadius))) * offset / TerrainConfig.HeightMapScale;

                Heights[(int)i.X, (int)i.Y] = elev;
            }

            Update();
        }

        private List<Vector2> getTilesInArea(Vector2 center, float radius)
        {
            var included = new List<Vector2>();
            var r = (radius + 2) * TerrainConfig.HeightMapDetail;

            for (var z = center.Y - r; z <= center.Y + r; z++)
                for (var x = center.X - r; x <= center.X + r; x++)
                {
                    if (x > 0 && z > 0 && x < size && z < size)
                        included.Add(new Vector2(x, z));
                }

            return included;
        }
    }
}