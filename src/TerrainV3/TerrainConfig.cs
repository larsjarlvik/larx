namespace Larx.TerrainV3
{
    public static class TerrainConfig
    {
        public const int RootNodes = 8;

        public static readonly int[] LodRange = new int[8] {
            1750, 874, 386, 192, 100, 50, 0, 0
        };

        public static int[] LodMorphAreas = new int[8];

        public const int TessFactor = 600;
        public const float TessSlope = 1.8f;
        public const float TessShift = 0.3f;
        public const float HeightMapDetail = 0.25f;
        public const float HeightMapScale = 25.0f;

        public static readonly string[] Textures = new [] {
            "grass-1",
            "grass-2",
            "grass-3",
            "grass-4",
            "bare-1",
            "bare-2",
            "bare-3",
            "sand-1",
            "sand-2",
            "sand-3",
            "sand-4",
            "rock-1",
            "rock-2"
        };
    }
}