namespace Larx.TerrainV3
{
    public static class TerrainConfig
    {
        public const int RootNodes = 8;

        public static readonly int[] LodRange = new int[8] {
            1750, 874, 386, 192, 100, 50, 0, 0
        };

        public static int[] LodMorphAreas = new int[8];
    }
}