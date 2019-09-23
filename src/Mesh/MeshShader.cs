using OpenTK.Graphics.OpenGL;

namespace Larx.Mesh
{
    public class MeshShader : Shader
    {
        public MeshShader() : base("object") { }

        public int Position { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            Position = GL.GetUniformLocation(Program, "position");
        }
    }
}