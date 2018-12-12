using System;
using Larx.Terrain;
using Larx.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GL4 = OpenTK.Graphics.OpenGL4;

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
        private TextRenderer text;

        private KeyboardState keyboard;

        public Program() : base(
            1280, 720,
            new GraphicsMode(32, 24, 0, 0), "Larx", 0,
            DisplayDevice.Default, 4, 0,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            lastFPSUpdate = 0;
            polygonMode = PolygonMode.Fill;
        }

        protected override void OnLoad(EventArgs e)
        {
            multisampling = new Multisampling(Width, Height, 4);
            GL.Enable(EnableCap.Blend);
            GL4.GL.BlendFuncSeparate(GL4.BlendingFactorSrc.SrcAlpha, GL4.BlendingFactorDest.OneMinusSrcAlpha, GL4.BlendingFactorSrc.One, GL4.BlendingFactorDest.One);

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.FromArgb(255, 24, 24, 24));
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            terrain = new TerrainRenderer();
            camera = new Camera();
            mousePicker = new MousePicker(camera);
            text = new TextRenderer();
            text.CreateText("Text Test", 26);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (keyboard[Key.W]) camera.Move(CameraMoveDirection.Forward);
            if (keyboard[Key.S]) camera.Move(CameraMoveDirection.Back);
            if (keyboard[Key.A]) camera.Move(CameraMoveDirection.Right);
            if (keyboard[Key.D]) camera.Move(CameraMoveDirection.Left);
            camera.Update();

            var mouse = Mouse.GetCursorState();
            var mousePos = this.PointToClient(new Point(mouse.X, mouse.Y));
            mousePicker.Update(mousePos.X, mousePos.Y, Width, Height);

            if (keyboard[Key.T]) terrain.ChangeElevation(0.1f, mousePicker);
            if (keyboard[Key.G]) terrain.ChangeElevation(-0.1f, mousePicker);

            lastFPSUpdate += e.Time;
            if (lastFPSUpdate > 1)
            {
                Title = $"Larx (Vsync: {VSync}) - FPS: {FPS}";
                FPS = 0;
                lastFPSUpdate %= 1;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            multisampling.Bind();
            FPS++;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Blend);
            text.Render(26.0f, 0.2f, 0.0f, 0.0f);
            GL.Disable(EnableCap.Blend);

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
            if (!e.IsRepeat)
            {
                if (e.Keyboard[Key.Escape])
                    Exit();

                if (e.Control && e.Keyboard[Key.F])
                    WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;

                if (e.Control && e.Keyboard[Key.W])
                {
                    polygonMode = polygonMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill;
                    GL.PolygonMode(MaterialFace.FrontAndBack, polygonMode);
                }
            }

            if (!e.Control)
                keyboard = e.Keyboard;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            keyboard = e.Keyboard;
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
