using OpenTK.Graphics.OpenGL;

namespace Larx.Buffers
{
    public class FramebufferShader : Shader
    {
        public FramebufferShader() : base("framebuffer") { }

        public int Matrix { get; private set; }
        public int Texture { get; private set; }
        public int Size { get; private set; }
        public int Position { get; private set; }

        protected override void SetUniformsLocations()
        {
            Matrix = GL.GetUniformLocation(Program, "uMatrix");
            Texture = GL.GetUniformLocation(Program, "uTexture");
            Size = GL.GetUniformLocation(Program, "uSize");
            Position = GL.GetUniformLocation(Program, "uPosition");
        }
    }
}