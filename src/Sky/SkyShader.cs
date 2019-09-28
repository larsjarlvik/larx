using OpenTK.Graphics.OpenGL;

namespace Larx.Sky
{
    public class SkyShader : Shader
    {
        public SkyShader() : base("sky") { }
        public int BaseColorTexture { get; private set; }
        public int CameraPosition { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            BaseColorTexture = GL.GetUniformLocation(Program, "uBaseColorTexture");
            CameraPosition = GL.GetUniformLocation(Program, "uCameraPosition");
        }
    }
}