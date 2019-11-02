namespace Larx.TerrainV3
{
    public static class TerrainConfig
    {
        public const float ScaleY = 0.0f;
        public const float ScaleXZ = 1000.0f;
        public const int RootNodes = 8;

        public static readonly float[] LodRange = new float [8] {
            2000.0f, 1000.0f, 500.0f, 200.0f, 100.0f, 0.0f, 0.0f, 0.0f
        };
    }
}