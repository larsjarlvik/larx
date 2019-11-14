using System;
using System.Collections.Generic;
using Larx.Storage;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class HeightMap
    {
        public int Texture;
        private int size;
        private float[,] heightMap;

        public HeightMap()
        {
            size = (int)(Map.MapData.MapSize * TerrainConfig.HeightMapDetail);
            heightMap = new float[size, size];
            var noise = SimplexNoise.Noise.Calc2D(size, size, 0.02f);

            for(var x = 0; x < size; x ++)
                for(var z = 0; z < size; z ++)
                    heightMap[x, z] = noise[x, z] * 0.005f - 0.5f;

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
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, size, size, 0, PixelFormat.Luminance, PixelType.Float, heightMap);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public float? GetElevationAtPoint(Vector2 position)
        {
            Vector3 index0;
            Vector3 index1;
            Vector3 index2;

            var tc = getTextureCoordinate(position);

            if (tc.X < 0.0f || tc.X >= size - 1 ||
                tc.Y < 0.0f || tc.Y >= size - 1)
                return null;

            if ((tc.X % 1) + (tc.Y % 1) > 0.5f) {
                index0 = new Vector3(tc.X, heightMap[(int)tc.X, (int)tc.Y], tc.Y);
                index1 = new Vector3(tc.X + 1, heightMap[(int)tc.X + 1, (int)tc.Y], tc.Y);
                index2 = new Vector3(tc.X, heightMap[(int)tc.X, (int)tc.Y + 1], tc.Y + 1);
            } else {
                index0 = new Vector3(tc.X + 1, heightMap[(int)tc.X + 1, (int)tc.Y], tc.Y);
                index1 = new Vector3(tc.X, heightMap[(int)tc.X, (int)tc.Y + 1], tc.Y + 1);
                index2 = new Vector3(tc.X + 1, heightMap[(int)tc.X + 1, (int)tc.Y + 1], tc.Y + 1);
            }

            return MathLarx.BaryCentric(index0, index1, index2, tc) * TerrainConfig.HeightMapScale;
        }

        private Vector2 getTextureCoordinate(Vector2 mapCoordinate)
        {
            return ((mapCoordinate + new Vector2(Map.MapData.MapSize / 2.0f)) * TerrainConfig.HeightMapDetail).Yx;
        }
    }
}