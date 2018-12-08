using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class DefaultShader : Shader
    {
        public int ProjectionMatrix { get; private set; }
        public int ViewMatrix { get; private set; }

        public DefaultShader() : base("default") { }

        protected override void SetUniforms()
        {
            ProjectionMatrix = GL.GetUniformLocation(Program, "uProjectionMatrix");
            ViewMatrix = GL.GetUniformLocation(Program, "uViewMatrix");
        }
    }
}