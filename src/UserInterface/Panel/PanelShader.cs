using OpenTK.Graphics.OpenGL;

namespace Larx.UserInterface.Panel
{
    public class PanelShader : Shader
    {
        public PanelShader() : base("panel") { }

        public int Matrix { get; private set; }
        public int Texture { get; private set; }
        public int Position { get; private set; }
        public int Size { get; private set; }
        public int State { get; private set; }
        public int Active { get; private set; }
        public int BackgroundColor { get; private set; }
        public int PanelType { get; private set; }
        public int BorderWidth { get; private set; }

        protected override void SetUniformsLocations()
        {
            Matrix = GL.GetUniformLocation(Program, "uMatrix");
            Texture = GL.GetUniformLocation(Program, "uTexture");
            Position = GL.GetUniformLocation(Program, "uPosition");
            Size = GL.GetUniformLocation(Program, "uSize");
            State = GL.GetUniformLocation(Program, "uState");
            Active = GL.GetUniformLocation(Program, "uActive");
            BackgroundColor = GL.GetUniformLocation(Program, "uBackgroundColor");
            PanelType = GL.GetUniformLocation(Program, "uPanelType");
            BorderWidth = GL.GetUniformLocation(Program, "uBorderWidth");
        }
    }
}