namespace Larx.TerrainV3
{
    public static class TerrainConfig
    {
        public const int RootNodes = 8;

        public static readonly int[] LodRange = new int[8] {
            1750, 874, 386, 192, 0, 0, 0, 0
        };

        public static int[] LodMorphAreas = new int[8];

        public const int TessFactor = 600;
        public const float TessSlope = 1.8f;
        public const float TessShift = 0.1f;
        public const float HeightMapDetail = 0.25f;
    }
}