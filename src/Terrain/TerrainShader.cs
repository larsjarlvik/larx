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
        }
    }
}