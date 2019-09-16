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
        public int ReflectionColorTexture { get; private set; }
        public int Near { get; private set; }
        public int Far { get; private set; }
        public int NormalMap { get; private set; }
        public int DuDvMap { get; private set; }
        public int TimeOffset { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            RefractionColorTexture = GL.GetUniformLocation(Program, "uRefractionColorTexture");
            RefractionDepthTexture = GL.GetUniformLocation(Program, "uRefractionDepthTexture");
            ReflectionColorTexture = GL.GetUniformLocation(Program, "uReflectionColorTexture");
            Near = GL.GetUniformLocation(Program, "uNear");
            Far = GL.GetUniformLocation(Program, "uFar");
            NormalMap = GL.GetUniformLocation(Program, "uNormalMap");
            DuDvMap = GL.GetUniformLocation(Program, "uDuDvMap");
            TimeOffset = GL.GetUniformLocation(Program, "uTimeOffset");
        }
    }
}