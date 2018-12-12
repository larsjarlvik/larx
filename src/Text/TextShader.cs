using OpenTK.Graphics.OpenGL;

namespace Larx.Text
{
    public class TextShader : Shader
    {
        public TextShader() : base("text") { }

        public int Matrix { get; private set; }
        public int TextSize { get; private set; }
        public int Texture { get; private set; }
        public int Color { get; private set; }
        public int Buffer { get; private set; }
        public int Gamma { get; private set; }
        public int Position { get; private set; }

        protected override void SetUniformsLocations()
        {
            Matrix = GL.GetUniformLocation(Program, "uMatrix");
            TextSize = GL.GetUniformLocation(Program, "uTexSize");

            Texture = GL.GetUniformLocation(Program, "uTexture");
            Color = GL.GetUniformLocation(Program, "uColor");
            Buffer = GL.GetUniformLocation(Program, "uBuffer");
            Gamma = GL.GetUniformLocation(Program, "uGamma");
            Position = GL.GetUniformLocation(Program, "uPosition");
        }
    }
}