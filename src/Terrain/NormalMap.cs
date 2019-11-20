using OpenTK.Graphics.OpenGL;
using Larx.Storage;
using OpenTK;
using Larx.Terrain.Shaders;

namespace Larx.Terrain
{
    public class NormalMap
    {
        public readonly Texture Texture;
        private NormalCompute shader;
        private int size;
        public Vector4[,] Normals;
        public bool HasChanged;

        public NormalMap()
        {
            shader = new NormalCompute();
            size = (int)(Map.MapData.MapSize * TerrainConfig.HeightMapDetail);
            Texture = new Texture();
            Texture.CreateTexture(new Point(size, size));
            HasChanged = true;
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

            HasChanged = true;
        }

        public void UpdateNormalArray()
        {
            var normals = new Vector4[size, size];
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.Float, normals);
            GL.ActiveTexture(TextureUnit.Texture0);
            Normals = normals;
            HasChanged = false;
        }
    }
}