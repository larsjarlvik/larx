using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class NormalMapShader : Shader
    {
        public NormalMapShader() : base("normals") { }

        public int Input { get; private set; }
        public int Size { get; private set; }
        public int NormalStrength { get; private set; }

        protected override void SetUniformsLocations()
        {
            Input = GL.GetUniformLocation(Program, "uInput");
            Size = GL.GetUniformLocation(Program, "uSize");
            NormalStrength = GL.GetUniformLocation(Program, "uNormalStrength");
        }
    }
}