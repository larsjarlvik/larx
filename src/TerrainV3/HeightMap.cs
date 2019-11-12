using System;
using System.Collections.Generic;
using Larx.Storage;
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
                    heightMap[x, z] = noise[x, z] / 100.0f - 1.0f;

            Update();
        }

        public void Update()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, size, size, 0, PixelFormat.Luminance, PixelType.Float, heightMap);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}