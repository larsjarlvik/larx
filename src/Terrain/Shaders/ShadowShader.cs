namespace Larx.Terrain.Shaders
{
    public class ShadowShader : BaseShader
    {
        public ShadowShader() : base("terrain-shadows") { }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
        }
    }
}