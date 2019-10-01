using OpenTK.Graphics.OpenGL;

namespace Larx.Sky
{
    public class SkyShader : Shader
    {
        public SkyShader() : base("sky") { }
        public int BaseColorTexture { get; private set; }
        public int ClearColor { get; private set; }
        public int FarPlane { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            BaseColorTexture = GL.GetUniformLocation(Program, "uBaseColorTexture");
            ClearColor = GL.GetUniformLocation(Program, "uClearColor");
            FarPlane = GL.GetUniformLocation(Program, "uFarPlane");
        }
    }
}