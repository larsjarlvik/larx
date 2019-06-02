using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public enum TopMenu
    {
        Terrain,
        Paint,
    }

    public static class State
    {
        public static bool ShowGridLines;
        public static PolygonMode PolygonMode;
        public static float ToolRadius;
        public static float ToolHardness;

        public static TopMenu ActiveTopMenu;
        public static byte ActiveTexture;
    }
}