using OpenTK.Graphics.OpenGL;

namespace Larx.GltfModel
{
    public class ModelShader : Shader
    {
        public ModelShader() : base("model") { }

        public int Position { get; private set; }
        public int BaseColorTexture { get; private set; }
        public int NormalTexture { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            Position = GL.GetUniformLocation(Program, "uPosition");
            BaseColorTexture = GL.GetUniformLocation(Program, "uBaseColorTexture");
            NormalTexture = GL.GetUniformLocation(Program, "uNormalTexture");
        }
    }
}