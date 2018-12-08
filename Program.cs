using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx
{
    class Program : GameWindow
    {
        private DefaultShader shader;
        private float cameraDistance = 20.0f;
        private float cameraRotation = 0.0f;
        private int FPS;
        private double lastFPSUpdate;
        private Multisampling multisampling;
        private PolygonMode polygonMode;

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

            shader = new DefaultShader();

            var triangleArray = GL.GenVertexArray();
            GL.BindVertexArray(triangleArray);
            var positions = new Vector3[] {
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f,  0.5f, 0.0f),
                new Vector3( 0.5f, -0.5f, 0.0f),
                new Vector3( 0.5f,  0.5f, 0.0f)
            };

            var positionBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, positions.Length * Vector3.SizeInBytes, positions, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            var colors = new Vector3[] {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
            };

            var colorBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, colors.Length * Vector3.SizeInBytes, colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.W]) cameraDistance -= 0.5f;
            if (Keyboard[Key.S]) cameraDistance += 0.5f;
            if (Keyboard[Key.A]) cameraRotation -= 0.5f;
            if (Keyboard[Key.D]) cameraRotation += 0.5f;

            var camX = (float)Math.Sin(cameraRotation) * cameraDistance;
            var camZ = (float)Math.Cos(cameraRotation) * cameraDistance;

            Matrix.SetViewMatrix(shader.ViewMatrix, new Vector3(camX, cameraDistance * 0.5f, camZ), new Vector3(0, 0, 0));

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
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            multisampling.Draw();
            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            var aspectRatio = (float)Width / (float)Height;
            Matrix.SetProjectionMatrix(shader.ProjectionMatrix, (float)Math.PI / 4, aspectRatio);

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
                program.VSync = VSyncMode.Off;
                program.Run(60);
            }
        }
    }
}
