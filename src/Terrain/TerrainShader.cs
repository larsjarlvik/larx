using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainShader : Shader
    {
        public TerrainShader() : base("terrain") { }

        public int Ambient { get; private set; }
        public int Diffuse { get; private set; }
        public int Specular { get; private set; }
        public int Shininess { get; private set; }
        public int Texture { get; private set; }
        public int GridLines { get; private set; }
        public int SplatMap { get; private set; }
        public int SplatCount { get; private set; }
        public int TextureNoise { get; private set; }
        public int MousePosition { get; private set; }
        public int SelectionSize { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();

            Ambient = GL.GetUniformLocation(Program, "uAmbient");
            Diffuse = GL.GetUniformLocation(Program, "uDiffuse");
            Specular = GL.GetUniformLocation(Program, "uSpecular");
            Shininess = GL.GetUniformLocation(Program, "uShininess");
            Texture = GL.GetUniformLocation(Program, "uTexture");
            GridLines = GL.GetUniformLocation(Program, "uGridLines");
            SplatMap = GL.GetUniformLocation(Program, "uSplatMap");
            SplatCount = GL.GetUniformLocation(Program, "uSplatCount");
            TextureNoise = GL.GetUniformLocation(Program, "uTextureNoise");
            SplatCount = GL.GetUniformLocation(Program, "uSplatCount");
            MousePosition = GL.GetUniformLocation(Program, "uMousePosition");
            SelectionSize = GL.GetUniformLocation(Program, "uSelectionSize");
        }
    }
}