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
        public int Texture { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();

            Texture = GL.GetUniformLocation(Program, "uTexture");
        }
    }
}