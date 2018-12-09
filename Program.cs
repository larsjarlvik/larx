using System;
using Larx.Terrain;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx
{
    class Program : GameWindow
    {
        private int FPS;
        private double lastFPSUpdate;
        private Multisampling multisampling;
        private PolygonMode polygonMode;

        private Camera camera;
        private TerrainRenderer terrain;
        private MousePicker mousePicker;

        public Program() : base(
            1280, 720,
            new GraphicsMode(32, 24, 0, 0), "Larx", 0,
            DisplayDevice.Default, 3, 3,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            Keyboard.KeyRepeat = false;
            lastFPSUpdate = 0;
            polygonMode = PolygonMode.Fill;
        }

        protected override void OnLoad(EventArgs e)
        {
            multisampling = new Multisampling(Width, Height, 4);

            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.ClearColor(Color.FromArgb(255, 24, 24, 24));
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            terrain = new TerrainRenderer();
            camera = new Camera();
            mousePicker = new MousePicker(camera);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.W]) camera.Move(CameraMoveDirection.Forward);
            if (Keyboard[Key.S]) camera.Move(CameraMoveDirection.Back);
            if (Keyboard[Key.A]) camera.Move(CameraMoveDirection.Right);
            if (Keyboard[Key.D]) camera.Move(CameraMoveDirection.Left);

            lastFPSUpdate += e.Time;
            if (lastFPSUpdate > 1)
            {
                Title = $"Larx (Vsync: {VSync}) - FPS: {FPS}";
                FPS = 0;
                lastFPSUpdate %= 1;
            }

            camera.Update();
            mousePicker.Update(Mouse.X, Mouse.Y, Width, Height);

            if (Keyboard[Key.T]) terrain.ChangeElevation(0.1f, mousePicker);
            if (Keyboard[Key.G]) terrain.ChangeElevation(-0.1f, mousePicker);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            multisampling.Bind();
            FPS++;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Render(camera);

            multisampling.Draw();
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            camera.AspectRatio = (float)Width / (float)Height;

            GL.Viewport(0, 0, Width, Height);
            multisampling.RefreshBuffers(Width, Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (e.IsRepeat) return;

            if (Keyboard[Key.Escape]) Exit();

            if (Keyboard[Key.F])
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;

            if (Keyboard[Key.P]) {
                polygonMode = polygonMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill;
                GL.PolygonMode(MaterialFace.FrontAndBack, polygonMode);
            }
        }

        public static void Main(string[] args)
        {
            using (var program = new Program())
            {
                program.VSync = VSyncMode.On;
                program.Run(60);
            }
        }
    }
}
