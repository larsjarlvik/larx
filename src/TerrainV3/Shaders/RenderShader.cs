using Larx.Terrain;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3.Shaders
{
    public class RenderShader : BaseShader
    {
        public RenderShader() : base("terrain-v2") { }

        public int NormalMap { get; private set; }
        public int Texture { get; private set; }
        public int GridLines { get; private set; }
        public int MousePosition { get; private set; }
        public int SelectionSize { get; private set; }
        public int SplatMap { get; private set; }
        public int SplatCount { get; internal set; }
        public int TextureNoise { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            base.SetShadowUniformLocations();

            NormalMap = GL.GetUniformLocation(Program, "uNormalMap");
            Texture = GL.GetUniformLocation(Program, "uTexture");
            GridLines = GL.GetUniformLocation(Program, "uGridLines");
            MousePosition = GL.GetUniformLocation(Program, "uMousePosition");
            SelectionSize = GL.GetUniformLocation(Program, "uSelectionSize");
            SplatMap = GL.GetUniformLocation(Program, "uSplatMap");
            SplatCount = GL.GetUniformLocation(Program, "uSplatCount");
            TextureNoise = GL.GetUniformLocation(Program, "uTextureNoise");
        }
    }
}