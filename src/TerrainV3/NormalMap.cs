using OpenTK.Graphics.OpenGL;
using Larx.Storage;
using OpenTK;
using System;

namespace Larx.TerrainV3
{
    public class NormalMap
    {
        public readonly Texture Texture;
        private NormalMapShader shader;
        private int size;

        public NormalMap()
        {
            shader = new NormalMapShader();
            size = (int)(Map.MapData.MapSize * TerrainConfig.HeightMapDetail);
            Texture = new Texture();
            Texture.CreateTexture(new Point(size, size));
        }

        public void Generate(int inputTexture)
        {
            GL.UseProgram(shader.Program);
            GL.Uniform1(shader.NormalStrength, 1.0f);
            GL.Uniform1(shader.Size, size);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, inputTexture);
            GL.Uniform1(shader.Input, 0);

            GL.BindImageTexture(0, Texture.TextureId, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);
            GL.DispatchCompute(size / 16, size / 16, 1);
            GL.Finish();
        }
    }
}