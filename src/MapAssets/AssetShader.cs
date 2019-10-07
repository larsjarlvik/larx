using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public class AssetShader : Shader
    {
        public AssetShader() : base("asset") { }

        public int Position { get; private set; }
        public int Rotation { get; private set; }
        public int BaseColorTexture { get; private set; }
        public int NormalTexture { get; private set; }
        public int Roughness { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            Position = GL.GetUniformLocation(Program, "uPosition");
            Rotation = GL.GetUniformLocation(Program, "uRotation");
            BaseColorTexture = GL.GetUniformLocation(Program, "uBaseColorTexture");
            NormalTexture = GL.GetUniformLocation(Program, "uNormalTexture");
            Roughness = GL.GetUniformLocation(Program, "uRoughness");
        }
    }
}