using System;
using Larx.Terrain;
using Larx.Object;
using Larx.Text;
using Larx.UserInterFace;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GL4 = OpenTK.Graphics.OpenGL4;
using Larx.Water;

namespace Larx
{
    class Program : GameWindow
    {
        private Framebuffer multisampling;
        private Camera camera;
        private Light light;
        private ObjectRenderer debug;
        private TerrainRenderer terrain;
        private WaterRenderer water;
        private MousePicker mousePicker;
        private Ui ui;

        public Program() : base(
            1280, 720,
            new GraphicsMode(32, 24, 0, 0), "Larx", 0,
            DisplayDevice.Default, 4, 0,
            GraphicsContextFlags.Debug)
        {
            State.PolygonMode = PolygonMode.Fill;
            State.ToolRadius = 3f;
            State.ToolHardness = 0.5f;
        }

        protected override void OnLoad(EventArgs e)
        {
            multisampling = new Framebuffer(4, State.Window.Size);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL4.GL.BlendFuncSeparate(GL4.BlendingFactorSrc.SrcAlpha, GL4.BlendingFactorDest.OneMinusSrcAlpha, GL4.BlendingFactorSrc.One, GL4.BlendingFactorDest.One);

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.FromArgb(255, 24, 24, 24));
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            ui = new Ui();
            terrain = new TerrainRenderer();
            water = new WaterRenderer();
            debug = new ObjectRenderer();
            camera = new Camera();
            light = new Light();
            mousePicker = new MousePicker(camera);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var mouse = Mouse.GetCursorState();

            State.Mouse.Set(PointToClient(new Point(mouse.X, mouse.Y)), mouse);
            State.Time.Set(e.Time);

            var uiIntersect = ui.Update();

            if (State.Mouse.ScrollDelta > 0) camera.Zoom(-0.2f);
            if (State.Mouse.ScrollDelta < 0) camera.Zoom( 0.2f);
            if (mouse.MiddleButton == ButtonState.Pressed) camera.Rotate(State.Mouse.Delta);

            if (State.Keyboard.Key[Key.W]) camera.Move(new Vector3( 0.0f, 0.0f, 1.0f));
            if (State.Keyboard.Key[Key.S]) camera.Move(new Vector3( 0.0f, 0.0f,-1.0f));
            if (State.Keyboard.Key[Key.A]) camera.Move(new Vector3( 1.0f, 0.0f, 0.0f));
            if (State.Keyboard.Key[Key.D]) camera.Move(new Vector3(-1.0f, 0.0f, 0.0f));

            if (State.Keyboard.Key[Key.Up]) light.Position += new Vector3( 0.0f, 0.0f, 1.0f);
            if (State.Keyboard.Key[Key.Down]) light.Position += new Vector3( 0.0f, 0.0f,-1.0f);
            if (State.Keyboard.Key[Key.Left]) light.Position += new Vector3( 1.0f, 0.0f, 0.0f);
            if (State.Keyboard.Key[Key.Right]) light.Position += new Vector3(-1.0f, 0.0f, 0.0f);

            camera.Update((float)e.Time);
            mousePicker.Update();
            terrain.Update(mousePicker);

            if (!uiIntersect) {
                switch (State.ActiveTopMenu)
                {
                    case TopMenu.Terrain:
                        if (mouse.LeftButton == ButtonState.Pressed) terrain.ChangeElevation(0.1f, mousePicker);
                        if (mouse.RightButton == ButtonState.Pressed) terrain.ChangeElevation(-0.1f, mousePicker);
                        break;
                    case TopMenu.Paint:
                        if (mouse.LeftButton == ButtonState.Pressed) terrain.Paint();
                        break;
                }
            }

            ui.UpdateText("position", $"Position: {terrain.MousePosition.X:0.##} {terrain.MousePosition.Z:0.##}");
            Title = $"Larx (Vsync: {VSync}) - FPS: {State.Time.FPS}";
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            multisampling.Bind();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, State.PolygonMode);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);

            terrain.Render(camera, light);
            water.Render(camera, light);
            multisampling.Draw();

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);

            ui.Render();

            SwapBuffers();
            State.Time.CountFPS();
        }

        protected override void OnResize(EventArgs e)
        {
            State.Window.Set(Width, Height);
            GL.Viewport(0, 0, Width, Height);

            multisampling.Size = State.Window.Size;
            multisampling.RefreshBuffers();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var mouse = Mouse.GetCursorState();
            State.Mouse.Set(PointToClient(new Point(mouse.X, mouse.Y)), mouse);
            ui.Click();
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
                    State.PolygonMode = State.PolygonMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill;

                if (e.Control && e.Keyboard[Key.G])
                    State.ShowGridLines = !State.ShowGridLines;
            }

            if (!e.Control)
                State.Keyboard.Set(e.Keyboard);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            State.Keyboard.Set(e.Keyboard);
        }

        public static void Main(string[] args)
        {
            using (var program = new Program())
            {
                program.VSync = VSyncMode.Off;
                program.Run(60);
            }
        }
    }
}
