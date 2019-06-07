using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

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

        public static class Window
        {
            public static Size Size { get; set; }
            public static float Aspect { get; set; }

            public static void Set(int Width, int Height)
            {
                Size = new Size(Width, Height);
                Aspect = (float)Width / (float)Height;
            }
        }

        public static class Time
        {
            private static double lastFPSUpdate;
            private static int currentFPS;


            public static double Elapsed { get; private set; }
            public static int FPS { get; private set; }

            public static void Set(double time)
            {
                Elapsed = time;
                lastFPSUpdate += time;
                if (lastFPSUpdate > 1)
                {
                    FPS = currentFPS;
                    currentFPS = 0;
                    lastFPSUpdate %= 1;
                }
            }

            public static void CountFPS()
            {
                currentFPS++;
            }
        }

        public static class Mouse
        {
            private static Vector2 lastPosition;
            private static int lastScroll;

            public static Vector2 Position { get; private set; }
            public static Vector2 Delta { get; private set; }
            public static int ScrollDelta { get; private set; }
            public static bool LeftButton { get; private set; }
            public static bool RightButton { get; private set; }

            public static void Set(Point mousePos, MouseState mouse)
            {
                Position = new Vector2(mousePos.X, mousePos.Y);
                Delta = Position - lastPosition;
                ScrollDelta = mouse.ScrollWheelValue - lastScroll;

                LeftButton = mouse.LeftButton == ButtonState.Pressed;
                RightButton = mouse.RightButton == ButtonState.Pressed;

                lastPosition = Position;
                lastScroll =  mouse.ScrollWheelValue;
            }
        }

        public static class Keyboard
        {
            public static KeyboardState Key { get; private set; }

            public static void Set(KeyboardState state)
            {
                Key = state;
            }
        }
    }
}