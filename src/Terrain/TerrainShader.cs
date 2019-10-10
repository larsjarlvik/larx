using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainShader : Shader
    {
        public TerrainShader() : base("terrain") { }

        public int Texture { get; private set; }
        public int GridLines { get; private set; }
        public int SplatMap { get; private set; }
        public int SplatCount { get; private set; }
        public int TextureNoise { get; private set; }
        public int MousePosition { get; private set; }
        public int SelectionSize { get; private set; }
        public int ClipPlane { get; private set; }
        public int ShowOverlays { get; private set; }
        public int ShadowMap { get; private set; }
        public int ShadowViewMatrix { get; private set; }
        public int ShadowProjectionMatrix { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();

            Texture = GL.GetUniformLocation(Program, "uTexture");
            GridLines = GL.GetUniformLocation(Program, "uGridLines");
            SplatMap = GL.GetUniformLocation(Program, "uSplatMap");
            SplatCount = GL.GetUniformLocation(Program, "uSplatCount");
            TextureNoise = GL.GetUniformLocation(Program, "uTextureNoise");
            SplatCount = GL.GetUniformLocation(Program, "uSplatCount");
            MousePosition = GL.GetUniformLocation(Program, "uMousePosition");
            SelectionSize = GL.GetUniformLocation(Program, "uSelectionSize");
            ClipPlane = GL.GetUniformLocation(Program, "uClipPlane");
            ShowOverlays = GL.GetUniformLocation(Program, "uShowOverlays");
            ShadowMap = GL.GetUniformLocation(Program, "uShadowMap");
            ShadowViewMatrix = GL.GetUniformLocation(Program, "uShadowViewMatrix");
            ShadowProjectionMatrix = GL.GetUniformLocation(Program, "uShadowProjectionMatrix");
        }
    }
}