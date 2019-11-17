using OpenTK.Graphics.OpenGL;

namespace Larx.Object
{
    public class ObjectShader : Shader
    {
        public ObjectShader() : base("object") { }

        public int Position { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetDefaultUniformLocations();
            Position = GL.GetUniformLocation(Program, "position");
        }
    }
}