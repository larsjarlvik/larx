using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public class ShadowShader : Shader
    {
        public ShadowShader() : base("asset-shadows") { }

        public int Position { get; private set; }
        public int Rotation { get; private set; }
        public int BaseColorTexture { get; private set; }
        public int ClipPlane { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();

            Position = GL.GetUniformLocation(Program, "uPosition");
            Rotation = GL.GetUniformLocation(Program, "uRotation");
            BaseColorTexture = GL.GetUniformLocation(Program, "uBaseColorTexture");
            ClipPlane = GL.GetUniformLocation(Program, "uClipPlane");
        }
    }
}