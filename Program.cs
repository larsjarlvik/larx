using System;
using Larx.Object;
using Larx.UserInterface;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GL4 = OpenTK.Graphics.OpenGL4;
using Larx.Water;
using Larx.Sky;
using Larx.Storage;
using Larx.Shadows;
using Larx.Buffers;
using Larx.Utils;
using Larx.Terrain;
using Larx.Assets;

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
        private SkyRenderer sky;
        private AssetRenderer assets;
        private ShadowBox shadows;
        private Ui ui;

        public Program() : base(
            1280, 720,
            new GraphicsMode(32, 24, 0, 0), "Larx", 0,
            DisplayDevice.Default, 4, 5,
            GraphicsContextFlags.ForwardCompatible)
        {
            State.PolygonMode = PolygonMode.Fill;
        }

        protected override void OnLoad(EventArgs e)
        {
            multisampling = new Multisampling(4);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.ClearColor(State.ClearColor);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.CullFace(CullFaceMode.Back);
            GL.MinSampleShading(1.0f);

            Map.New(2048);
            debug = new ObjectRenderer();
            camera = new Camera();
            terrain = new TerrainRenderer(camera);
            water = new WaterRenderer();
            light = new Light();
            assets = new AssetRenderer();
            ui = new Ui();
            sky = new SkyRenderer();
            shadows = new ShadowBox();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var mouse = Mouse.GetCursorState();

            State.Mouse.Set(PointToClient(new Point(mouse.X, mouse.Y)), mouse);
            State.Time.Set(e.Time);

            var uiIntersect = ui.Update();

            if (Focused)
            {
                if (State.Mouse.ScrollDelta > 0) camera.Zoom(-0.2f);
                if (State.Mouse.ScrollDelta < 0) camera.Zoom( 0.2f);
                if (mouse.MiddleButton == ButtonState.Pressed || State.Keyboard.Key[Key.ShiftLeft]) camera.Rotate(State.Mouse.Delta);

                if (State.Keyboard.Key[Key.W]) camera.Move(new Vector3( 0.0f, 0.0f, 1.0f));
                if (State.Keyboard.Key[Key.S]) camera.Move(new Vector3( 0.0f, 0.0f,-1.0f));
                if (State.Keyboard.Key[Key.A]) camera.Move(new Vector3( 1.0f, 0.0f, 0.0f));
                if (State.Keyboard.Key[Key.D]) camera.Move(new Vector3(-1.0f, 0.0f, 0.0f));

                if (State.Keyboard.Key[Key.Up]) light.Rotation.Y += 0.01f;
                if (State.Keyboard.Key[Key.Down]) light.Rotation.Y -= 0.01f;
                if (State.Keyboard.Key[Key.Left]) light.Rotation.X += 0.01f;
                if (State.Keyboard.Key[Key.Right]) light.Rotation.X -= 0.01f;
            }

            camera.Update((float)e.Time);
            light.Update();
            shadows.Update(camera, light);
            terrain.Update();

            if (!uiIntersect) {
                switch (ui.State.ActiveTopMenuKey)
                {
                    case UiKeys.TopMenu.ElevationTools:
                        switch(ui.State.ActiveChildMenuKey)
                        {
                            case UiKeys.Terrain.ElevationTool:
                                if (mouse.LeftButton == ButtonState.Pressed) {
                                    terrain.HeightMap.Sculpt(terrain.MousePosition, 0.1f);
                                    assets.Refresh(terrain);
                                } else if (mouse.RightButton == ButtonState.Pressed) {
                                    terrain.HeightMap.Sculpt(terrain.MousePosition, -0.1f);
                                    assets.Refresh(terrain);
                                }
                                break;
                            case UiKeys.Terrain.SmudgeTool:
                                if (mouse.LeftButton == ButtonState.Pressed) {
                                    terrain.HeightMap.Smudge(terrain.MousePosition);
                                    assets.Refresh(terrain);
                                }
                                break;
                        }
                        break;
                    case UiKeys.TopMenu.TerrainPaint:
                        if (mouse.LeftButton == ButtonState.Pressed) {
                            if (ui.State.ActiveChildMenuKey == UiKeys.SplatMap.AutoPaint)
                                terrain.SplatMap.AutoPaint(terrain.HeightMap, terrain.NormalMap, terrain.MousePosition);
                            else
                                terrain.SplatMap.Paint(terrain.MousePosition, byte.Parse(ui.State.ActiveChildMenuKey));
                        }
                        break;
                    case UiKeys.TopMenu.Assets:
                        if (mouse.LeftButton == ButtonState.Pressed) {
                            if (ui.State.ActiveChildMenuKey == UiKeys.Assets.Erase) {
                                assets.Remove(terrain.MousePosition.Xz, terrain);
                            } else if (!ui.State.MouseRepeat) {
                                assets.Add(terrain.MousePosition.Xz, terrain, ui.State.ActiveChildMenuKey);
                            }
                        }
                        break;
                }
            } else {
                switch (ui.State.PressedKey)
                {
                    case UiKeys.Terrain.LevelRaise:
                        terrain.HeightMap.ChangeSettings(0.1f, 1.0f);
                        assets.Refresh(terrain);
                        break;
                    case UiKeys.Terrain.LevelLower:
                        terrain.HeightMap.ChangeSettings(-0.1f, 1.0f);
                        assets.Refresh(terrain);
                        break;
                    case UiKeys.Terrain.StrengthIncrease:
                        terrain.HeightMap.ChangeSettings(0.0f, 1.02f);
                        assets.Refresh(terrain);
                        break;
                    case UiKeys.Terrain.StrengthDecrease:
                        terrain.HeightMap.ChangeSettings(0.0f, 0.98f);
                        assets.Refresh(terrain);
                        break;
                    case UiKeys.SplatMap.AutoPaintGlobal:
                        terrain.SplatMap.AutoPaint(terrain.HeightMap, terrain.NormalMap);
                        break;
                }
            }

            ui.UpdateText(UiKeys.Texts.Position, $"Position: {terrain.MousePosition.X:0.##} {terrain.MousePosition.Z:0.##}");
            Title = $"Larx (Vsync: {VSync}) - FPS: {State.Time.FPS}";
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Enable(EnableCap.ClipDistance0);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

            // Shadow rendering
            shadows.ShadowBuffer.Bind();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            assets.RenderShadowMap(shadows, terrain);
            terrain.RenderShadowMap(camera, shadows, ClipPlane.ClipBottom);

            GL.ClipControl(ClipOrigin.LowerLeft, ClipDepthMode.ZeroToOne);

            // Water refraction rendering
            water.RefractionBuffer.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            terrain.Render(camera, light, null, ClipPlane.ClipTop);
            assets.Render(camera, light, null, terrain, ClipPlane.ClipTop);

            // Water reflection rendering
            camera.InvertY();
            water.ReflectionBuffer.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            terrain.Render(camera, light, null, ClipPlane.ClipBottom);
            assets.Render(camera, light, null, terrain, ClipPlane.ClipBottom);
            sky.Render(camera, light);
            GL.Disable(EnableCap.ClipDistance0);
            camera.Reset();

            // Main rendering
            multisampling.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, State.PolygonMode);

            sky.Render(camera, light);
            terrain.Render(camera, light, shadows, ClipPlane.ClipBottom);
            water.Render(camera, light, shadows);

            GL.Enable(EnableCap.SampleShading);
            assets.Render(camera, light, shadows, terrain, ClipPlane.ClipBottom);

            GL.Disable(EnableCap.SampleShading);

            // Draw to screen
            multisampling.Draw();

            GL.Enable(EnableCap.Blend);
            // UI and debug
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.Disable(EnableCap.DepthTest);
            GL4.GL.BlendFuncSeparate(GL4.BlendingFactorSrc.SrcAlpha, GL4.BlendingFactorDest.OneMinusSrcAlpha, GL4.BlendingFactorSrc.One, GL4.BlendingFactorDest.One);

            GL.ClipControl(ClipOrigin.LowerLeft, ClipDepthMode.NegativeOneToOne);
            ui.Render();
            // shadows.ShadowBuffer.DrawDepthBuffer();

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

            shadows.ShadowBuffer.RefreshBuffers();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var mouse = Mouse.GetCursorState();
            State.Mouse.Set(PointToClient(new Point(mouse.X, mouse.Y)), mouse);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                if (e.Keyboard[Key.Escape])
                    Exit();

                if (e.Control) {
                    if (e.Keyboard[Key.F])
                        WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;

                    if (e.Keyboard[Key.W])
                        State.PolygonMode = State.PolygonMode == PolygonMode.Fill ? PolygonMode.Line : PolygonMode.Fill;

                    if (e.Keyboard[Key.G])
                        State.ShowGridLines = !State.ShowGridLines;

                    if (e.Keyboard[Key.S])
                        Map.Save(terrain);

                    if (e.Keyboard[Key.O])
                        Map.Load(terrain, assets);

                    if (e.Keyboard[Key.H])
                        terrain.HeightMap.LoadFromImage();
                }
            }

            if (e.Key == Key.BackSpace)
                ui.KeyPress((char)27);

            if (!e.Control)
                State.Keyboard.Set(e.Keyboard);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            ui.KeyPress(e.KeyChar);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            State.Keyboard.Set(e.Keyboard);
        }

        public static void Main(string[] args)
        {
            Nvidia.InitializeDedicatedGraphics();

            using (var program = new Program())
            {
                program.VSync = VSyncMode.Off;
                program.Run(60);
            }
        }
    }
}
