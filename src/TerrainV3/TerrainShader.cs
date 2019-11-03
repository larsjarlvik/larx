using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainShader : Shader
    {
        public TerrainShader() : base("terrain-v2") { }

        public int Position { get; private set; }
        public int Size { get; private set; }
        public int Scale { get; private set; }
        public int Depth { get; private set; }
        public int LocalMatrix { get; private set; }
        public int WorldMatrix { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            Position = GL.GetUniformLocation(Program, "uPosition");
            Size = GL.GetUniformLocation(Program, "uSize");
            Scale = GL.GetUniformLocation(Program, "uScale");
            Depth = GL.GetUniformLocation(Program, "uDepth");
            LocalMatrix = GL.GetUniformLocation(Program, "uLocalMatrix");
            WorldMatrix = GL.GetUniformLocation(Program, "uWorldMatrix");
        }
    }
}