using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainShader : Shader
    {
        public TerrainShader() : base("default") { }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
        }
    }
}