using System;
using Larx.Terrain;
using Larx.Object;
using Larx.UserInterFace;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GL4 = OpenTK.Graphics.OpenGL4;
using Larx.Water;
using Larx.GltfModel;
using System.Collections.Generic;

namespace Larx
{
    class Program : GameWindow
    {
        private Multisampling multisampling;
        private Camera camera;
        private Light light;
        private ObjectRenderer debug;
        private TerrainRenderer terrain;
        private WaterRenderer water;
        private MousePicker mousePicker;
        private ModelRenderer model;
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
            multisampling = new Multisampling(4);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.ClearColor(Color.FromArgb(255, 156, 207, 210));

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            GL.BindVertexArray(GL.GenVertexArray());

            ui = new Ui();
            terrain = new TerrainRenderer();
            water = new WaterRenderer();
            debug = new ObjectRenderer();
            camera = new Camera();
            light = new Light();
            model = new ModelRenderer("tree");
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
            if (mouse.MiddleButton == ButtonState.Pressed || State.Keyboard.Key[Key.ShiftLeft]) camera.Rotate(State.Mouse.Delta);

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
            GL.PolygonMode(MaterialFace.Front, State.PolygonMode);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ClipDistance0);
            GL.Enable(EnableCap.DepthTest);

            // Water refraction rendering
            water.RefractionBuffer.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Render(camera, light, true, ClipPlane.ClipTop);

            // Water reflection rendering
            camera.InvertY();
            water.ReflectionBuffer.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Render(camera, light, true, ClipPlane.ClipBottom);
            GL.Disable(EnableCap.ClipDistance0);

            model.Render(camera, light, new Vector3(0.0f, (float)terrain.GetElevationAtPoint(new Vector3(0.0f, 0.0f, 0.0f)), 0.0f));
            camera.Reset();

            // Main rendering
            GL.Disable(EnableCap.ClipDistance0);
            multisampling.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Render(camera, light, true, ClipPlane.ClipBottom);
            model.Render(camera, light, new Vector3(0.0f, (float)terrain.GetElevationAtPoint(new Vector3(0.0f, 0.0f, 0.0f)), 0.0f));

            water.Render(camera, light);

            // Draw to screen
            multisampling.Draw();

            // UI and debug
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.Disable(EnableCap.DepthTest);
            GL4.GL.BlendFuncSeparate(GL4.BlendingFactorSrc.SrcAlpha, GL4.BlendingFactorDest.OneMinusSrcAlpha, GL4.BlendingFactorSrc.One, GL4.BlendingFactorDest.One);

            ui.Render();
            // water.ReflectionBuffer.Draw(new Point(State.Window.Size.Width - 330, State.Window.Size.Height - 190), new Size(320, 180));

            SwapBuffers();
            State.Time.CountFPS();
        }

        protected override void OnResize(EventArgs e)
        {
            State.Window.Set(Width, Height);
            GL.Viewport(0, 0, Width, Height);

            multisampling.RefreshBuffers();
            water.RefractionBuffer.Size = State.Window.Size;
            water.RefractionBuffer.RefreshBuffers();

            water.ReflectionBuffer.Size = State.Window.Size;
            water.ReflectionBuffer.RefreshBuffers();
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
