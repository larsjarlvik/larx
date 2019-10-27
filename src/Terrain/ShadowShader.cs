namespace Larx.Terrain
{
    public class ShadowShader : Shader
    {
        public ShadowShader() : base("terrain-shadows") { }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
        }
    }
}