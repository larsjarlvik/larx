using OpenTK.Graphics.OpenGL;

namespace Larx.Object
{
    public class ObjectShader : Shader
    {
        public ObjectShader() : base("object") { }

        public int Position { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            Position = GL.GetUniformLocation(Program, "position");
        }
    }
}