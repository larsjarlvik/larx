using OpenTK.Graphics.OpenGL;

namespace Larx.Water
{
    public class WaterShader : Shader
    {
        public WaterShader() : base("water") { }

        public int Ambient { get; private set; }
        public int Diffuse { get; private set; }
        public int Specular { get; private set; }
        public int Shininess { get; private set; }
        public int RefractionColorTexture { get; private set; }
        public int RefractionDepthTexture { get; private set; }
        public int Near { get; private set; }
        public int Far { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            RefractionColorTexture = GL.GetUniformLocation(Program, "uRefractionColorTexture");
            RefractionDepthTexture = GL.GetUniformLocation(Program, "uRefractionDepthTexture");
            Near = GL.GetUniformLocation(Program, "uNear");
            Far = GL.GetUniformLocation(Program, "uFar");
        }
    }
}